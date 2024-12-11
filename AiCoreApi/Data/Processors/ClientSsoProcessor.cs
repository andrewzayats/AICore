using AiCoreApi.Common.Data;
using AiCoreApi.Models.DbModels;
using Microsoft.EntityFrameworkCore;

namespace AiCoreApi.Data.Processors;

public class ClientSsoProcessor : IClientSsoProcessor
{
    private readonly IDbQuery _dbQuery;
    private readonly Db _db;

    public ClientSsoProcessor(Db db, IDbQuery dbQuery)
    {
        _dbQuery = dbQuery;
        _db = db;
    }

    public async Task<ClientSsoModel?> Get(int clientSsoId)
    {
        return await _db.ClientSso.Include(e => e.Groups).AsNoTracking().FirstOrDefaultAsync(e => e.ClientSsoId == clientSsoId);
    }

    public async Task<List<ClientSsoModel>> List()
    {
        return await _db.ClientSso.Include(e => e.Groups).AsNoTracking().ToListAsync();
    }

    public async Task Delete(int clientSsoId)
    {
        var clientSso = await _db.ClientSso.Include(e => e.Groups).FirstOrDefaultAsync(item => item.ClientSsoId == clientSsoId);
        if (clientSso == null) return;
        _db.ClientSso.Remove(clientSso);
        await _db.SaveChangesAsync();
    }

    public async Task<ClientSsoModel> Add(ClientSsoModel clientSsoModel)
    {
        if (clientSsoModel.ClientSsoId != 0) throw new ArgumentException("Sso Client identifier must be zero");

        if (clientSsoModel.Groups != null && clientSsoModel.Groups.Count > 0)
        {
            var gIds = clientSsoModel.Groups.Select(e => e.GroupId);
            var groups = gIds.Any()
            ? await _db.Groups.Where(e => gIds.Contains(e.GroupId)).ToListAsync()
            : new List<GroupModel>();

            clientSsoModel.Groups = groups.ToList();
        }

        await _db.ClientSso.AddAsync(clientSsoModel);
        await _db.SaveChangesAsync();

        return clientSsoModel;
    }

    public async Task<ClientSsoModel> Update(ClientSsoModel clientSsoModel)
    {
        if (clientSsoModel.ClientSsoId == 0) throw new ArgumentException("Sso Client identifier mustn't be zero");

        var existingClientSso = await _db.ClientSso.Include(e => e.Groups).FirstAsync(item => item.ClientSsoId == clientSsoModel.ClientSsoId);

        var gIds = clientSsoModel.Groups.Select(e => e.GroupId);

        var groups = gIds.Any()
            ? await _db.Groups.Where(e => gIds.Contains(e.GroupId)).ToListAsync()
            : [];

        _db.Entry(existingClientSso).CurrentValues.SetValues(clientSsoModel);

        existingClientSso.Groups = groups;

        _db.ClientSso.Update(existingClientSso);
        await _db.SaveChangesAsync();
        return existingClientSso;
    }
}

public interface IClientSsoProcessor
{
    Task<ClientSsoModel?> Get(int clientSsoId);
    Task<List<ClientSsoModel>> List();
    Task Delete(int clientSsoId);
    Task<ClientSsoModel> Add(ClientSsoModel clientSsoModel);
    Task<ClientSsoModel> Update(ClientSsoModel clientSsoModel);
}