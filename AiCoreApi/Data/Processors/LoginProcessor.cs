using AiCoreApi.Common.Data;
using AiCoreApi.Common.Extensions;
using AiCoreApi.Models.DbModels;
using Microsoft.EntityFrameworkCore;

namespace AiCoreApi.Data.Processors
{
    public class LoginProcessor : ILoginProcessor
    {
        private readonly IDbQuery _dbQuery;
        private readonly Db _db;

        public LoginProcessor(Db db, IDbQuery dbQuery)
        {
            _dbQuery = dbQuery;
            _db = db;
        }

        public async Task<LoginModel?> GetByCredentials(string login, string password)
        {
            var loginModel = await _db.Login.Include(e => e.Tags).AsNoTracking()
                .FirstOrDefaultAsync(item => item.Login == login && item.LoginType == LoginTypeEnum.Password);
            return loginModel == null || !loginModel.IsEnabled || loginModel.PasswordHash != password.GetHash()
                ? null
                : loginModel;
        }

        public async Task<List<LoginModel>> List()
        {
            return await _db.Login.Include(e => e.Tags).AsNoTracking().ToListAsync();
        }

        public async Task<List<LoginWithSpentModel>> ListWithSpent()
        {
            var loginModelExtended = await _db.Login
                .Include(e => e.Tags)
                .GroupJoin(
                    _db.Spent
                        .Where(x => x.Date == DateTime.UtcNow.Date)
                        .GroupBy(x => x.LoginId)
                        .Select(group => new
                        {
                            LoginId = (int?)group.Key,
                            TokensIncoming = (int?)group.Sum(x => x.TokensIncoming), 
                            TokensOutgoing = (int?)group.Sum(x => x.TokensOutgoing)
                        }),
                    login => login.LoginId,
                    spent => spent.LoginId,
                    (login, spent) => new { login, spent }
                )
                .SelectMany(
                    x => x.spent.DefaultIfEmpty(),
                    (x, spent) => new LoginWithSpentModel(x.login)
                    {
                        TokensSpent = spent != null 
                            ? spent.TokensIncoming + spent.TokensOutgoing 
                            : 0
                    }
                )
                .ToListAsync();
            loginModelExtended = loginModelExtended.OrderBy(item => item.LoginId).ToList();
            return loginModelExtended;
        }

        public async Task<LoginModel?> GetById(int id)
        {
            var login = await _db.Login.AsNoTracking()
                .Include(e => e.Tags)
                .Include(e => e.Groups).AsNoTracking()
                .FirstOrDefaultAsync(item => item.LoginId == id);
            return login;
        }

        public async Task<LoginModel?> GetByLogin(string login, LoginTypeEnum loginType)
        {
            return await _db.Login.AsNoTracking()
                .Include(e => e.Tags).AsNoTracking()
                .Include(e => e.Groups).AsNoTracking()
                .FirstOrDefaultAsync(item => item.Login == login && item.LoginType == loginType);
        }

        public async Task<List<TagModel>> GetTagsByLogin(string login, LoginTypeEnum loginType)
        {
            var loginModel = await _db.Login.AsNoTracking()
                .Include(e => e.Tags)
                .Include(e => e.Groups).ThenInclude(e => e.Tags)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Login == login && e.LoginType == loginType);
                
            var tags = new List<TagModel>(loginModel.Tags)
                .Union(loginModel.Groups.SelectMany(e => e.Tags))
                .DistinctBy(e => e.TagId)
                .Select(e => new TagModel
                {
                    TagId = e.TagId,
                    Name = e.Name,
                    Description = e.Description,
                    Created = e.Created,
                    CreatedBy = e.CreatedBy,
                    Color = e.Color
                })
                .ToList();

            return tags;
        }

        public async Task Update(LoginModel loginModel)
        {
            var existingLogin = await _db.Login
                .Include(e => e.Tags)
                .Include(e => e.Groups)
                .FirstOrDefaultAsync(item => item.Login == loginModel.Login && loginModel.LoginType == item.LoginType);

            if (existingLogin == null)
                return;

            var tIds = loginModel.Tags.Select(e => e.TagId);
            var gIds = loginModel.Groups.Select(e => e.GroupId);

            var tags = tIds.Any()
                ? await _db.Tags.Where(e => tIds.Contains(e.TagId)).ToListAsync()
                : new List<TagModel>();

            var groups = gIds.Any()
                ? await _db.Groups.Where(e => gIds.Contains(e.GroupId)).ToListAsync()
            : new List<GroupModel>();

            _db.Entry(existingLogin).CurrentValues.SetValues(loginModel);

            existingLogin.Tags = tags;
            existingLogin.Groups = groups;

            _db.Login.Update(existingLogin);
            await _db.SaveChangesAsync();
        }

        public async Task<LoginModel> Add(LoginModel loginModel)
        {
            var existingLogin = await _db.Login
                .Include(e => e.Tags)
                .Include(e => e.Groups)
                .FirstOrDefaultAsync(item => item.Login == loginModel.Login && loginModel.LoginType == item.LoginType);

            if (existingLogin != null) return existingLogin;

            if(loginModel.Tags != null && loginModel.Tags.Count > 0)
            {
                var tags = _db.Tags.ToList();
                var tIds = loginModel.Tags.Select(e => e.TagId);

                loginModel.Tags = tags.Where(e => tIds.Contains(e.TagId)).ToList();
            }

            if (loginModel.Groups != null && loginModel.Groups.Count > 0)
            {
                var groups = _db.Groups.ToList();
                var gIds = loginModel.Groups.Select(e => e.GroupId);

                loginModel.Groups = groups.Where(e => gIds.Contains(e.GroupId)).ToList();
            }

            await _db.Login.AddAsync(loginModel);
            await _db.SaveChangesAsync();
            return loginModel;
        }
    }

    public interface ILoginProcessor
    {
        Task<LoginModel?> GetByCredentials(string login, string password);
        Task<List<LoginModel>> List();
        Task<List<LoginWithSpentModel>> ListWithSpent();
        Task<LoginModel?> GetById(int id);
        Task<LoginModel?> GetByLogin(string login, LoginTypeEnum loginType);
        Task<List<TagModel>> GetTagsByLogin(string login, LoginTypeEnum loginType);
        Task Update(LoginModel login);
        Task<LoginModel> Add(LoginModel login);
    }
}