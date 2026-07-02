using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;

namespace OrderFlow.Application.Services
{
    public class TokenManagementService
    {
        // ИСПРАВЛЕНО: Теперь поля хранят интерфейсы, а не конкретные классы
        private readonly IAspNetUserTokenRepo _userTokenRepo;
        private readonly IAspNetUserRepo _userRepo;

        // ИСПРАВЛЕНО: В конструктор внедряются интерфейсы
        public TokenManagementService(IAspNetUserTokenRepo userTokenRepo, IAspNetUserRepo userRepo)
        {
            _userTokenRepo = userTokenRepo ?? throw new ArgumentNullException(nameof(userTokenRepo));
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
        }

        public async Task WriteTokenAsync(AspNetUserToken token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            await _userTokenRepo.CreateAsync(token);
        }

        public async Task<IEnumerable<AspNetUserToken>> GetTokensAsync(string login)
        {
            if (string.IsNullOrWhiteSpace(login)) return Enumerable.Empty<AspNetUserToken>();

            AspNetUser? user = await _userRepo.GetUserAsync(login);
            if (user == null) return Enumerable.Empty<AspNetUserToken>();

            return await _userTokenRepo.GetAllByUserIdAsync(user.Id);
        }

        public async Task RogulateTokensAsync(string login) // Из вашего исходного метода Revoke
        {
            if (string.IsNullOrWhiteSpace(login)) return;

            AspNetUser? user = await _userRepo.GetUserAsync(login);
            if (user == null) return;

            IEnumerable<AspNetUserToken> tokens = await _userTokenRepo.GetAllByUserIdAsync(user.Id);
            foreach (var token in tokens)
            {
                await _userTokenRepo.DeleteAsync(user.Id, token.LoginProvider, token.Name);
            }
        }
    }
}