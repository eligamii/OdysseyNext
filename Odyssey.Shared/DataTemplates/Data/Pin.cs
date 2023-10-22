using Odyssey.Shared.Enums;

namespace Odyssey.Shared.DataTemplates.Data
{
    // The pin oject to be saved / shown on UI
    public class Pin : Tab
    {
        public string Icon { get; set; }
        public int Index { get; set; }
        public PinShowMode ShowMode { get; set; } = PinShowMode.OnPane;
    }
}
