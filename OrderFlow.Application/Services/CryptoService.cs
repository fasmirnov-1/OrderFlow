using System.Security.Cryptography;
using System.Text;

namespace OrderFlow.Application.Services
{
    /// <summary>
    /// Статический сервис для выполнения криптографических операций приложения.
    /// Не требует регистрации в DI-контейнере.
    /// </summary>
    public static class CryptoService
    {
        /// <summary>
        /// Генерирует SHA256 хэш из Guid без выделения лишней памяти в куче.
        /// </summary>
        public static string GetSha256String(Guid guid)
        {
            // Выделяем 16 байт под GUID на стеке (без аллокаций в куче)
            Span<byte> guidBytes = stackalloc byte[16];
            guid.TryWriteBytes(guidBytes);

            // Выделяем 32 байта под результат хэша SHA256 на стеке
            Span<byte> hashBytes = stackalloc byte[32];
            SHA256.HashData(guidBytes, hashBytes);

            // Преобразуем стек-буфер напрямую в Base64 строку
            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Безопасно сравнивает два Base64 хэша за фиксированное время (защита от Timing Attacks).
        /// </summary>
        public static bool VerifyHashes(string storedHashBase64, string computedHashBase64)
        {
            if (string.IsNullOrEmpty(storedHashBase64) || string.IsNullOrEmpty(computedHashBase64))
            {
                return false;
            }

            try
            {
                byte[] storedBytes = Convert.FromBase64String(storedHashBase64);
                byte[] computedBytes = Convert.FromBase64String(computedHashBase64);

                return CryptographicOperations.FixedTimeEquals(storedBytes, computedBytes);
            }
            catch (FormatException)
            {
                // На случай, если передан некорректный формат строки Base64
                return false;
            }
        }
        public static string GetSha256String(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            // 1. Вычисляем максимальное количество байт, которое может занять строка в UTF-8.
            // Для коротких строк (пароли, логины, GUID) это число всегда небольшое.
            int maxByteCount = Encoding.UTF8.GetByteCount(text);

            // 2. Выделяем память под байты строки и под будущий хэш на стеке (без аллокаций в куче)
            Span<byte> textBytes = stackalloc byte[maxByteCount];
            Span<byte> hashBytes = stackalloc byte[32]; // SHA256 всегда возвращает 32 байта

            // 3. Кодируем строку напрямую в стек-буфер
            int writtenTextBytes = Encoding.UTF8.GetBytes(text, textBytes);

            // 4. Считаем хэш, используя только заполненную часть буфера
            SHA256.HashData(textBytes[..writtenTextBytes], hashBytes);

            // 5. Преобразуем массив байт хэша из стека в Base64 строку
            return Convert.ToBase64String(hashBytes);
        }
    }
}