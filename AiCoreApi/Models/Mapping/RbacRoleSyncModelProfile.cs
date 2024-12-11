using AiCoreApi.Models.DbModels;
using AiCoreApi.Models.ViewModels;
using AutoMapper;

namespace AiCoreApi.Models.Mapping
{
    public class RbacRoleSyncModelProfile : Profile
    {
        public RbacRoleSyncModelProfile()
        {
            CreateMap<RbacRoleSyncViewModel, RbacRoleSyncModel>();
            CreateMap<RbacRoleSyncModel, RbacRoleSyncViewModel>();
        }
    }
}