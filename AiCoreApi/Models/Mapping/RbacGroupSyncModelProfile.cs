using AiCoreApi.Models.DbModels;
using AiCoreApi.Models.ViewModels;
using AutoMapper;

namespace AiCoreApi.Models.Mapping
{
    public class RbacGroupSyncModelProfile : Profile
    {
        public RbacGroupSyncModelProfile()
        {
            CreateMap<RbacGroupSyncViewModel, RbacGroupSyncModel>();
            CreateMap<RbacGroupSyncModel, RbacGroupSyncViewModel>();
        }
    }
}