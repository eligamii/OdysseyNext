﻿using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace Odyssey.Shared.Helpers
{
    public sealed class SymbolIconEx : FontIcon
    {
        public SymbolIconEx(SymbolEx symbol) 
        { 
            FontFamily = new FontFamily("Segoe Fluent Icons");
            Symbol = symbol;
        }

        public SymbolIconEx()
        {
            FontFamily = new FontFamily("Segoe Fluent Icons");
        }

        public SymbolEx Symbol
        {
            get => (SymbolEx)Glyph[0];
            set => Glyph = ((char)value).ToString();
        }
    }
}
