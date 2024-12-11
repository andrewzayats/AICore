using AiCoreApi.Common;
using AiCoreApi.Common.Data;
using AiCoreApi.Models.DbModels;
using Microsoft.EntityFrameworkCore;

namespace AiCoreApi.Data.Processors;

public class GroupsProcessor : IGroupsProcessor
{
    private readonly Db _db;
    private readonly IDbQuery _dbQuery;
    private readonly ExtendedConfig _config;

    public GroupsProcessor(Db db, IDbQuery dbQuery, ExtendedConfig config)
    {
        _db = db;
        _dbQuery = dbQuery;
        _config = config;
    }

    public async Task<GroupModel?> Get(int groupId)
    {
        var data = await _db.Groups
            .Include(e => e.Tags)
            .Include(e => e.Logins)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.GroupId == groupId);
        return data;
    }

    public async Task<GroupModel?> Get(string groupName)
    {
        var data = await _db.Groups
            .Include(e => e.Tags)
            .Include(e => e.Logins)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Name == groupName);
        return data;
    }

    public async Task<GroupModel> Set(GroupModel groupModel)
    {
        GroupModel? group;

        var gId = groupModel.GroupId;
        var tIds = groupModel.Tags.Select(e => e.TagId);
        var uIds = groupModel.Logins.Select(e => e.LoginId);

        var tags = tIds.Any() 
            ? await _db.Tags.Where(e => tIds.Contains(e.TagId)).ToListAsync()
            : new List<TagModel>();

        var users = uIds.Any()
            ? await _db.Login.Where(e => uIds.Contains(e.LoginId)).ToListAsync()
            : new List<LoginModel>();
        
        if (gId == 0)
        {
            group = new GroupModel
            {
                Name = groupModel.Name,
                Description = groupModel.Description,
                Created = groupModel.Created,
                CreatedBy = groupModel.CreatedBy,
                Tags = tags,
                Logins = users
            };

            await _db.Groups.AddAsync(group);
        }
        else
        {
            group = await _db.Groups
                .Include(e => e.Tags)
                .Include(e => e.Logins)
                .FirstAsync(e => e.GroupId == gId);

            group.Name = groupModel.Name;
            group.Description = groupModel.Description;
            group.Created = groupModel.Created;
            group.CreatedBy = groupModel.CreatedBy;
            group.Tags = tags;
            group.Logins = users;

            _db.Groups.Update(group);
        }

        await _db.SaveChangesAsync();
        return group;
    }

    public async Task<List<GroupModel>> List()
    {
        var data = await _db.Groups.Include(e => e.Tags).AsNoTracking().ToListAsync();
        return data;
    }
}

public interface IGroupsProcessor
{
    Task<GroupModel?> Get(int groupId);
    Task<GroupModel?> Get(string groupName);
    Task<GroupModel> Set(GroupModel groupModel);
    Task<List<GroupModel>> List();
}