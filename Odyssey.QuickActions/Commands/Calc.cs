using Odyssey.QuickActions.Objects;

namespace Odyssey.QuickActions.Commands
{
    internal class Calc
    {
        internal static Res Exec(string[] options)
        {
            string expression = string.Empty;
            foreach (var option in options) expression += option + " ";

            var calc = new NCalc.Expression(expression);
            if (calc.HasErrors()) return new Res(false, "0", "Wrong syntax");

            string res = calc.Evaluate().ToString();

            return new Res(true, res);
        }
    }
}
