using Microsoft.UI.Windowing;
using System.Linq;

namespace Odyssey.QuickActions.Commands
{
    internal class Minimize
    {
        internal static bool Exec(string[] options)
        {
            if (options.Count() == 0)
            {
                ((OverlappedPresenter)QACommands.MainWindow.AppWindow.Presenter).Minimize();
                return true;
            }
            else return false;
        }
    }
}
