using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Odyssey.Migration.Chromium
{
    public class Extensions
    {
        public static async void Apply(string path)
        {
            await Task.Factory.StartNew(async () =>
            {
                string extensionsPath = Path.Combine(path, "Default", "Extensions");
                var odysseyExtensionFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Extensions", CreationCollisionOption.OpenIfExists);

                if (!Directory.Exists(extensionsPath)) return;

                foreach (string dirPath in Directory.GetDirectories(extensionsPath, "*", SearchOption.AllDirectories))
                {
                    FileInfo file = new(dirPath);
                    if (!Directory.Exists(Path.Combine(odysseyExtensionFolder.Path, file.Name)))
                    {
                        Directory.CreateDirectory(dirPath.Replace(extensionsPath, odysseyExtensionFolder.Path));
                    }
                }

                //Copy all the files & Replaces any files with the same name
                foreach (string newPath in Directory.GetFiles(extensionsPath, "*", SearchOption.AllDirectories))
                {
                    File.Copy(newPath, newPath.Replace(extensionsPath, odysseyExtensionFolder.Path), true);
                }
            });

        }
    }
}
