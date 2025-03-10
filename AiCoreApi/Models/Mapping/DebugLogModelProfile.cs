using AiCoreApi.Models.DbModels;
using AiCoreApi.Models.ViewModels;
using AutoMapper;

namespace AiCoreApi.Models.Mapping;

public class DebugLogModelProfile : Profile
{
    public DebugLogModelProfile()
    {
        CreateMap<DebugLogViewModel, DebugLogModel>();
        CreateMap<DebugLogModel, DebugLogViewModel>();

        CreateMap<DebugMessageViewModel, DebugMessage>();
        CreateMap<DebugMessage, DebugMessageViewModel>();

        CreateMap<TokensSpentViewModel, TokensSpent>();
        CreateMap<TokensSpent, TokensSpentViewModel>();

        CreateMap<DebugLogFilterViewModel, DebugLogFilterModel>();
        CreateMap<DebugLogFilterModel, DebugLogFilterViewModel>();
    }
}