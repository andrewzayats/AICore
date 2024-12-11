using AiCoreApi.Common.Data;
using AiCoreApi.Models.DbModels;
using Microsoft.EntityFrameworkCore;

namespace AiCoreApi.Data.Processors
{
    public class SchedulerAgentTaskProcessor : ISchedulerAgentTaskProcessor
    {
        private readonly Db _db;

        public SchedulerAgentTaskProcessor(
            Db db)
        {
            _db = db;
        }

        public async Task<SchedulerAgentTaskModel?> GetNext()
        {
            return await _db.SchedulerAgentTasks
                .Where(x => x.SchedulerAgentTaskState == SchedulerAgentTaskState.New)
                .OrderBy(x => x.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<SchedulerAgentTaskModel?> GetByGuid(string schedulerAgentTaskGuid)
        {
            return await _db.SchedulerAgentTasks
                .FirstOrDefaultAsync(x => x.SchedulerAgentTaskGuid == schedulerAgentTaskGuid);
        }

        public async Task<SchedulerAgentTaskModel> Update(SchedulerAgentTaskModel schedulerAgentTaskModel)
        {
            var existingAgent = await _db.SchedulerAgentTasks
                .FirstOrDefaultAsync(item => item.SchedulerAgentTaskId == schedulerAgentTaskModel.SchedulerAgentTaskId);
            if (existingAgent == null)
                return await Add(schedulerAgentTaskModel);
            _db.Entry(existingAgent).CurrentValues.SetValues(schedulerAgentTaskModel);
            _db.SchedulerAgentTasks.Update(existingAgent);
            await _db.SaveChangesAsync();
            return existingAgent;
        }

        public async Task<SchedulerAgentTaskModel> Add(SchedulerAgentTaskModel schedulerAgentTaskModel)
        {
            var existingAgent = await _db.SchedulerAgentTasks
                .FirstOrDefaultAsync(item => item.SchedulerAgentTaskId == schedulerAgentTaskModel.SchedulerAgentTaskId);
            if (existingAgent != null)
                return existingAgent;
            _db.SchedulerAgentTasks.Add(schedulerAgentTaskModel);
            await _db.SaveChangesAsync();
            return schedulerAgentTaskModel;
        }

        public async Task RemoveExpired()
        {
            var expiredAgents = await _db.SchedulerAgentTasks
                .Where(x => x.ValidTill < DateTime.UtcNow)
                .ToListAsync();
            _db.SchedulerAgentTasks.RemoveRange(expiredAgents);
            await _db.SaveChangesAsync();
        }
    }

    public interface ISchedulerAgentTaskProcessor
    {
        Task<SchedulerAgentTaskModel?> GetNext();
        Task<SchedulerAgentTaskModel?> GetByGuid(string schedulerAgentTaskGuid);
        Task<SchedulerAgentTaskModel> Update(SchedulerAgentTaskModel schedulerAgentTaskModel);
        Task<SchedulerAgentTaskModel> Add(SchedulerAgentTaskModel schedulerAgentTaskModel);
        Task RemoveExpired();
    }
}

