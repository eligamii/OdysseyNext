using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Odyssey.WebSearch.Helpers
{
    internal static class MatchHelper
    {
        /// <summary>
        /// Compare two string based on a tolerability value
        /// </summary>
        /// <param name="str1">The first value to test</param>
        /// <param name="str2">The second value to test</param>
        /// <param name="tolerability">The persentage (from 0 to 1) of accepted typos</param>
        /// <returns></returns>
        public static MatchValue IsMatch(string str1, string str2, float tolerability)
        {
            if (!string.IsNullOrWhiteSpace(str1) && !string.IsNullOrEmpty(str2))
            {
                

                string longestString = str1.Length > str2.Length ? str1 : str2;
                string shortestString = str1.Length > str2.Length ? str2 : str1;

                longestString = new(longestString.ToASCII().Where(p => p.ToString() != " ").ToArray());
                shortestString = new(shortestString.ToASCII().Where(p => p.ToString() != " ").ToArray());

                char firstShortestStringChar = shortestString[0];
                List<int> indexes = new();

                for(int i = longestString.IndexOf(firstShortestStringChar); i != -1; i = longestString.IndexOf(firstShortestStringChar, i + 1))
                {
                    indexes.Add(i);
                }

                

                foreach(int index in indexes)
                {
                    int matches = 0;
                    int tests = 0;

                    for (int i = 0; i + index <= longestString.Length - 1 && i <= shortestString.Length - 1; i++)
                    {
                        tests++;
                        int longIndex = i + index;

                        if (longestString[longIndex] == shortestString[i]) 
                            matches++;
                    }

                    float successPercentage = matches / tests;

                    bool success = successPercentage >= tolerability;

                    if(success) 
                        return new MatchValue() { Success = success, SuccessRate = successPercentage };
                }
            }

            return new MatchValue() { Success = false };
        }

        private static string ToASCII(this string input) // WIP
        {
            Encoding encoding;
            string str = input;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            encoding = Encoding.GetEncoding(1250);
            str = encoding.GetString(encoding.GetBytes(str));
            encoding = Encoding.GetEncoding(1252);
            str = encoding.GetString(encoding.GetBytes(str));
            encoding = Encoding.ASCII;
            str = encoding.GetString(encoding.GetBytes(str));

            return str;
        }


        public class MatchValue
        {
            /// <summary>
            /// Returns true if the 
            /// </summary>
            public bool Success { get; set; }
            /// <summary>
            /// The percentage of characters that matches (0 to 1)
            /// </summary>
            public float SuccessRate { get; set; }
        }
    }
}
