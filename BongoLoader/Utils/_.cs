
using BongoLoader.Patches;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using UnityEngine;

public static class _
{
    public static bool IsNull(this object obj) => obj == null;

    public static bool IsNotNull(this object obj) => !IsNull(obj);

    public static bool IsValid(this string str) => !string.IsNullOrEmpty(str) || !string.IsNullOrWhiteSpace(str);

    public static bool IsNotValid(this string str) => !IsValid(str);

    public static string ToTitleCase(this string str) => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());

    public static string StripSeparators(this string str)
    {
        return str
            .Replace(" ", "")
            .Replace(BongoPrefs.STR_SEPARATOR, "")
            .Replace(",", "");
    }
}
