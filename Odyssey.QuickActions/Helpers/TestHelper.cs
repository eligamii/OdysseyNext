using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;

namespace Odyssey.QuickActions.Helpers
{
    public class TestHelper
    {
        private static Regex membersSeparatorRegex = new(@"[^<>\!\=]*");
        private static Regex operatorSeparatorRegex = new (@"[<>\!\=]{0,1}\={0,1}");
        
        public static string ResolveTest(string test)
        {
            var members = membersSeparatorRegex.Matches(test);
            string operators = operatorSeparatorRegex.Matches(test)[0].Value;
            
            bool? greater;
            bool negate = false;
            bool equals = false;
            
            bool operatorTest = false;
            
            switch(operators[0])
            {
                case '<':
                    operatorTest = IsGreater(members[0].Value, members[1].Value, false);
                    break;
                    
                case '>':
                    operatorTest = IsGreater(members[0].Value, members[1].Value, false);
                    break;
                    
                case '!':
                    negate = true;
                    break;
                    
                case '=': break;
                
                default:
                    return "null";
            }
           
            if(operators.Length == 2)
            {
                if(operators[1] == '=')
                {
                    if(negate) 
                    {
                        return (!(operatorTest || members[0].Value == members[1].Value)).ToString();
                    }
                    else 
                    {
                        return (operatorTest || members[0].Value == members[1].Value).ToString();
                    }
                }
                else
                {
                    return "null";
                }
            }
            else
            {
                return operatorTest.ToString();
            }
            
        }
        
        private static bool IsGreater(string member1, string member2, bool greater)
        {
            float intMember1, intMember2 = 0; 
            bool membersAreNumbers = float.TryParse(member1, out intMember1) && float.TryParse(member2, out intMember2);
            bool membersAreCoordinates = Regex.IsMatch(member1, "[0-9]*;[0-9]*") &&  Regex.IsMatch(member2, "[0-9]*;[0-9]*");       
            int lengthMember1 = member1.Length; int lengthMember2 = member2.Length;

            if(membersAreNumbers)
            {
                if(greater)
                    return intMember1 > intMember2;
                else
                    return intMember1 < intMember2;
            }
            else if(Regex.IsMatch(member1, @"[0-9]*;[0-9]"))
            {
                if(greater) { return int.Parse(member1.Split(";")[1]) > int.Parse(member2.Split(";")[1]); }
                else { return int.Parse(member1.Split(";")[1]) < int.Parse(member2.Split(";")[1]); }
            }
            else
            {
                if(greater) {return lengthMember1 > lengthMember2;}
                else {return lengthMember1 < lengthMember2;}
            }
        }
        
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
