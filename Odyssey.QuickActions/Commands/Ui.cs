using Odyssey.Migration;
using Odyssey.QuickActions.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odyssey.QuickActions.Commands
{
    internal class Ui
    {
        internal static Res Exec(string[] options)
        {
            if(options.Count() == 1)
            {
                string type = options[0].Split(',')[1];

                switch(type)
                {
                    case "button":
                        new UIButton(options[0]);
                        break;
                }
            }

            return new Res(true);
        }
    }
}
