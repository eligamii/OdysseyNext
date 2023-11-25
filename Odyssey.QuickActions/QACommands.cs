// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Odyssey.QuickActions.Commands;
using System.Linq;
using System.Text.RegularExpressions;
using Flyout = Odyssey.QuickActions.Commands.Flyout;

namespace Odyssey.QuickActions
{
    // Usage: command option:value option:<variable> option:"value with spaces,..."
    public static class QACommands
    {
        public static Frame Frame { get; set; }
        public static Window MainWindow { internal get; set; } // for commands which manipulate the app itself as $close or $minimize
        public static bool Execute(string command)
        {
            // Replace the <variable> with real values
            command = Variables.VariablesToValues(command);

            // Remove the first "$" is the command is from the search box
            if (command.StartsWith("$"))
                command = command.Substring(1);

            string commandName;

            try
            {
                // Get the command var (ex: flyout)
                commandName = Regex.Match(command, @"^[a-z]*").Value;
            }
            catch { return false; } // the entered command is in a $<non existant variable> pattern 


            // The string used by the commands
            string optionsRegex = @"([-a-zA-Z0-9()@:%_\+.~#?&/\\=;,]|(\*|%){0,1}"".*"")*";
            string commandWithoutCommandName = command.Replace(commandName, "");

            // Sepatate every option
            string[] options = Regex.Matches(commandWithoutCommandName, optionsRegex).Select(p => p.Value).Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();

            // Execute the command
            switch (commandName)
            {
                case "flyout": return Flyout.Exec(options);
                case "close": return Close.Exec(options);
                case "minimize": return Minimize.Exec(options);
                case "set": return Set.Exec(options);
                case "toast": return Toast.Exec(options);
                case "new": return New.Exec(options);
                case "test": return Test.Exec(options);

                default: return false;
            }
        }
    }
}
