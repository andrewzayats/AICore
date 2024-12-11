using AiCoreApi.Common.Data;
using AiCoreApi.Models.DbModels;
using Microsoft.EntityFrameworkCore;

namespace AiCoreApi.Data.Processors
{
    public class RbacRoleSyncProcessor : IRbacRoleSyncProcessor
    {
        private readonly IDbQuery _dbQuery;
        private readonly Db _db;

        public RbacRoleSyncProcessor(Db db, IDbQuery dbQuery)
        {
            _dbQuery = dbQuery;
            _db = db;
        }
        
        public async Task<List<RbacRoleSyncModel>> ListAsync()
        {
            return await _db.RbacRoleSync.Include(e => e.Tags).AsNoTracking().ToListAsync();
        }

        public async Task DeleteAsync(int rbacRoleSyncId)
        {
            var rbacRoleSync = await _db.RbacRoleSync.Include(e => e.Tags)
                .FirstOrDefaultAsync(item => item.RbacRoleSyncId == rbacRoleSyncId);
            if (rbacRoleSync == null)
                return;
            _db.RbacRoleSync.Remove(rbacRoleSync);
            await _db.SaveChangesAsync();
        }

        public async Task<RbacRoleSyncModel> AddAsync(RbacRoleSyncModel rbacRoleSyncModel)
        {
            var existingRbacRoleSync = await _db.RbacRoleSync.Include(e => e.Tags)
                .FirstOrDefaultAsync(item => item.RbacRoleName == rbacRoleSyncModel.RbacRoleName);
            if (existingRbacRoleSync != null)
                return existingRbacRoleSync;

            if (rbacRoleSyncModel.Tags != null && rbacRoleSyncModel.Tags.Count > 0)
            {
                var tags = _db.Tags.ToList();
                var tIds = rbacRoleSyncModel.Tags.Select(e => e.TagId);

                rbacRoleSyncModel.Tags = tags.Where(e => tIds.Contains(e.TagId)).ToList();
            }

            _db.RbacRoleSync.Add(rbacRoleSyncModel);
            await _db.SaveChangesAsync();
            return rbacRoleSyncModel;
        }

        public async Task<RbacRoleSyncModel> UpdateAsync(RbacRoleSyncModel rbacRoleSyncModel)
        {
            var existingRbacRoleSync = await _db.RbacRoleSync.Include(e => e.Tags)
                .FirstOrDefaultAsync(item => item.RbacRoleSyncId == rbacRoleSyncModel.RbacRoleSyncId);
            if (existingRbacRoleSync == null)
                return null;
            var createdBy = existingRbacRoleSync.CreatedBy;
            _db.Entry(existingRbacRoleSync).CurrentValues.SetValues(rbacRoleSyncModel);
            existingRbacRoleSync.CreatedBy = createdBy;
            if (rbacRoleSyncModel.Tags != null && rbacRoleSyncModel.Tags.Count > 0)
            {
                var tags = _db.Tags.ToList();
                var tIds = rbacRoleSyncModel.Tags.Select(e => e.TagId);

                existingRbacRoleSync.Tags = tags.Where(e => tIds.Contains(e.TagId)).ToList();
            }
            _db.RbacRoleSync.Update(existingRbacRoleSync);
            await _db.SaveChangesAsync();
            return rbacRoleSyncModel;
        }
    }

    public interface IRbacRoleSyncProcessor
    {
        Task<List<RbacRoleSyncModel>> ListAsync();
        Task DeleteAsync(int rbacRoleSyncId);
        Task<RbacRoleSyncModel> AddAsync(RbacRoleSyncModel rbacRoleSyncModel);
        Task<RbacRoleSyncModel> UpdateAsync(RbacRoleSyncModel rbacRoleSyncModel);
    }
}