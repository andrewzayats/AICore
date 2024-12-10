using AiCoreApi.Common.Data;
using AiCoreApi.Models.DbModels;
using Microsoft.EntityFrameworkCore;

namespace AiCoreApi.Data.Processors
{
    public class RbacGroupSyncProcessor : IRbacGroupSyncProcessor
    {
        private readonly IDbQuery _dbQuery;
        private readonly Db _db;

        public RbacGroupSyncProcessor(Db db, IDbQuery dbQuery)
        {
            _dbQuery = dbQuery;
            _db = db;
        }
        
        public async Task<List<RbacGroupSyncModel>> ListAsync()
        {
            return await _db.RbacGroupSync.AsNoTracking().ToListAsync();
        }

        public async Task DeleteAsync(int rbacGroupSyncId)
        {
            var rbacGroupSync = await _db.RbacGroupSync
                .FirstOrDefaultAsync(item => item.RbacGroupSyncId == rbacGroupSyncId);
            if (rbacGroupSync == null)
                return;
            _db.RbacGroupSync.Remove(rbacGroupSync);
            await _db.SaveChangesAsync();
        }

        public async Task<RbacGroupSyncModel> AddAsync(RbacGroupSyncModel rbacGroupSyncModel)
        {
            var existingRbacGroupSync = await _db.RbacGroupSync
                .FirstOrDefaultAsync(item => item.RbacGroupName == rbacGroupSyncModel.RbacGroupName);
            if (existingRbacGroupSync != null)
                return existingRbacGroupSync;

            _db.RbacGroupSync.Add(rbacGroupSyncModel);
            await _db.SaveChangesAsync();
            return rbacGroupSyncModel;
        }

        public async Task<RbacGroupSyncModel> UpdateAsync(RbacGroupSyncModel rbacGroupSyncModel)
        {
            var existingRbacGroupSync = await _db.RbacGroupSync
                .FirstOrDefaultAsync(item => item.RbacGroupSyncId == rbacGroupSyncModel.RbacGroupSyncId);
            if (existingRbacGroupSync == null)
                return null;
            var createdBy = existingRbacGroupSync.CreatedBy;
            _db.Entry(existingRbacGroupSync).CurrentValues.SetValues(rbacGroupSyncModel);
            existingRbacGroupSync.CreatedBy = createdBy;
            _db.RbacGroupSync.Update(existingRbacGroupSync);
            await _db.SaveChangesAsync();
            return rbacGroupSyncModel;
        }
    }

    public interface IRbacGroupSyncProcessor
    {
        Task<List<RbacGroupSyncModel>> ListAsync();
        Task DeleteAsync(int rbacGroupSyncId);
        Task<RbacGroupSyncModel> AddAsync(RbacGroupSyncModel rbacGroupSyncModel);
        Task<RbacGroupSyncModel> UpdateAsync(RbacGroupSyncModel rbacGroupSyncModel);
    }
}