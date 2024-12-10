using Microsoft.Graph.Models;

namespace AiCoreApi.Models.ViewModels
{
    public class FileViewModel
    {
        private readonly Drive _drive;
        private readonly DriveItem _item;
        private string? _uniqueId;
        public string UniqueId
        {
            get
            {
                if (_uniqueId == null)
                {
                    _uniqueId = ItemId ?? Guid.NewGuid().ToString("N");
                }

                return _uniqueId;
            }
        }
        public string? DriveId => _drive.Id;
        public string? ItemId => _item.Id;
        public string? Name => _item.Name;
        public string? Url => _item.WebUrl;
        public string? ParentPath => _item.ParentReference?.Path;
        public FileViewModel(Drive drive, DriveItem item)
        {
            _drive = drive;
            _item = item;
        }
    }
}
