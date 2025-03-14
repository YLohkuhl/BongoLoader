using BongoLoader.Utils;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BongoLoader.Patches
{
    /// <summary>
    /// A patch for <see cref="PlayerPrefs"/>, but also, the loader miniture save system.
    /// </summary>
    [HarmonyPatch(typeof(PlayerPrefs))]
    public static class BongoPrefs
    {
        public const string EQUIPPED_KEY = "EQUIPPED_MODDED_ITEMS";
        public const string ONCE_EQUIPPED_KEY = "ONCE_EQUIPPED_MODDED_ITEMS";

        public const string STR_SEPARATOR = ";";
        public const char CHAR_SEPARATOR = ';';

        #region PATCH

        [HarmonyPostfix]
        [HarmonyPatch(nameof(PlayerPrefs.DeleteKey))]
        private static void DeleteKey(string key) => Delete(key);

        ///

        [HarmonyPostfix]
        [HarmonyPatch(nameof(PlayerPrefs.SetInt))]
        private static void SetInt(string key, int value) => Set(key, value);

        [HarmonyPostfix]
        [HarmonyPatch(nameof(PlayerPrefs.SetFloat))]
        private static void SetFloat(string key, float value) => Set(key, value);

        [HarmonyPostfix]
        [HarmonyPatch(nameof(PlayerPrefs.SetString))]
        private static void SetString(string key, string value) => Set(key, value);

        ///

        [HarmonyPrefix]
        [HarmonyPatch(nameof(PlayerPrefs.GetInt), new Type[] { typeof(string), typeof(int) })]
        private static bool GetInt(string key, int defaultValue, ref int __result)
        {
            if (!Has(key))
                return true; // Retreat to the original method

            __result = GetInt(key, defaultValue);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(PlayerPrefs.GetFloat), new Type[] { typeof(string), typeof(float) })]
        private static bool GetFloat(string key, float defaultValue, ref float __result)
        {
            if (!Has(key))
                return true;

            __result = GetFloat(key, defaultValue);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(PlayerPrefs.GetString), new Type[] { typeof(string), typeof(string) })]
        private static bool GetString(string key, string defaultValue, ref string __result)
        {
            if (!Has(key))
                return true;

            __result = GetString(key, defaultValue);
            return false;
        }

        #endregion

        #region BONGO

        ///

        public static string GetFilePath(string key) => Path.Combine(BongoEnvironment.PrefsDirectory, key);

        public static string GetFavoriteFilePath(string key) => Path.Combine(BongoEnvironment.FavoritePrefsDirectory, key);

        ///

        public static bool HasFavorite(string favKey) => File.Exists(GetFavoriteFilePath(favKey));

        public static void RemoveFavorite(string favKey) => File.Delete(GetFavoriteFilePath(favKey));

        public static void AddFavorite(string favKey) => File.WriteAllText(GetFavoriteFilePath(favKey), "");

        ///

        public static bool Has(string key) => File.Exists(GetFilePath(key));

        public static void Delete(string key) => File.Delete(GetFilePath(key));

        public static void Set(string key, object value) => File.WriteAllText(GetFilePath(key), value.ToString());

        ///

        public static int GetInt(string key, int defaultValue = 0)
        {
            string text = File.ReadAllText(GetFilePath(key));

            if (text.IsNotValid())
                return defaultValue;

            return int.Parse(text);
        }

        public static float GetFloat(string key, float defaultValue = 0f)
        {
            string text = File.ReadAllText(GetFilePath(key));

            if (text.IsNotValid())
                return defaultValue;

            return float.Parse(text);
        }

        public static string GetString(string key, string defaultValue = "")
        {
            string text = File.ReadAllText(GetFilePath(key));

            if (text.IsNotValid())
                return defaultValue;

            return text;
        }

        #endregion
    }
}
