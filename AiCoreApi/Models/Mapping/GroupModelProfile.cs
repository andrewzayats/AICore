using AiCoreApi.Common.Extensions;
using AiCoreApi.Models.DbModels;
using AiCoreApi.Models.ViewModels;
using AutoMapper;

namespace AiCoreApi.Models.Mapping
{
    public class GroupModelProfile : Profile
    {
        public GroupModelProfile()
        {
            CreateMap<GroupViewModel, GroupModel>()
                .ForMember(dst => dst.Logins, opt => opt.MapFrom(e => MappingExtensions.GetModelLogins(e.Logins)))
                .ForMember(dst => dst.Tags, opt => opt.MapFrom(e => MappingExtensions.GetModelTags(e.Tags)));
            CreateMap<GroupModel, GroupViewModel>()
                .ForMember(dst => dst.Logins, opt => opt.MapFrom(e => MappingExtensions.GetViewModelLogins(e.Logins)))
                .ForMember(dst => dst.Tags, opt => opt.MapFrom(e => MappingExtensions.GetViewModelTags(e.Tags)));
        }
    }
}