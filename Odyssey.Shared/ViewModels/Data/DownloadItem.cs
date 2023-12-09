using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Odyssey.Shared.ViewModels.Data
{
    [DataContract]
    public class DonwloadItem : INotifyPropertyChanged
    { 
        private string name;
        private string subtitle;
        private BitmapImage image;
        private bool downloadCompleted = false;
        public object downloadOperation;
        private int progress = 0;

        [DataMember]
        public string OutputPath
        {
            get; set;
        }
        [DataMember]
        public string DownloadUrl
        {
            get; set;
        }
        [DataMember]
        public string Name
        {
            get { return name; }
            set
            {
                if (value != name)
                {
                    name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [DataMember]
        public int Progress
        {
            get { return progress; }
            set
            {
                if (value != progress)
                {
                    progress = value;
                    NotifyPropertyChanged();
                }
            }
        }
        [DataMember]
        public bool DownloadCompleted
        {
            get { return downloadCompleted; }
            set
            {
                if (value != downloadCompleted)
                {
                    downloadCompleted = value;
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

        public string Subtitle
        {
            get { return subtitle; }
            set
            {
                if (value != subtitle)
                {
                    subtitle = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
