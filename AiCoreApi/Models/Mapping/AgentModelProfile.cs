using AiCoreApi.Models.DbModels;
using AiCoreApi.Models.ViewModels;
using AutoMapper;

namespace AiCoreApi.Models.Mapping;

public class AgentModelProfile : Profile
{
    public AgentModelProfile()
    {
        CreateMap<AgentViewModel, AgentModel>();
        CreateMap<AgentModel, AgentViewModel>();

        CreateMap<ConfigurableSetting, ConfigurableSettingView>();
        CreateMap<ConfigurableSettingView, ConfigurableSetting>();

        CreateMap<AgentExportModel, AgentModel>();
        CreateMap<AgentModel, AgentExportModel>();

        CreateMap<ConfigurableSetting, ConfigurableExportSetting>();
        CreateMap<ConfigurableExportSetting, ConfigurableSetting>();
    }
}