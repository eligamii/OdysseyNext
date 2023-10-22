using Odyssey.Shared.DataTemplates.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odyssey.Data.Main
{
    public class Tabs
    {
        public static ObservableCollection<Tab> Items { get; set; } = new();
    }
}
