using System.Collections.Concurrent;

namespace OrderFlow.Application.Services
{
    public class SessionManagerService
    {
        // Ключ — чистая строка GUID (или хэш, переданный извне), Значение — структура данных
        private readonly ConcurrentDictionary<string, (Guid Id, DateTime LastSeen)> _sessions = new();
        private readonly TimeSpan _sessionTimeout = TimeSpan.FromMinutes(30);

        public SessionManagerService()
        {
        }

        /// <summary>
        /// Создает временную сессию и возвращает строковый идентификатор для туннеля.
        /// </summary>
        public string CreateSession()
        {
            Guid sessionGuid = Guid.NewGuid();

            // За основу ключа берем строковое представление. 
            // Если вам принципиально нужен хэш в качестве ключа, мы возвращаем именно его!
            string sessionKey = CryptoService.GetSha256String(sessionGuid);

            // Сохраняем в потокобезопасный словарь
            _sessions[sessionKey] = (sessionGuid, DateTime.UtcNow);

            // ИСПРАВЛЕНО: Возвращаем именно тот ключ, под которым сохранили в памяти!
            return sessionKey;
        }

        /// <summary>
        /// Проверяет наличие сессии по ее ключу (хэшу).
        /// </summary>
        /// <summary>
        /// Проверяет наличие сессии по ее ключу (хэшу) и атомарно продлевает её жизнь.
        /// </summary>
        public Guid? GetSession(string sessionHash)
        {
            if (string.IsNullOrEmpty(sessionHash))
            {
                return null;
            }

            // 1. Пытаемся достать данные из словаря
            if (!_sessions.TryGetValue(sessionHash, out var sessionData))
            {
                return null;
            }

            // 2. Проверка таймаута бездействия
            if (DateTime.UtcNow - sessionData.LastSeen > _sessionTimeout)
            {
                RemoveSession(sessionHash);
                return null;
            }

            // 3. ИСПРАВЛЕНО: Потокобезопасное продление времени жизни (Sliding Expiration)
            // Метод TryUpdate гарантирует, что мы обновим запись только если её не изменил/удалил другой поток
            var updatedData = (sessionData.Id, LastSeen: DateTime.UtcNow);

            // Если обновить не удалось (например, сессия была удалена параллельно), возвращаем null
            if (!_sessions.TryUpdate(sessionHash, updatedData, sessionData))
            {
                // На случай гонки потоков проверяем, существует ли она ещё вообще
                if (!_sessions.TryGetValue(sessionHash, out sessionData))
                {
                    return null;
                }
            }

            return sessionData.Id;
        }

        /// <summary>
        /// Принудительное удаление временной сессии после авторизации.
        /// </summary>
        public void RemoveSession(string sessionHash)
        {
            if (!string.IsNullOrEmpty(sessionHash))
            {
                _sessions.TryRemove(sessionHash, out _);
            }
        }
    }
}