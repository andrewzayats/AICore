using AiCoreApi.Models.DbModels;
using AiCoreApi.Models.ViewModels;
using AutoMapper;

namespace AiCoreApi.Models.Mapping
{
    public class IngestionModelProfile : Profile
    {
        public IngestionModelProfile()
        {
            CreateMap<IngestionViewModel, IngestionModel>();
            CreateMap<IngestionModel, IngestionViewModel>();

            CreateMap<TaskModel, IngestionTaskViewModel>()
                .ForMember(dst => dst.IngestionName,
                    opt => opt.MapFrom(e => e.Ingestion.Name));
        }
    }
}