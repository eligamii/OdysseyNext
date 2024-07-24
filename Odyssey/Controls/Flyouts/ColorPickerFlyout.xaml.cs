using Microsoft.UI.Xaml.Controls;
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
