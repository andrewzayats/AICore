using AiCoreApi.Models.ViewModels;
using AiCoreApi.Data.Processors;
using AiCoreApi.Models.DbModels;
using AutoMapper;

namespace AiCoreApi.Services.ControllersServices;

public class GroupsService : IGroupsService
{
    private readonly IMapper _mapper;
    private readonly IGroupsProcessor _groupsProcessor;

    public GroupsService(IGroupsProcessor groupsProcessor, IMapper mapper)
    {
        _groupsProcessor = groupsProcessor;
        _mapper = mapper;
    }

    public async Task<GroupViewModel?> GetGroupById(int groupId)
    {
        var group = await _groupsProcessor.Get(groupId);
        if (group == null) return null;      
        return _mapper.Map<GroupViewModel>(group);
    }

    public async Task<GroupViewModel> AddOrUpdateGroup(GroupViewModel groupViewModel)
    {
        var groupModel = _mapper.Map<GroupModel>(groupViewModel);
        var savedModel = await _groupsProcessor.Set(groupModel);
        var result = _mapper.Map<GroupViewModel>(savedModel);
        return result;
    }

    public async Task<List<GroupViewModel>> ListGroups()
    {
        var groups = await _groupsProcessor.List();
        var groupsViewModelList = _mapper.Map<List<GroupViewModel>>(groups);
        return groupsViewModelList;
    }
}

public interface IGroupsService
{
    Task<GroupViewModel?> GetGroupById(int groupId);
    Task<GroupViewModel> AddOrUpdateGroup(GroupViewModel groupViewModel);
    Task<List<GroupViewModel>> ListGroups();
}