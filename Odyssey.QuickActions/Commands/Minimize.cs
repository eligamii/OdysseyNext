using Microsoft.UI.Windowing;
using Odyssey.QuickActions.Objects;
using System.Linq;

namespace Odyssey.QuickActions.Commands
{
    internal class Minimize
    {
        internal static Res Exec(string[] options)
        {
            if (options.Count() == 0)
            {
                ((OverlappedPresenter)QACommands.MainWindow.AppWindow.Presenter).Minimize();
                return new Res(true);
            }
            else return new Res(false, null, "Invalid syntax (no parameter is needed).");
        }
    }
}
