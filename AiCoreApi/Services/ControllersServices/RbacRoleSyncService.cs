using AiCoreApi.Models.ViewModels;
using AiCoreApi.Data.Processors;
using AiCoreApi.Models.DbModels;
using AutoMapper;

namespace AiCoreApi.Services.ControllersServices;

public class RbacRoleSyncService : IRbacRoleSyncService
{
    private readonly IMapper _mapper;
    private readonly IRbacRoleSyncProcessor _rbacRoleSyncProcessor;

    public RbacRoleSyncService(IRbacRoleSyncProcessor rbacRoleSyncProcessor, IMapper mapper)
    {
        _rbacRoleSyncProcessor = rbacRoleSyncProcessor;
        _mapper = mapper;
    }

    public async Task<RbacRoleSyncViewModel> AddRbacRoleSync(RbacRoleSyncViewModel rbacRoleSyncViewModel)
    {
        var rbacRoleSyncModel = _mapper.Map<RbacRoleSyncModel>(rbacRoleSyncViewModel);
        var savedModel = await _rbacRoleSyncProcessor.AddAsync(rbacRoleSyncModel);
        var result = _mapper.Map<RbacRoleSyncViewModel>(savedModel);
        return result;
    }

    public async Task<List<RbacRoleSyncViewModel>> ListRbacRoleSyncs()
    {
        var rbacRoleSyncList = await _rbacRoleSyncProcessor.ListAsync();
        var rbacRoleSyncViewModelList = _mapper.Map<List<RbacRoleSyncViewModel>>(rbacRoleSyncList);
        return rbacRoleSyncViewModelList;
    }

    public async Task DeleteRbacRoleSync(int rbacRoleSyncId)
    {
        await _rbacRoleSyncProcessor.DeleteAsync(rbacRoleSyncId);
    }

    public async Task<RbacRoleSyncViewModel> UpdateRbacRoleSync(RbacRoleSyncViewModel rbacRoleSyncViewModel)
    {
        var rbacRoleSyncModel = _mapper.Map<RbacRoleSyncModel>(rbacRoleSyncViewModel);
        rbacRoleSyncModel = await _rbacRoleSyncProcessor.UpdateAsync(rbacRoleSyncModel);
        var result = _mapper.Map<RbacRoleSyncViewModel>(rbacRoleSyncModel);
        return result;
    }
}

public interface IRbacRoleSyncService
{
    Task<RbacRoleSyncViewModel> AddRbacRoleSync(RbacRoleSyncViewModel rbacRoleSyncViewModel);
    Task<List<RbacRoleSyncViewModel>> ListRbacRoleSyncs();
    Task DeleteRbacRoleSync(int rbacRoleSyncId);
    Task<RbacRoleSyncViewModel> UpdateRbacRoleSync(RbacRoleSyncViewModel rbacRoleSyncViewModel);
}