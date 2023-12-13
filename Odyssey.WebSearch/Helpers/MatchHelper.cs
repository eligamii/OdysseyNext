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
            if (str2 != null)
            {
                float length = str1.Length;
                float matches = 0;
                float t = tolerability;

                for (int i = 0; i < length && i < str2.Length; i++)
                {
                    char str1char = str1[i].ToString().ToLower().ToCharArray()[0];
                    char str2char = str2[i].ToString().ToLower().ToCharArray()[0];

                    if (str1char == str2char) { matches++; }
                }

                float f = 0;

                if (matches > 0)
                    f = matches / length;
                bool b = f >= t;

                var value = new MatchValue() { Success = b, Value = f };

                return value;
            }
            else
            {
                return new MatchValue() { Success = false };
            }
        }

        public class MatchValue
        {
            /// <summary>
            /// Returns true if the 
            /// </summary>
            public bool Success { get; set; }
            /// <summary>
            /// The persentage of characters that matches (0 to 1)
            /// </summary>
            public float Value { get; set; }
        }
    }
}
