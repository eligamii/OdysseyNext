using Odyssey.Migration;
using Odyssey.QuickActions.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odyssey.QuickActions.Commands
{
    internal class Test
    {
        internal static Res Exec(string[] options)
        {
            Odyssey.Migration.Migration.Migrate(Browser.Edge);
            return new Res(true);
        }
    }
}
