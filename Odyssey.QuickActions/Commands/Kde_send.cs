using Odyssey.Integrations.KDEConnect;
using Odyssey.QuickActions.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odyssey.QuickActions.Commands
{
    internal class Kde_send
    {
        private static string content = string.Empty;
        internal static async Task<Res> Exec(string[] options)
        {
            if(options.Count() == 1)
            {
                SetOptions(options[0]);

                if(content != string.Empty)
                {
                    var devices = await KDEConnect.GetDevicesAsync();
                    
                    if(devices.Count >= 1)
                    {
                        KDEConnect.Share(content, devices[0]);

                        return new Res(true, null);
                    }
                    else
                    {
                        return new Res(false, null, "KDEConnect is not installed on your device / No device is connected to KDEConnect.");
                    }
                }

                return new Res(false, null, "This command requires one parameter: content.");
            }

            return new Res(false, null, "This command requires only one parameter: content.");
        }

        private static void SetOptions(string option)
        {
            if (Option.IsAValidOptionString(option))
            {
                Option opt = new(option);

                switch (opt.Name)
                {
                    case "content": content = opt.Value; break;
                }
            }
        }
    }
}
