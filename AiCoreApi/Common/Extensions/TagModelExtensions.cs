using AiCoreApi.Models.DbModels;

namespace AiCoreApi.Common.Extensions;

internal static class TagModelExtensions
{
    public static Dictionary<string, List<string>> ToTagDictionary(this List<TagModel> tags) => new()
    {
        { AiCoreConstants.TagName, tags.Select(t => t.Name).ToList() }
    };
}