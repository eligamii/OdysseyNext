using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odyssey.QuickActions.Commands
{
    internal class Test
    {
        internal static bool Exec(string[] options)
        {
            Odyssey.Migration.Chromium.Passwords.Get();
            return true;
        }
    }
}
