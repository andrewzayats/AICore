using AiCoreApi.Common;
using AiCoreApi.Common.Data;
using AiCoreApi.Models.DbModels;
using Microsoft.EntityFrameworkCore;

namespace AiCoreApi.Data.Processors;

public class TagsProcessor : ITagsProcessor
{
    private readonly Db _db;
    private readonly IDbQuery _dbQuery;
    private readonly ExtendedConfig _config;

    public TagsProcessor(Db db, IDbQuery dbQuery, ExtendedConfig config)
    {
        _db = db;
        _dbQuery = dbQuery;
        _config = config;
    }

    public TagModel? Get(int tagId)
    {
        return _db.Tags.AsNoTracking().FirstOrDefault(item => item.TagId == tagId);
    }

    public async Task<TagModel?> Set(TagModel tagModel)
    {
        TagModel? tag;
        if (tagModel.TagId == 0)
        {
            tag = new TagModel
            {
                TagId = tagModel.TagId,
                Name = tagModel.Name,
                Description = tagModel.Description,
                Created = tagModel.Created,
                CreatedBy = tagModel.CreatedBy,
                Color = tagModel.Color
            };
            await _db.Tags.AddAsync(tag);
        }
        else
        {
            tag = _db.Tags.FirstOrDefault(item => item.TagId == tagModel.TagId);

            if (tag == null) return null;

            tag.TagId = tagModel.TagId;
            tag.Name = tagModel.Name;
            tag.Description = tagModel.Description;
            tag.Color = tagModel.Color;
            _db.Tags.Update(tag);
        }
        await _db.SaveChangesAsync();
        return tag;
    }

    public List<TagModel> List()
    {
        return _db.Tags.
            Select(item => new TagModel
            {
                TagId = item.TagId,
                Name = item.Name,
                Description = item.Description,
                Created = item.Created,
                CreatedBy = item.CreatedBy,
                Color = item.Color,
            })
            .AsNoTracking().ToList();
    }
}

public interface ITagsProcessor
{
    TagModel? Get(int tagId);
    Task<TagModel?> Set(TagModel tagModel);
    List<TagModel> List();
}