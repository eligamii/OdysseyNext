using Microsoft.UI.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
