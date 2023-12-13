using Odyssey.Migration;
using Odyssey.QuickActions.Objects;

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
