using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Odyssey.TwoFactorsAuthentification.ViewModels
{
    public class TwoFactAuth : INotifyPropertyChanged
    {
        private string code;
        private int progressValue = 100;

        private int remainingSeconds;


        [DataMember]
        public OtpNet.OtpHashMode OtpHashMode { get; set; } = OtpNet.OtpHashMode.Sha1;
        [DataMember]
        public int Size { get; set; } = 6;
        [DataMember]
        public int Step { get; set; } = 30;
        [DataMember]
        public string Name { get; set; }

        public string Code
        {
            get { return code; }
            set
            {
                if (value != code)
                {
                    code = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int ProgressValue
        {
            get { return progressValue; }
            set
            {
                if (value != progressValue)
                {
                    progressValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private OtpNet.Totp totp;
        public void Start(byte[] secret)
        {
            totp = new(secret, Step, OtpHashMode, Size);

            Refresh();

        }

        private async void Refresh()
        {
            // Calibrate the timer
            int sec = totp.RemainingSeconds();
            while (totp.RemainingSeconds() == sec)
            {
                await Task.Delay(10);
            }

            Code = totp.ComputeTotp();

            while (true)
            {
                remainingSeconds = totp.RemainingSeconds();
                ProgressValue = 100 * remainingSeconds / 30;

                if (remainingSeconds == 30)
                {
                    // Generate new code
                    Code = totp.ComputeTotp();
                }

                await Task.Delay(1000);
            }
        }
    }
}
