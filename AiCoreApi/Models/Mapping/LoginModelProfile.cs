using AiCoreApi.Common.Extensions;
using AiCoreApi.Models.DbModels;
using AiCoreApi.Models.ViewModels;
using AutoMapper;

namespace AiCoreApi.Models.Mapping;

public class LoginModelProfile : Profile
{
    public LoginModelProfile()
    {
        CreateMap<LoginViewModel, LoginModel>()
            .ForMember(
                dst => dst.Role,
                opt => opt.MapFrom(e => e.Role))
            .ForMember(
                dst => dst.LoginType,
                opt => opt.MapFrom(e => e.LoginType));

        CreateMap<LoginModel, LoginViewModel>()
            .ForMember(
                dst => dst.Role,
                opt => opt.MapFrom(e => e.Role))
            .ForMember(
                dst => dst.LoginType,
                opt => opt.MapFrom(e => e.LoginType));

        CreateMap<LoginSummaryViewModel, LoginModel>()
            .ForMember(
                dst => dst.Role,
                opt => opt.MapFrom(e => e.Role))
            .ForMember(
                dst => dst.LoginType,
                opt => opt.MapFrom(e => e.LoginType))
            .ForMember(
                dst => dst.PasswordHash,
                opt => opt.MapFrom(e => e.Password.GetHash()))
            .ForMember(dst => dst.Tags, opt => opt.MapFrom(e => MappingExtensions.GetModelTags(e.Tags)))
            .ForMember(dst => dst.Groups, opt => opt.MapFrom(e => MappingExtensions.GetModelGroups(e.Groups)));

        CreateMap<LoginModel, LoginSummaryViewModel>()
            .ForMember(
                dst => dst.Role,
                opt => opt.MapFrom(e => e.Role))
            .ForMember(
                dst => dst.LoginType,
                opt => opt.MapFrom(e => e.LoginType))
            .ForMember(dst => dst.Tags, opt => opt.MapFrom(e => MappingExtensions.GetViewModelTags(e.Tags)))
            .ForMember(dst => dst.Groups, opt => opt.MapFrom(e => MappingExtensions.GetViewModelGroups(e.Groups)));

        CreateMap<EditLoginViewModel, LoginModel>()
         .ForMember(
             dst => dst.Role,
             opt => opt.MapFrom(e => e.Role))
         .ForMember(dst => dst.Tags, opt => opt.MapFrom(e => MappingExtensions.GetModelTags(e.Tags)))
            .ForMember(dst => dst.Groups, opt => opt.MapFrom(e => MappingExtensions.GetModelGroups(e.Groups)));

        CreateMap<LoginModel, EditLoginViewModel>()
            .ForMember(
                dst => dst.Role,
                opt => opt.MapFrom(e => e.Role))
            .ForMember(dst => dst.Tags, opt => opt.MapFrom(e => MappingExtensions.GetViewModelTags(e.Tags)))
            .ForMember(dst => dst.Groups, opt => opt.MapFrom(e => MappingExtensions.GetViewModelGroups(e.Groups)));

        CreateMap<LoginSummaryViewModel, LoginWithSpentModel>()
            .ForMember(
                dst => dst.Role,
                opt => opt.MapFrom(e => e.Role))
            .ForMember(
                dst => dst.LoginType,
                opt => opt.MapFrom(e => e.LoginType))
            .ForMember(dst => dst.Tags, opt => opt.MapFrom(e => MappingExtensions.GetModelTags(e.Tags)))
            .ForMember(dst => dst.Groups, opt => opt.MapFrom(e => MappingExtensions.GetModelGroups(e.Groups)));

        CreateMap<LoginWithSpentModel, LoginSummaryViewModel>()
            .ForMember(
                dst => dst.Role,
                opt => opt.MapFrom(e => e.Role))
            .ForMember(
                dst => dst.LoginType,
                opt => opt.MapFrom(e => e.LoginType))
            .ForMember(dst => dst.Tags, opt => opt.MapFrom(e => MappingExtensions.GetViewModelTags(e.Tags)))
            .ForMember(dst => dst.Groups, opt => opt.MapFrom(e => MappingExtensions.GetViewModelGroups(e.Groups)));
    }
}