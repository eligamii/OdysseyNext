using Microsoft.UI.Xaml.Media.Imaging;

namespace Odyssey.Shared.ViewModels.Data
{
    public class Favorite : Tab
    {
        private int width = 48;

        public int Width
        {
            get { return width; }
            set
            {
                if (value != width)
                {
                    width = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public BitmapImage Icon { get; set; }

    }

}
