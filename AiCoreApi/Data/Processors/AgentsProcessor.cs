using AiCoreApi.Common.Data;
using AiCoreApi.Models.DbModels;
using Microsoft.EntityFrameworkCore;

namespace AiCoreApi.Data.Processors;

public class AgentsProcessor : IAgentsProcessor
{
    private readonly Db _db;

    public AgentsProcessor(
        Db db)
    {
        _db = db;
    }

    public async Task<List<AgentModel>> List()
    {
        var data = await _db.Agents.Include(e => e.Tags).AsNoTracking().ToListAsync();
        return data;
    }

    public async Task<AgentModel?> GetById(int agentId)
    {
        return await _db.Agents.Include(e => e.Tags).AsNoTracking().FirstOrDefaultAsync(e => e.AgentId == agentId);
    }

    public async Task<AgentModel?> GetByName(string agentName)
    {
        return await _db.Agents.Include(e => e.Tags).AsNoTracking().FirstOrDefaultAsync(e => e.Name == agentName);
    }

    public async Task<AgentModel?> Update(AgentModel agentModel)
    {
        var existingAgent = await _db.Agents
            .Include(e => e.Tags)
            .FirstOrDefaultAsync(item => item.AgentId == agentModel.AgentId);
        if (existingAgent == null)
            return await Add(agentModel);
        var tIds = agentModel.Tags.Select(e => e.TagId);
        var tags = tIds.Any()
            ? await _db.Tags.Where(e => tIds.Contains(e.TagId)).ToListAsync()
            : new List<TagModel>();
        _db.Entry(existingAgent).CurrentValues.SetValues(agentModel);
        existingAgent.Tags = tags;
        _db.Agents.Update(existingAgent);
        await _db.SaveChangesAsync();
        return existingAgent;
    }

    public async Task<AgentModel> Add(AgentModel agentModel)
    {
        var existingAgent = await _db.Agents
            .Include(e => e.Tags)
            .FirstOrDefaultAsync(item => item.AgentId == agentModel.AgentId);
        if (existingAgent != null) return existingAgent;

        if (agentModel.Tags is { Count: > 0 })
        {
            var tags = _db.Tags.ToList();
            var tIds = agentModel.Tags.Select(e => e.TagId);
            agentModel.Tags = tags.Where(e => tIds.Contains(e.TagId)).ToList();
        }
        await _db.Agents.AddAsync(agentModel);
        await _db.SaveChangesAsync();
        return agentModel;
    }

    public async Task<AgentModel> Set(AgentModel agentModel)
    {
        if (agentModel.AgentId == 0)
        {
            await _db.Agents.AddAsync(agentModel);
        }
        else
        {
            _db.Agents.Update(agentModel);
        }

        await _db.SaveChangesAsync();
        return agentModel;
    }

    public async Task Delete(int agentId)
    {
        var agent = await _db.Agents
            .Include(e => e.Tags)
            .FirstOrDefaultAsync(item => item.AgentId == agentId);
        if (agent == null)
            return;
        _db.Agents.Remove(agent);
        await _db.SaveChangesAsync();
    }
}

public interface IAgentsProcessor
{
    Task<List<AgentModel>> List();
    Task<AgentModel?> GetById(int agentId);
    Task<AgentModel?> GetByName(string agentName);
    Task<AgentModel> Add(AgentModel agentModel);
    Task<AgentModel?> Update(AgentModel agentModel);
    Task Delete(int agentId);
}