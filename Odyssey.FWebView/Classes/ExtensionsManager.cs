using Downloader;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using Odyssey.FWebView.Controls;
using Odyssey.FWebView.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Odyssey.FWebView
{
    public class BrowserExtensionInfo
    {

        public static async Task<BrowserExtensionInfo> GetFromExtensionFolderAsync(string extensionId)
        {
            var extensionsFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Extensions");
            string extensionFolder = Path.Combine(extensionsFolder.Path, extensionId);

            string json = File.ReadAllText(Path.Combine(extensionFolder, "manifest.json"));
            var res = JsonConvert.DeserializeObject<ExtensionManifestDeserializer>(json);

            return new BrowserExtensionInfo()
            {
                DisplayName = res.browser_action.default_title,
                MinQualityIcon = Path.Combine(extensionFolder, res.browser_action.default_icon.First().Value),
                MaxQualityIcon = Path.Combine(extensionFolder, res.browser_action.default_icon.Last().Value),
                DefaultPopup = $"extension://{extensionId}/{res.browser_action.default_popup}",
                Id = extensionId
            };
        }

        public string DisplayName { get; set; }
        public string MinQualityIcon { get; set; }
        public string MaxQualityIcon { get; set; }
        public string DefaultPopup { get; set; }
        public string Id { get; set; }

        public void OpenDefultPopup(FrameworkElement target)
        {
            ExtensionPopupFlyout flyout = new(this);
            flyout.ShowAt(target);
        }
    }

    public sealed partial class WebView : WebView2
    {
        public static async Task<List<BrowserExtensionInfo>> GetExtensionsAsync()
        {
            StorageFolder extensionFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Extensions", CreationCollisionOption.OpenIfExists);
            List<BrowserExtensionInfo> browserExtensionsList = new();

            foreach (StorageFolder folder in await extensionFolder.GetFoldersAsync())
            {
                string path = folder.Path;
                browserExtensionsList.Add(await BrowserExtensionInfo.GetFromExtensionFolderAsync(folder.Name));
            }

            return browserExtensionsList;
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

            download.StartAsync();

            download.DownloadFileCompleted += async (s, a) =>
            {
                string file = Path.Combine(downloadFolder.Path, extensionId);
                await ExtractExtensionAsync(file, extensionFolder.Path);

                //await this.CoreWebView2.Profile.AddBrowserExtensionAsync(extensionFolder.Path);
                DispatcherQueue.TryEnqueue(async () => CurrentlySelectedWebViewEventTriggered(this.CoreWebView2, new CurrentlySelectedWebViewEventTriggeredEventArgs(await BrowserExtensionInfo.GetFromExtensionFolderAsync(extensionId), EventType.ExtensionInstalled)));
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
