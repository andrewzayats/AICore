using AiCoreApi.Common.Extensions;
using AiCoreApi.Models.DbModels;
using AiCoreApi.Models.ViewModels;
using AutoMapper;

namespace AiCoreApi.Models.Mapping
{
    public class ClientSsoModelProfile : Profile
    {
        public ClientSsoModelProfile()
        {
            CreateMap<ClientSsoViewModel, ClientSsoModel>()
                .ForMember(
                    dst => dst.ClientSsoId,
                    opt => opt.MapFrom(e => e.ClientSsoId))
                .ForMember(dst => dst.Groups, opt => opt.MapFrom(e => MappingExtensions.GetModelGroups(e.Groups)));

            CreateMap<ClientSsoModel, ClientSsoViewModel>()
                .ForMember(
                    dst => dst.ClientSsoId,
                    opt => opt.MapFrom(e => e.ClientSsoId))
                .ForMember(dst => dst.Groups, opt => opt.MapFrom(e => MappingExtensions.GetViewModelGroups(e.Groups)));
        }
    }
}