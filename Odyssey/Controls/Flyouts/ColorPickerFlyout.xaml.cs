using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
namespace Odyssey.Controls.Flyouts
{
    public sealed partial class ColorPickerFlyout : Flyout
    {
        public delegate void ColorChangedEventHandler(ColorPicker sender, ColorChangedEventArgs args);
        public event ColorChangedEventHandler ColorChanged;

        public ColorPickerFlyout()
        {
            this.InitializeComponent();
            ColorChanged += (s, a) => { };
        }

        private void colorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args) => ColorChanged(sender, args);

    }
}
