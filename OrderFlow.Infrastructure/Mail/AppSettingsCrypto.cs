using System.Security.Cryptography;
using System.Text;

namespace OrderFlow.Infrastructure.Mail
{
    public static class AppSettingsCrypto
    {
        // Защита привязывается к текущему пользователю операционной системы (DPAPI)
        private static readonly DataProtectionScope Scope = DataProtectionScope.CurrentUser;

        /// <summary>
        /// Зашифровывает открытый текст в Base64 строку с использованием системного DPAPI.
        /// </summary>
        /// <param name="plainText">Исходный текст (например, пароль).</param>
        /// <returns>Зашифрованная строка в формате Base64 либо исходная строка, если она пуста.</returns>
        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return plainText;
            }

            try
            {
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedBytes = ProtectedData.Protect(plainBytes, null, Scope);
                return Convert.ToBase64String(encryptedBytes);
            }
            catch (Exception ex)
            {
                // Логирование ошибки шифрования (при необходимости можно заменить на ваш ILogger)
                throw new InvalidOperationException("Ошибка при шифровании данных с использованием DPAPI.", ex);
            }
        }

        /// <summary>
        /// Расшифровывает Base64 строку обратно в исходный текст.
        /// </summary>
        /// <param name="encryptedText">Зашифрованная строка в формате Base64.</param>
        /// <returns>Расшифрованный текст или пустая строка в случае сбоя.</returns>
        public static string Decrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
            {
                return encryptedText;
            }

            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                byte[] plainBytes = ProtectedData.Unprotect(encryptedBytes, null, Scope);
                return Encoding.UTF8.GetString(plainBytes);
            }
            catch (FormatException)
            {
                // Если строка не является корректным Base64, возможно, она передана в открытом виде
                return encryptedText;
            }
            catch
            {
                // Возвращаем пустую строку, если ключ не подошел, сменился пользователь или повреждены данные
                return string.Empty;
            }
        }
    }
}