using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Odyssey.QuickActions.Commands
{
    internal static class Set
    {
        private static string value = string.Empty;
        private static string name = string.Empty;


        /// <summary>
        /// Set an user variable
        /// </summary>
        /// <param name="options">The options</param>
        /// <remarks>
        /// Available options :
        /// - (required) name: name of the variable to set
        /// - (required) value: the new value of the variable
        /// </remarks>
        /// <returns></returns>
        internal static bool Exec(string[] options)
        {
            if (options.Count() == 2)
            {
                foreach(string option in options)
                {
                    SetOptions(option); // set the value and name variables
                }

                if(value != string.Empty && name != string.Empty)
                {
                    // Save the variable as a KeyValuePair<name, value>

                    // Delete any variable with the same name (prevent having two times the same variable)
                    if (Variables.UserVariables.Any(p => p.Key == name))
                    {
                        var variable = Variables.UserVariables.Where(p => p.Key == name).ElementAt(0);
                        Variables.UserVariables.Remove(variable);
                    }

                    Variables.UserVariables.Add(new KeyValuePair<string, string>(name, value));

                    return true;
                }
            }

            return false; // this command requires at least two options: name and value
        }

        private static void SetOptions(string option)
        {
            string optionSeparatorRegex = @"([-a-zA-Z0-9()@%_\+.~#?&/\\=]|""[-a-zA-Z0-9()@:%_\+.~#?&/\\= ]*"")*";

            string optionName = Regex.Match(option, optionSeparatorRegex).Value;
            string optionValue = Regex.Matches(option, optionSeparatorRegex).Select(p => p.Value).ElementAt(2); // every two value is empty

            if (optionValue.StartsWith("\""))
                optionValue = optionValue.Remove(0, 1).Remove(optionValue.Length - 2, 1);


            switch (optionName)
            {
                case "value": value = optionValue; break;
                case "name": name = optionValue; break;
            }
        }
    }
}
