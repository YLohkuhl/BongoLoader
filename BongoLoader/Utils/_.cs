
using BongoLoader.Patches;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using UnityEngine;

namespace BongoLoader.Utils
{
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


        // public static T Get<T>(string name) where T : Object => Resources.FindObjectsOfTypeAll<T>().FirstOrDefault(found => found.name.Equals(name));

        //public static Sprite ToSprite(this Texture2D texture)
        //{
        //    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        //    sprite.hideFlags |= HideFlags.HideAndDontSave;
        //    sprite.name = texture.name;
        //    return sprite;
        //}
    }
}
