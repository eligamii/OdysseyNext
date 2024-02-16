using AriaSharp;
using Downloader;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using Odyssey.FWebView.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Odyssey.FWebView
{
    public class BrowserExtensionInfo
    {
        
        public static BrowserExtensionInfo GetFromExtensionFolder(string extensionFolder)
        {
            var res = JsonConvert.DeserializeObject<ExtensionManifestDeserializer>(System.IO.Path.Combine(extensionFolder, "manifest.json"));

            return new BrowserExtensionInfo()
            {
                DisplayName = res.browser_action.default_title,
                MinQualityIcon = res.browser_action.default_icon["32"],
                MaxQualityIcon = res.browser_action.default_icon["64"],
                DefaultPopup = res.browser_action.default_popup,
                Path = extensionFolder
            };
        }

        public string DisplayName { get; set; }
        public string MinQualityIcon { get; set; }
        public string MaxQualityIcon { get; set; }
        public string DefaultPopup { get; set; }

        public string Path { get; set; }
    }

    public sealed partial class WebView : WebView2
    {
        private async void LoadExtensionsAsync() // TODO: See if it's really needed
        {
            StorageFolder extensionFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Extensions");

            foreach(StorageFolder folder in await extensionFolder.GetFoldersAsync())
            {
                string path = folder.Path;
                await this.CoreWebView2.Profile.AddBrowserExtensionAsync(path);
            }
        }


        public async void InstallExtensionAsync(string extensionId)
        {
            string url = $"https://clients2.google.com/service/update2/crx?response=redirect&os=linux&arch=x64&os_arch=x86_64&nacl_arch=x86-64&prod=chromium&prodchannel=unknown&prodversion=91.0.4442.4&lang=en-US&acceptformat=crx2,crx3&x=id%3D{extensionId}%26installsource%3Dondemand%26uc";

            var downloadFolder = ApplicationData.Current.TemporaryFolder;
            var extensionsFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Extensions", CreationCollisionOption.OpenIfExists);

            var extensionFolder = await extensionsFolder.CreateFolderAsync(extensionId, CreationCollisionOption.OpenIfExists);

            var download = DownloadBuilder.New()
            .WithUrl(url)
            .WithDirectory(downloadFolder.Path)
            .WithFileName(extensionId)
            .Build();

            download.DownloadFileCompleted += async (s, a) =>
            {
                string file = Path.Combine(downloadFolder.Path, extensionId);
                await ExtractExtensionAsync(file, extensionFolder.Path);

                await this.CoreWebView2.Profile.AddBrowserExtensionAsync(extensionFolder.Path);
            };
        }

        public async static Task ExtractExtensionAsync(string archivePath, string outFolder)
        {

            using (var fsInput = File.OpenRead(archivePath))
            using (var zf = new ICSharpCode.SharpZipLib.Zip.ZipFile(fsInput))
            {
                foreach (ICSharpCode.SharpZipLib.Zip.ZipEntry zipEntry in zf)
                {
                    if (!zipEntry.IsFile)
                    {
                        // Ignore directories
                        continue;
                    }
                    String entryFileName = zipEntry.Name;
                    // to remove the folder from the entry:
                    //entryFileName = Path.GetFileName(entryFileName);
                    // Optionally match entrynames against a selection list here
                    // to skip as desired.
                    // The unpacked length is available in the zipEntry.Size property.

                    // Manipulate the output filename here as desired.
                    var fullZipToPath = Path.Combine(outFolder, entryFileName);
                    var directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    // 4K is optimum
                    var buffer = new byte[4096];

                    // Unzip file in buffered chunks. This is just as fast as unpacking
                    // to a buffer the full size of the file, but does not waste memory.
                    // The "using" will close the stream even if an exception occurs.
                    using (var zipStream = zf.GetInputStream(zipEntry))
                    using (Stream fsOutput = File.Create(fullZipToPath))
                    {
                        ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(zipStream, fsOutput, buffer);
                    }
                }
            }

            await Task.Delay(1);
        }
    }
}
