using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Core.N.Utils.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveDiacritics(this String inputString)
        {
            String normalizedString = inputString.RevertTextFromHtmlFriendly().Normalize(NormalizationForm.FormD);
            StringBuilder stringReformed = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++)
            {
                Char c = normalizedString[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    stringReformed.Append(c);
            }

            return stringReformed.ToString();
        }

        public static string ConvertTextToHtmlFriendly(this String inputString)
        {
            return inputString
                .Replace("á", "&aacute;")
                .Replace("é", "&eacute;")
                .Replace("í", "&iacute;")
                .Replace("ó", "&oacute;")
                .Replace("ú", "&uacute;")
                .Replace("Á", "&Aacute;")
                .Replace("É", "&Eacute;")
                .Replace("Í", "&Iacute;")
                .Replace("Ó", "&Oacute;")
                .Replace("Ú", "&Uacute;")
                .Replace("Ñ", "&Ntilde;")
                .Replace("ñ", "&ntilde;")
                .Replace("'", "&apos;")
                .Replace("\"", "&quot;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("&", "&amp;");
        }

        public static string RevertTextFromHtmlFriendly(this String inputString)
        {
            return inputString
                .Replace("&aacute;", "á")
                .Replace("&eacute;", "é")
                .Replace("&iacute;", "í")
                .Replace("&oacute;", "ó")
                .Replace("&uacute;", "ú")
                .Replace("&Aacute;", "Á")
                .Replace("&Eacute;", "É")
                .Replace("&Iacute;", "Í")
                .Replace("&Oacute;", "Ó")
                .Replace("&Uacute;", "Ú")
                .Replace("&Uacute;", "Ñ")
                .Replace("&Uacute;", "ñ")
                .Replace("&#225;", "á")
                .Replace("&#233;", "é")
                .Replace("&#237;", "í")
                .Replace("&#243;", "ó")
                .Replace("&#250;", "ú")
                .Replace("&#193;", "Á")
                .Replace("&#201;", "É")
                .Replace("&#205;", "Í")
                .Replace("&#211;", "Ó")
                .Replace("&#218;", "Ú")
                .Replace("&#209;", "Ñ")
                .Replace("&#241;", "ñ")
                .Replace("&apos;", "'")
                .Replace("&quot;", "\"")
                .Replace("&lt;", "<")
                .Replace("&gt;", ">")
                .Replace("&amp;", "&");
        }
    }
}
