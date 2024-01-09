using Odyssey.Migration;
using Odyssey.QuickActions.Objects;

namespace Odyssey.QuickActions.Commands
{
    internal class Test
    {
        internal static async System.Threading.Tasks.Task<Res> Exec(string[] options)
        {
            Odyssey.Migration.Migration.Migrate(Browser.Edge);
            return new Res(true, await POST());
        }

        internal static async System.Threading.Tasks.Task<string> POST()
        {
            using System.Net.Http.HttpClient client = new();
            var json = "{\"jsonrpc\":\"2.0\",\"method\":\"aria2.addUri\",\"params\":{\"token\":\"\",[\"https://proof.ovh.net/files/1Mb.dat\"]}}";
            var response = await client.PostAsync($"http://localhost:6800/jsonrpc", new System.Net.Http.StringContent(json));
            return await response.Content.ReadAsStringAsync();
        }
    }
}
