using System;

namespace Odyssey.Shared.Helpers
{
    public class UnitsRepresentationHelper
    {
        public static string ToString(double byteSize)
        {
            double size = byteSize;
            int unit = 0; // b

            while (!(size < 1000))
            {
                unit++;
                size = size / 1024;              
            }

            return $"{Math.Round(size, 1)} {GetUnitFromExponant(unit)}";
        }

        private static string GetUnitFromExponant(int exponant)
        {
            string byteRepr = ResourceString.GetString("ByteUnit", "Shared"); // B in english, o in french (Set this to "B" or whatever you want for your projects)

            switch(exponant)
            {
                case 1: return byteRepr;
                case 2: return "M" + byteRepr;
                case 3: return "G" + byteRepr;
                case 4: return "T" + byteRepr;
                // Do not need any unit higher than TB (4) but it's the exact same logic for PB and higher

                default: return string.Empty;
            }

        }
    }
}
