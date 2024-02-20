


using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Odyssey.QuickActions.Commands;
using Odyssey.QuickActions.Data;
using Odyssey.QuickActions.Helpers;
using Odyssey.QuickActions.Objects;
using Odyssey.QuickActions.SystemCommands;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
            foreach (var variable in UserVariables.Items)
            {
                string pattern = "\"([^ ]*,){3}[^ ]*\"";
                bool match = Regex.IsMatch(variable.Value, pattern);

                if (match)
                {
                    _ = new UIButton(variable.Value, true);
                }
            }
        }

        private static async Task<string> ExecuteSubCommands(string command)
        {
            Regex subCommandPresentRegex = new("(?<!\\\\)(\\[|\\])");
            Regex subCommandsSeparator = new("(?<=\\[)[^\\]\\[]*(?=\\])");
            var c = subCommandPresentRegex.Matches(command).Count();
            while (c % 2 == 0 && c > 0)
            {
                foreach (Match match in subCommandsSeparator.Matches(command))
                {
                    Res res = await Execute(match.Value);
                    command = command.Replace($"[{match.Value}]", $"\"{res.Output}\"");
                    c = subCommandPresentRegex.Matches(command).Count();
                }
            }

            return command;
        }

        private static string ResolveTests(string command)
        {
            Regex testPresentRegex = new("(?<!\\\\)(\\(|\\))");
            Regex testSeparator = new("(?<=\\()[^\\)\\(]*(?=\\))");
            var c = testPresentRegex.Matches(command).Count();
            while (c > 0)
            {
                foreach (Match match in testSeparator.Matches(command))
                {
                    string boolean = TestHelper.ResolveTest(match.Value);
                    command = command.Replace($"({match.Value})", $"{boolean}");
                    c = testSeparator.Matches(command).Count();
                }
            }

            return command;
        }


        public static async Task<Res> Execute(string command)
        {
            // Execute sub-commands
            command = await ExecuteSubCommands(command);

            // Replace the <variable> with real values
            command = Variables.ConvertToValues(command);

            // Resolve the (test)
            command = ResolveTests(command);

            // Remove the first "$" is the command is from the search box
            if (command.StartsWith("$"))
                command = command.Substring(1);

            string commandName;

            try
            {
                // Get the command var (ex: flyout)
                commandName = Regex.Match(command, @"^[a-z]*").Value;
            }
            catch { return new Res(false, null, "The entered command is in a $<non existant variable> pattern"); } // the entered command is in a $<non existant variable> pattern 

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

            // Execute the command (it's the only part you should worry about when adding new commands)
            switch (commandName)
            {

                case "if": return await If.Exec(options); // execute another command only if the (test) returns true, wip
                case "while": return await While.Exec(options); // execute the command while the test is true, wip

                case "flyout": return Flyout.Exec(options); // opens a flyout at desired position
                case "close": return Close.Exec(options); // close the window or tabs
                case "minimize": return Minimize.Exec(options); // minimize the window
                case "set": return Set.Exec(options); // create or modify a new variable
                case "toast": return Toast.Exec(options); // create a toast notification, wip
                case "new": return New.Exec(options); // create a new tab
                case "test": return await Test.Exec(options); // only for testing purposes, to remove in stable releases
                case "js": return await Js.Exec(options); // execute js scripts
                case "navigate": return Navigate.Exec(options); // navigate to an url
                case "ui": return Ui.Exec(options); // create a control visible and usable by the user
                case "back": return Back.Exec(options); // go back
                case "forward": return Forward.Exec(options); // go forward
                case "refresh": return Refresh.Exec(options); // refresh the webview, wip
                case "webview": return Webview.Exec(options); // webview tools, like devtools, task manager, etc, wip
                case "kdesend": return await Kde_send.Exec(options); // share a file or a link to another device, wip
                case "calc": return Calc.Exec(options); // Calculate a mathematical expression using NCalc

                default: return new Res(false, null, "Command not found");
            }
        }
    }
}
