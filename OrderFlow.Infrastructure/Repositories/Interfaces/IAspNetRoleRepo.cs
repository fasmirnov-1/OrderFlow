using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories
{
    /// <summary>
    /// Интерфейс репозитория для управления ролями пользователей (AspNetRoles) 
    /// с учетом ограничений уникальности и ссылочной целостности базы данных.
    /// </summary>
    public interface IAspNetRoleRepo
    {
        /// <summary>
        /// Возвращает роль по её строковому идентификатору (Id).
        /// </summary>
        /// <param name="id">Строковый уникальный идентификатор роли.</param>
        /// <returns>Экземпляр <see cref="AspNetRole"/> или null, если роль не найдена.</returns>
        Task<AspNetRole?> GetByIdAsync(string id);

        /// <summary>
        /// Возвращает полный список всех ролей в системе без отслеживания изменений (AsNoTracking).
        /// </summary>
        /// <returns>Коллекция всех существующих ролей.</returns>
        Task<IEnumerable<AspNetRole>> GetAllAsync();

        /// <summary>
        /// Добавляет новую роль в базу данных.
        /// Перед сохранением превентивно проверяет уникальность полей Name и NormalizedName.
        /// </summary>
        /// <param name="role">Сущность новой роли для добавления.</param>
        /// <exception cref="ArgumentNullException">Вызывается, если передана пустая сущность.</exception>
        /// <exception cref="ArgumentException">Вызывается при нарушении Unique Constraint (если Name или NormalizedName уже заняты).</exception>
        Task AddAsync(AspNetRole role);

        /// <summary>
        /// Обновляет данные существующей роли.
        /// Превентивно проверяет ограничения уникальности Name и NormalizedName среди других записей в базе данных.
        /// </summary>
        /// <param name="role">Сущность роли с измененными данными.</param>
        /// <exception cref="ArgumentNullException">Вызывается, если передана пустая сущность.</exception>
        /// <exception cref="ArgumentException">Вызывается, если обновляемое имя роли конфликтует с уже существующей другой ролью.</exception>
        Task UpdateAsync(AspNetRole role);

        /// <summary>
        /// Удаляет роль по её идентификатору.
        /// Превентивно проверяет Restrict Constraints: запрещает удаление, если к роли привязаны права (AspNetRoleClaims) 
        /// или если роль назначена хотя бы одному пользователю (связь Many-to-Many с AspNetUsers).
        /// </summary>
        /// <param name="id">Строковый идентификатор роли, которую необходимо удалить.</param>
        /// <exception cref="InvalidOperationException">Вызывается при нарушении ссылочной целостности (если у роли есть зависимые клеймы или пользователи).</exception>
        Task DeleteAsync(string id);
    }
}