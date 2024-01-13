using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;

namespace Odyssey.QuickActions.Helpers
{
    public static partial class TestHelper
    {
        [GeneratedRegex(@"[^<>\!\=\|\&]*")]
        private static partial Regex MembersSeparatorRegex();

        [GeneratedRegex(@"[<>\!\=\|\&]{0,1}[\=\|\&]{0,1}")]
        private static partial Regex OperatorRegex();

        public static string ResolveTest(string test)
        {
                                                               // Prevent the array from having multiple blank values
            var members = MembersSeparatorRegex().Matches(test).Where(p => p.Length > 0).ToArray();
            string operators = OperatorRegex().Matches(test).Where(p => p.Length > 0).ToArray()[0].Value;
            
            bool negate = false;
            
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

                case '|':
                    operatorTest = members[0].Value == "true" || members[1].Value == "true";
                    break;

                case '&':
                    operatorTest = members[0].Value == "true" && members[1].Value == "true";
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
                        return (!(operatorTest || members[0].Value == members[1].Value)).ToString().ToLower();
                    }
                    else 
                    {
                        return (operatorTest || members[0].Value == members[1].Value).ToString().ToLower();
                    }
                }
                else if ((operators[1] == '|' && operators[0] == '|') || (operators[1] == '&' && operators[0] == '&')
                      && (members[0].Value == "true" || members[0].Value == "false") // the members should be only be true or false
                      && (members[1].Value == "true" || members[1].Value == "false"))
                {
                    return operatorTest.ToString().ToLower();
                }
                else
                {
                    return "null";
                }
            }
            else
            {
                return operatorTest.ToString().ToLower();
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
            else if(membersAreCoordinates)
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


    }
}
