using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Odyssey.Shared.Helpers
{
    internal class FileIconHelper
    {
        public async static Task<StorageItemThumbnail> GetFileIcon(StorageFile file, uint size = 32)
        {
            StorageItemThumbnail iconTmb;
            var imgExt = new[] { "bmp", "gif", "jpeg", "jpg", "png" }.FirstOrDefault(ext => file.Path.ToLower().EndsWith(ext));
            if (imgExt != null)
            {
                var dummy = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("dummy." + imgExt, CreationCollisionOption.GenerateUniqueName);
                iconTmb = await dummy.GetThumbnailAsync(ThumbnailMode.SingleItem, size);
            }
            else
            {
                iconTmb = await file.GetThumbnailAsync(ThumbnailMode.SingleItem, size);
            }
            return iconTmb;
        }
    }
}
