using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;

namespace Odyssey.QuickActions.Helpers
{
    public class TestHelper
    {
        private readonly char[] operators = ['<', '>', '!'];
        public static string Test(string test)
        {
            string[] members = test.Split('=');

            string rawLeft = members[0]; // ex: hello world!, hello world=, 25< or 25>

            string left = rawLeft.Remove(rawLeft.Length - 1, 1); // ex: hello world or 25
            string right = members[1]; // ex: good luck or 59

            // Finding what test to do
            bool? negate = null;
            bool? greater = null;

            switch (rawLeft.Last())
            {
                case '!': negate = true; break;
                case '<': greater = false; break;
                case '>': greater = true; break;
            }

            // ex: when the test is hello world &= good luck (when the test is invalid)
            if (negate == null && greater == null) return "null";

            // ******** mathematical test *********
            if (greater != null)
            {
                bool isRightInt = int.TryParse(right, out int rightInt);
                bool isLeftInt = int.TryParse(left, out int leftInt);

                // ex: the test is linux >= windows
                if (!isRightInt || !isLeftInt) return "null";

                // doo the actual test
                if (greater == true) return (leftInt > rightInt).ToString().ToLower();
                else return (leftInt < rightInt).ToString().ToLower();
            }
            else // ******** equals test *********
            {
                return (negate == false && left == right).ToString().ToLower();
            }
        }

        
    }
}
