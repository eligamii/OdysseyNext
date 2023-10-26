using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinRT;

namespace Odyssey._2FA.ViewModels
{
    internal class TwoFactorAuth
    {
        public string Issuer { get; set; }
        public byte[] Secret { get; set; }
        public string Code { get; set; }
        public int RemainingSeconds { get; set; }

        private OtpNet.Totp totp;
        public void Init()
        {
            totp = new(Secret);

            Refresh();
        }

        private async void Refresh()
        {
            // Calibrate the timer
            int sec = totp.RemainingSeconds();
            while(totp.RemainingSeconds() == sec)
            {
                await Task.Delay(100);
            }

            while(true)
            {
                RemainingSeconds = totp.RemainingSeconds();

                if(RemainingSeconds == 30)
                {
                    // Generate new code
                     Code = totp.ComputeTotp();
                }

                await Task.Delay(1000);
            }
        }
    }
}
