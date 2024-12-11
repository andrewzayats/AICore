using AiCoreApi.Models.DbModels;
using AiCoreApi.Models.ViewModels;
using AutoMapper;

namespace AiCoreApi.Models.Mapping
{
    public class TagModelProfile : Profile
    {
        public TagModelProfile()
        {
            CreateMap<TagViewModel, TagModel>();
            CreateMap<TagModel, TagViewModel>();
        }
    }
}