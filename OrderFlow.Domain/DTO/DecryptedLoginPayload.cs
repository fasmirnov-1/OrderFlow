namespace OrderFlow.Domain.DTO
{
    /// <summary>
    /// Структура данных внутри распакованного из Base64 крипто-пакета.
    /// </summary>
    public class DecryptedLoginPayload
    {
        public string Login { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string SessionHash { get; set; } = string.Empty;
    }
}