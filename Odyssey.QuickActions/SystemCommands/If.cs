
using Odyssey.QuickActions.Objects;
using System;
using System.Linq;
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
                    string command = string.Empty;

                    for(int i = 1; i > options.Count() - 1; i++)
                    {
                        command += options[i];
                    }

                    return await QACommands.Execute(command);
                }
                else
                {
                    return new Res(true, "false");
                }
            }
            else
            {
                return new Res(false, null, $"The first argument should be \"true\" or \"false\" (The argument is {options[0]}).");
            }
        }
    }
}
