// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Odyssey.QuickActions.Commands;
using Odyssey.QuickActions.Data;
using Odyssey.QuickActions.Objects;
using System.Linq;
using System.Text.RegularExpressions;
using Flyout = Odyssey.QuickActions.Commands.Flyout;

namespace Odyssey.QuickActions
{
    // Usage: command option:value option:<variable> option:"value with spaces,..." option:tabindex
    public static class QACommands
    {
        public static StackPanel ButtonsStackPanel { internal get; set; }
        public static Frame Frame { get; set; }
        public static Window MainWindow { internal get; set; } // for commands which manipulate the app itself as $close or $minimize do
        public static void RestoreUIElements()
        {
            foreach(var variable in UserVariables.Items.Where(p => p.Key.StartsWith("ui")))
            {
                if(variable.Key.StartsWith("uib"))
                {
                    new UIButton(variable.Value, true);
                }
            }
        }

        public static Res Execute(string command)
        {
            // Replace the <variable> with real values
            command = Variables.ConvertToValues(command);

            // Remove the first "$" is the command is from the search box
            if (command.StartsWith("$"))
                command = command.Substring(1);

            string commandName;
           
            try
            {
                // Get the command var (ex: flyout)
                commandName = Regex.Match(command, @"^[a-z]*").Value;
            }
            catch { return new Res(false, null, "the entered command is in a $<non existant variable> pattern"); } // the entered command is in a $<non existant variable> pattern 

            string commandWithoutCommandName;
            // The string used by the commands
            string optionsRegex = @"([-a-zA-Z0-9()@:%_\+.~#?&/\\=;,]|(\*|%){0,1}"".*"")*";
            try
            {
                commandWithoutCommandName = command.Substring(commandName.Length + 1); // also remove the first space
            }
            catch 
            {
                commandWithoutCommandName = string.Empty;
            }

            // Sepatate every option
            string[] options = Regex.Matches(commandWithoutCommandName, optionsRegex).Select(p => p.Value).Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();

            // Execute the command
            switch (commandName)
            {
                case "flyout": return Flyout.Exec(options); // opens a flyout at desired position
                case "close": return Close.Exec(options); // close the window or tabs
                case "minimize": return Minimize.Exec(options); // minimize the window
                case "set": return Set.Exec(options); // create or modify a new variable
                case "toast": return Toast.Exec(options); // create a toast notification, wip
                case "new": return New.Exec(options); // create a new tab
                case "test": return Test.Exec(options); // only for testing purposes, to remove in stable releases
                case "js": return Js.Exec(options); // execute js scripts
                case "navigate": return Navigate.Exec(options); // navigate to an url
                case "ui": return Ui.Exec(options); // Create a control visible and usable by the user
                case "back": return Back.Exec(options); // go back
                case "forward": return Forward.Exec(options); // go forward
                case "refresh": return Refresh.Exec(options); // refresh the webview, wip
                case "webview": return Webview.Exec(options); // webview tools, like devtools, task manager, etc, wip

                default: return new Res(false, null, "Command not found");
            }
        }
    }
}
