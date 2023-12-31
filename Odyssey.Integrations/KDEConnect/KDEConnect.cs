using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.System;

namespace Odyssey.Integrations.KDEConnect
{
    public class Device
    {
        public string Name { get; private set; }
        public string Id { get; private set; }

        internal Device(string name, string id) 
        {
            Name = name;
            Id = id;
        }
    }

    public class KDEConnect
    {
        private static Regex idRegex = new("^[^ ]*");
        private static async Task<string> Send(string args)
        {
            Process process = new();

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "kdeconnect-cli",
                Arguments = $"{args}",
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };

            process.StartInfo = startInfo;
            process.Start();

            await process.WaitForExitAsync();

            return await process.StandardOutput.ReadToEndAsync();
        }

        public static async Task<List<Device>> GetDevicesAsync()
        {
            List<Device> devices = new();

            string rawOutput = await Send("-l");
            string[] devicesLines = rawOutput.Split("\r\n").Where(p => p.StartsWith('-')).ToArray();

            foreach(string line in devicesLines)
            {
                string lineWithoutStartChar = line.Remove(0, 2);

                string name = lineWithoutStartChar.Split(" : ")[0];

                string rawId = lineWithoutStartChar.Split(" : ")[1];
                string id = idRegex.Match(rawId).Value;

                Device device = new(name, id);
                devices.Add(device);
            }

            return devices;
        }

        public static async void SendMessage(string message, Device device) => await Send($"-d {device.Id} --ping-msg \"{message}\"");
        public static async void Share(string urlOrFilePath, Device device) => await Send($"-d {device.Id}  --share \"{urlOrFilePath}\"");
    }
}
