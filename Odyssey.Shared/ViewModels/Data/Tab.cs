using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Odyssey.Shared.ViewModels.Data
{
    [DataContract]
    public class Tab : INotifyPropertyChanged
    {
        private string url;
        private string title;
        private BitmapImage image;
        private string tip;
        private SolidColorBrush color = new(Windows.UI.Color.FromArgb(0, 0, 0, 0));

        [DataMember]
        public string Url
        {
            get { return url; }
            set
            {
                if (value != url)
                {
                    url = value;
                    NotifyPropertyChanged();
                }
            }
        }
        [DataMember]
        public string Title
        {
            get { return title; }
            set
            {
                if (value != title)
                {
                    title = value;
                    NotifyPropertyChanged();
                }
            }
        }
        [DataMember]
        public bool IsSelected
        {
            get; set;
        }
        public SolidColorBrush ForegroundColor
        {
            get { return color; }
            set
            {
                if (value != color)
                {
                    color = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public BitmapImage ImageSource
        {
            get { return image; }
            set
            {
                if (value != image)
                {
                    image = value;
                    NotifyPropertyChanged();
                }
            }
        }
        [DataMember]
        public string ToolTip
        {
            get { return tip; }
            set
            {
                if (value != tip)
                {
                    tip = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public WebView2 MainWebView
        {
            get;
            set;
        }
        public WebView2 SplitViewWebView
        {
            get;
            set;
        }

        public bool IsInPrivateMode
        {
            get;
            set;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
