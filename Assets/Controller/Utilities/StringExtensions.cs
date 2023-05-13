using System;
using System.Text;

namespace IdenticalStudios
{
    public static class StringExtensions
    {
        public static string ToUnityLikeNameFormat(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            if (str.Length > 2 && str[0] == 'm' && str[1] == '_')
                str = str.Remove(0, 2);

            if (str.Length > 1 && str[0] == '_')
                str = str.Remove(0);

            StringBuilder newText = new StringBuilder(str.Length * 2);
            newText.Append(str[0]);

            for (int i = 1; i < str.Length; i++)
            {
                bool lastIsUpper = char.IsUpper(str[i - 1]);
                bool lastIsSpace = str[i - 1] == ' ';
                bool lastIsDigit = char.IsDigit(str[i - 1]);

                if (char.IsUpper(str[i]) && !lastIsUpper && !lastIsSpace)
                    newText.Append(' ');

                if (char.IsDigit(str[i]) && !lastIsDigit && !lastIsUpper && !lastIsSpace)
                    newText.Append(' ');

                newText.Append(str[i]);
            }

            return newText.ToString();
        }

        public static int DamerauLevenshteinDistanceTo(this string thisString, string targetString)
        {
            return DamerauLevenshteinDistance(thisString, targetString);
        }

        public static int DamerauLevenshteinDistance(string string1, string string2)
        {
            if (string.IsNullOrEmpty(string1))
            {
                if (!string.IsNullOrEmpty(string2))
                    return string2.Length;

                return 0;
            }

            if (string.IsNullOrEmpty(string2))
            {
                if (!string.IsNullOrEmpty(string1))
                    return string1.Length;

                return 0;
            }

            int length1 = string1.Length;
            int length2 = string2.Length;

            int[,] d = new int[length1 + 1, length2 + 1];

            int cost, del, ins, sub;

            for (int i = 0; i <= d.GetUpperBound(0); i++)
                d[i, 0] = i;

            for (int i = 0; i <= d.GetUpperBound(1); i++)
                d[0, i] = i;

            for (int i = 1; i <= d.GetUpperBound(0); i++)
            {
                for (int j = 1; j <= d.GetUpperBound(1); j++)
                {
                    if (string1[i - 1] == string2[j - 1])
                        cost = 0;
                    else
                        cost = 1;

                    del = d[i - 1, j] + 1;
                    ins = d[i, j - 1] + 1;
                    sub = d[i - 1, j - 1] + cost;

                    d[i, j] = Math.Min(del, Math.Min(ins, sub));

                    if (i > 1 && j > 1 && string1[i - 1] == string2[j - 2] && string1[i - 2] == string2[j - 1])
                        d[i, j] = Math.Min(d[i, j], d[i - 2, j - 2] + cost);
                }
            }

            return d[d.GetUpperBound(0), d.GetUpperBound(1)];
        }
    }
}