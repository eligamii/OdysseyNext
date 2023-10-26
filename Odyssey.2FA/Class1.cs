using Odyssey._2FA.ViewModels;
using OtpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Odyssey._2FA
{
    public class Class1
    {
        public Class1()
        {
            TwoFactorAuth twoFactorAuth = new()
            {
                Issuer = "Git",
                Secret = Base32Encoding.ToBytes("secret")
            };

            twoFactorAuth.Init();
        }

    }
}
