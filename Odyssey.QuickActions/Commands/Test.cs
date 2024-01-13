using Odyssey.Migration;
using Odyssey.QuickActions.Objects;

namespace Odyssey.QuickActions.Commands
{
    internal class Test
    {
        internal static async System.Threading.Tasks.Task<Res> Exec(string[] options)
        {
            //Odyssey.Migration.Migration.Migrate(Browser.Edge);
            return new Res(true, await POST());
        }

        internal static async System.Threading.Tasks.Task<string> POST()
        {
            return await AriaSharp.Helpers.AriaRPCHelper.AddUriAsync(["https://proof.ovh.net/files/1Mb.dat"]);            
        }
    }
}
