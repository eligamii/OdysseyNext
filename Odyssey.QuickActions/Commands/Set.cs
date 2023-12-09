using Odyssey.QuickActions.Objects;
using System.Collections.Generic;
using System.Linq;

namespace Odyssey.QuickActions.Commands
{
    internal static class Set
    {
        private static string value = string.Empty;
        private static string var = string.Empty;


        /// <summary>
        /// Set an user variable
        /// </summary>
        /// <param var="options">The options</param>
        /// <remarks>
        /// Available options :
        /// - (required) var: var of the variable to set
        /// - (required) value: the new value of the variable
        /// </remarks>
        /// <returns></returns>
        internal static Res Exec(string[] options)
        {
            if (options.Count() == 2)
            {
                foreach (string option in options)
                {
                    SetOptions(option); // set the value and var variables
                }

                if (value != string.Empty && var != string.Empty)
                {
                    // Save the variable as a KeyValuePair<var, value>

                    // Delete any variable with the same var (prevent having two times the same variable)
                    if (Data.UserVariables.Items.Any(p => p.Key == var))
                    {
                        var variable = Data.UserVariables.Items.Where(p => p.Key == var).ElementAt(0);
                        Data.UserVariables.Items.Remove(variable);
                    }

                    Data.UserVariables.Items.Add(new KeyValuePair<string, string>(var, $"\"{value}\""));

                    return new Res(true, $"\"{value}\"");
                }
            }

            return new Res(false, null, "This command requires at least two options: var and value");
        }

        private static void SetOptions(string option)
        {
            if (Option.IsAValidOptionString(option))
            {
                Option opt = new(option);

                switch (opt.Name)
                {
                    case "value": value = opt.Value; break;
                    case "var": var = opt.Value; break;
                }
            }
        }
    }
}
