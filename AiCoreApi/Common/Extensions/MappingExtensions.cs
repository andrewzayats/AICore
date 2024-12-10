using AiCoreApi.Models.DbModels;
using AiCoreApi.Models.ViewModels;

namespace AiCoreApi.Common.Extensions
{
    public static class MappingExtensions
    {
        public static List<GroupViewModel> GetViewModelGroups(List<GroupModel> dbModels)
        {
            return dbModels.Select(e => new GroupViewModel
            {
                GroupId = e.GroupId,
                Name = e.Name,
                Description = e.Description,
                Created = e.Created,
                CreatedBy = e.CreatedBy,
                Tags = new(),
                Logins = new(),
            }).ToList();
        }

        public static List<GroupModel> GetModelGroups(List<GroupViewModel> viewModels)
        {
            return viewModels.Select(e => new GroupModel
            {
                GroupId = e.GroupId,
                Name = e.Name,
                Description = e.Description,
                Created = e.Created,
                CreatedBy = e.CreatedBy,
                Tags = new(),
                Logins = new(),
            }).ToList();
        }

        public static List<TagViewModel> GetViewModelTags(List<TagModel> dbModels)
        {
            return dbModels.Select(e => new TagViewModel
            {
                TagId = e.TagId,
                Name = e.Name,
                Description = e.Description,
                Color = e.Color,
                Created = e.Created,
                CreatedBy = e.CreatedBy,
                Groups = new (),
                Logins = new(),
                Ingestions = new(),
            }).ToList();
        }

        public static List<TagModel> GetModelTags(List<TagViewModel> viewModels)
        {
            return viewModels.Select(e => new TagModel
            {
                TagId = e.TagId,
                Name = e.Name,
                Description = e.Description,
                Color = e.Color,
                Created = e.Created,
                CreatedBy = e.CreatedBy,
                Groups = new(),
                Logins = new(),
                Ingestions = new(),
            }).ToList();
        }

        public static List<LoginViewModel> GetViewModelLogins(List<LoginModel> dbModels)
        {
            return dbModels.Select(e => new LoginViewModel
            {
                LoginId = e.LoginId,
                Login = e.Login,
                FullName = e.FullName,
                Created = e.Created,
                CreatedBy = e.CreatedBy,
                TokensLimit = e.TokensLimit,
                Role = e.Role.ToString("G"),
                LoginType = e.LoginType.ToString("G"),
                Tags = new(),
                Groups = new(),
            }).ToList();
        }

        public static List<LoginModel> GetModelLogins(List<LoginViewModel> viewModels)
        {
            return viewModels.Select(e => new LoginModel
            {
                LoginId = e.LoginId,
                Login = e.Login,
                FullName = e.FullName,
                Created = e.Created,
                CreatedBy= e.CreatedBy,
                Role = Enum.Parse<RoleEnum>(e.Role),
                LoginType = Enum.Parse<LoginTypeEnum>(e.LoginType),
                TokensLimit = e.TokensLimit,
                Tags = new(),
                Groups = new(),
            }).ToList();
        }

        public static List<LoginSummaryViewModel> GetSummaryViewModelLogins(List<LoginModel> dbModels)
        {
            return dbModels.Select(e => new LoginSummaryViewModel
            {
                LoginId = e.LoginId,
                Login = e.Login,
                FullName = e.FullName,
                Created = e.Created,
                CreatedBy = e.CreatedBy,
                TokensLimit = e.TokensLimit,
                IsEnabled = e.IsEnabled,
                Email = e.Email,
                Role = e.Role.ToString("G"),
                LoginType = e.LoginType.ToString("G"),
                Tags = new(),
                Groups = new(),
            }).ToList();
        }

        public static List<LoginModel> GetModelLogins(List<LoginSummaryViewModel> viewModels)
        {
            return viewModels.Select(e => new LoginModel
            {
                LoginId = e.LoginId,
                Login = e.Login,
                FullName = e.FullName,
                Role = Enum.Parse<RoleEnum>(e.Role),
                LoginType = Enum.Parse<LoginTypeEnum>(e.LoginType),
                Created = e.Created,
                CreatedBy = e.CreatedBy,
                TokensLimit = e.TokensLimit,
                Email = e.Email,
                IsEnabled = e.IsEnabled,
                Tags = new(),
                Groups = new(),
            }).ToList();
        }
    }
}
