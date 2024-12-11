using AiCoreApi.Common.Data;
using AiCoreApi.Models.DbModels;
using Microsoft.EntityFrameworkCore;

namespace AiCoreApi.Data.Processors
{
    public class SpentProcessor : ISpentProcessor
    {
        private readonly IDbQuery _dbQuery;
        private readonly Db _db;

        public SpentProcessor(Db db, IDbQuery dbQuery)
        {
            _dbQuery = dbQuery;
            _db = db;
        }

        public async Task<SpentModel> GetTodayByLoginId(int loginId, string modelName)
        {
            return await _db.Spent.AsNoTracking()
                .FirstOrDefaultAsync(item => item.LoginId == loginId && item.Date == DateTime.UtcNow.Date && item.ModelName == modelName) 
                   ?? new SpentModel{ModelName = modelName, LoginId = loginId, Date = DateTime.UtcNow.Date };
        }

        public Task<List<SpentModel>> ListLastMonth()
        {
            var lastMonth = DateTime.UtcNow.Date.AddDays(-30);
            return _db.Spent.AsNoTracking().Where(item => item.Date >= lastMonth).ToListAsync();
        }

        public async Task Update(SpentModel spentModel)
        {
            var existingSpent = await _db.Spent.FirstOrDefaultAsync(item => item.SpentId == spentModel.SpentId);
            if (existingSpent == null)
            {
                _db.Spent.Add(spentModel);
            }
            else
            {
                _db.Entry(existingSpent).CurrentValues.SetValues(spentModel);
                _db.Spent.Update(existingSpent);
            }
            await _db.SaveChangesAsync();
        }
    }

    public interface ISpentProcessor
    {
        Task<List<SpentModel>> ListLastMonth();
        Task<SpentModel> GetTodayByLoginId(int loginId, string modelName);
        Task Update(SpentModel spentModel);
    }
}