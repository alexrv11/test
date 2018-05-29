using System;
using System.Globalization;
using System.Text;

namespace BGBA.Services.N.Enrollment.SCS
{
    internal class Utils
    {
        static String candidateAlphanumericChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        static String candidateChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static String generateRandomString(bool alphanumeric, int length)
        {
            String availableCharacters = (alphanumeric) ? candidateAlphanumericChars : candidateChars;

            StringBuilder sb = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                sb.Append(availableCharacters[random.Next(availableCharacters.Length)]);
            }

            return sb.ToString();
        }

        public static string jsubstring(string s, int beginIndex, int endIndex)
        {
            int len = endIndex - beginIndex;
            return s.Substring(beginIndex, len);
        }

        public static String generateRandomAZString(int length)
        {
            String enc = "";
            double rnd = 0;
            for (int i = 0; (i < length); i++)
            {
                rnd = ((new Random().Next() * 25) + 65);
                enc = (enc + ((char)(rnd)));
            }

            return enc;
        }

        public static String hexEncode(byte[] data)
        {
            char[] c = new char[data.Length * 2];
            byte b;
            for (int i = 0; i < data.Length; ++i)
            {
                b = ((byte)(data[i] >> 4));
                c[i * 2] = (char)(b > 9 ? b + 0x37 : b + 0x30);
                b = ((byte)(data[i] & 0xF));
                c[i * 2 + 1] = (char)(b > 9 ? b + 0x37 : b + 0x30);
            }

            return new string(c);
        }

        public static byte[] hexDecode(String hexEncoded)
        {
            byte[] hexDecoded = new byte[hexEncoded.Length / 2];
            String sub = "";

            for (int i = 0; i < hexDecoded.Length; i++)
            {
                sub = jsubstring(hexEncoded, i * 2, i * 2 + 2);
                int size = int.Parse(sub, NumberStyles.AllowHexSpecifier);
                hexDecoded[i] = (byte)size;
            }
            return hexDecoded;
        }
    }
}
