﻿using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Odyssey.QuickActions.Objects
{
    internal class Option
    {
        internal static bool IsAValidOptionString(string optionString)
        {
            return Regex.IsMatch(optionString, ".*:.{1,}") && optionString.Count(p => p == '"') % 2 == 0;
        }

        internal Option(string optionString)
        {
            string optionSeparatorRegex = @"([-a-zA-Z0-9()@%_\+.~#?&/\\=;]|"".*"")*";

            string Name = Regex.Match(optionString, optionSeparatorRegex).Value;
            string Value = Regex.Matches(optionString, optionSeparatorRegex).Select(p => p.Value).ElementAt(2); // every two value is empty

            // Simple filter
            if(Value.EndsWith("\""))
            {
                string valueWithoutQuote = Value.Remove(Value.IndexOf("\""), 1).Remove(Value.Length - 2, 1);

                if(Value.StartsWith("\""))
                {
                    Value = valueWithoutQuote;
                }
                else
                {
                    string valueWithoutSpecial = valueWithoutQuote.Remove(0, 1);

                    switch (Value.First())
                    {
                        case '%':
                            Value = WebUtility.UrlEncode(valueWithoutSpecial);
                            break;

                        case '*':
                            var expression = new NCalc.Expression(valueWithoutSpecial);
                            if (expression.HasErrors()) Value = "Syntax error";
                            else Value = expression.Evaluate().ToString();
                            break;
                    }
                }
            }

            
        }
        internal string Name { get; private set; }
        internal string Value { get; private set; }

    }
}
