
using Odyssey.QuickActions.Objects;
using System.Threading.Tasks;

namespace Odyssey.QuickActions.SystemCommands
{
    internal class If
    {
        internal static async Task<Res> Exec(string[] options)
        {
            string test = options[0]; // true, false or null

            if (test == "true" || test == "false")
            {
                if (test == "true")
                {
                    return await QACommands.Execute(new Option(options[1]).Value);
                }
                else
                {
                    return new Res(true, "false");
                }
            }
            else
            {
                return new Res(false, null, "Wrong test");
            }
        }
    }
}
