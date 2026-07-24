using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace OrderFlow.Application.Middlewares
{
    public class UrlDecryptionMiddleware
    {
        private readonly RequestDelegate _next;
        // Ключ шифрования (32 байта)
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("MySuperSecretKeyForEncryption123");

        public UrlDecryptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 1. Проверяем, есть ли в пакете зашифрованный заголовок пути[cite: 11]
            if (context.Request.Headers.TryGetValue("X-Encrypted-Path", out var encryptedPathHeader))
            {
                try
                {
                    // 2. Расшифровываем реальный URL-путь (например, "/UserRegister/Register?type=admin")[cite: 11]
                    string decryptedUrl = DecryptString(encryptedPathHeader!);

                    // Разделяем путь и Query-параметры (если они есть)[cite: 11]
                    string[] urlParts = decryptedUrl.Split('?');

                    // ПОДМЕНЯЕМ HTTP-контекст для приложения[cite: 11]
                    context.Request.Path = urlParts[0];
                    if (urlParts.Length > 1)
                    {
                        context.Request.QueryString = new QueryString("?" + urlParts[1]);
                    }

                    // 3. Расшифровываем тело запроса (Body), если оно передано[cite: 11]
                    if (context.Request.ContentType != null && context.Request.ContentType.Contains("application/json"))
                    {
                        using var reader = new StreamReader(context.Request.Body);
                        string bodyJson = await reader.ReadToEndAsync();

                        if (!string.IsNullOrEmpty(bodyJson))
                        {
                            using var doc = JsonDocument.Parse(bodyJson);
                            if (doc.RootElement.TryGetProperty("payload", out var payloadProp))
                            {
                                string? encryptedPayload = payloadProp.GetString();
                                if (!string.IsNullOrEmpty(encryptedPayload))
                                {
                                    string decryptedBodyJson = DecryptString(encryptedPayload);

                                    // Подменяем поток тела запроса на чистый JSON для контроллеров[cite: 11]
                                    var decryptedBytes = Encoding.UTF8.GetBytes(decryptedBodyJson);
                                    var memoryStream = new MemoryStream(decryptedBytes);
                                    context.Request.Body = memoryStream;
                                    context.Request.ContentLength = memoryStream.Length;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync($"Запрос поврежден или заблокирован: {ex.Message}");
                    return;
                }
            }

            // Передаем управление дальше. Контроллер получит правильный HTTP-запрос с правильным Path и Query[cite: 11]
            await _next(context);
        }

        private static string DecryptString(string combinedInput)
        {
            string[] parts = combinedInput.Split(':');
            byte[] iv = Convert.FromBase64String(parts[0]);
            byte[] fullCipherWithTag = Convert.FromBase64String(parts[1]);

            byte[] tag = new byte[16];
            byte[] cipherText = new byte[fullCipherWithTag.Length - tag.Length];

            Buffer.BlockCopy(fullCipherWithTag, 0, cipherText, 0, cipherText.Length);
            Buffer.BlockCopy(fullCipherWithTag, cipherText.Length, tag, 0, tag.Length);

            byte[] decryptedBytes = new byte[cipherText.Length];

            using (AesGcm aesGcm = new AesGcm(Key))
            {
                aesGcm.Decrypt(iv, cipherText, tag, decryptedBytes);
            }

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}