using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Newtonsoft.Json.Linq;
using Odyssey.QuickActions.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Odyssey.QuickActions.Objects
{
    internal class UIButton
    {
        Button XAMLButton { get; set; }
        private string command;
        public UIButton(string option, bool isRestoring = false) // button,glyph,command-variable syntax
        {
            string[] options = option.Split(',');
            int unicode = int.Parse(options[1], System.Globalization.NumberStyles.HexNumber);

            string variable = options[2].Replace("\"", "");

            char glyph = (char)unicode;
            this.command = Variables.ConvertToValues($"<{variable}>");

            XAMLButton = new()
            {
                Background = new SolidColorBrush(Colors.Transparent),
                BorderThickness = new Microsoft.UI.Xaml.Thickness(0),
                FontFamily = new FontFamily("Segoe Fluent Icons"),

                Content = glyph
            };

            XAMLButton.Click += XAMLButton_Click;

            // Save the UIButton 
            if(!isRestoring) QACommands.Execute($"$set var:ui{option[0]}{new Guid()} value:\"{option}\"").RunSynchronously();
            QACommands.ButtonsStackPanel.Children.Insert(2, XAMLButton);
        }

        private async void XAMLButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if(command.StartsWith("\"")) command = command.Remove(command.IndexOf("\""), 1).Remove(command.Length - 2, 1);
            await QACommands.Execute(command);
        }
    }
}
