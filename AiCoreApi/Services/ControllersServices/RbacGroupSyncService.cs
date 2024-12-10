using AiCoreApi.Models.ViewModels;
using AiCoreApi.Data.Processors;
using AiCoreApi.Models.DbModels;
using AutoMapper;

namespace AiCoreApi.Services.ControllersServices;

public class RbacGroupSyncService : IRbacGroupSyncService
{
    private readonly IMapper _mapper;
    private readonly IRbacGroupSyncProcessor _rbacGroupSyncProcessor;

    public RbacGroupSyncService(IRbacGroupSyncProcessor rbacGroupSyncProcessor, IMapper mapper)
    {
        _rbacGroupSyncProcessor = rbacGroupSyncProcessor;
        _mapper = mapper;
    }

    public async Task<RbacGroupSyncViewModel> AddRbacGroupSync(RbacGroupSyncViewModel rbacGroupSyncViewModel)
    {
        var rbacGroupSyncModel = _mapper.Map<RbacGroupSyncModel>(rbacGroupSyncViewModel);
        var savedModel = await _rbacGroupSyncProcessor.AddAsync(rbacGroupSyncModel);
        var result = _mapper.Map<RbacGroupSyncViewModel>(savedModel);
        return result;
    }

    public async Task<List<RbacGroupSyncViewModel>> ListRbacGroupSyncs()
    {
        var rbacGroupSyncList = await _rbacGroupSyncProcessor.ListAsync();
        var rbacGroupSyncViewModelList = _mapper.Map<List<RbacGroupSyncViewModel>>(rbacGroupSyncList);
        return rbacGroupSyncViewModelList;
    }

    public async Task DeleteRbacGroupSync(int rbacGroupSyncId)
    {
        await _rbacGroupSyncProcessor.DeleteAsync(rbacGroupSyncId);
    }

    public async Task<RbacGroupSyncViewModel> UpdateRbacGroupSync(RbacGroupSyncViewModel rbacGroupSyncViewModel)
    {
        var rbacGroupSyncModel = _mapper.Map<RbacGroupSyncModel>(rbacGroupSyncViewModel);
        rbacGroupSyncModel = await _rbacGroupSyncProcessor.UpdateAsync(rbacGroupSyncModel);
        var result = _mapper.Map<RbacGroupSyncViewModel>(rbacGroupSyncModel);
        return result;
    }
}

public interface IRbacGroupSyncService
{
    Task<RbacGroupSyncViewModel> AddRbacGroupSync(RbacGroupSyncViewModel rbacGroupSyncViewModel);
    Task<List<RbacGroupSyncViewModel>> ListRbacGroupSyncs();
    Task DeleteRbacGroupSync(int rbacGroupSyncId);
    Task<RbacGroupSyncViewModel> UpdateRbacGroupSync(RbacGroupSyncViewModel rbacGroupSyncViewModel);
}