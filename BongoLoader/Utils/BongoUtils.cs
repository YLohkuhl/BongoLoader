using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BongoLoader.Utils
{
    public static class BongoUtils
    {
        public const string ICON_FILENAME = "icon.png";
        public const string FULL_IMAGE_FILENAME = "fullImage.png";

        public static Texture2D LoadLocalImage(string name, string path)
        {
            if (!File.Exists(path))
                return null;

            byte[] bytes = File.ReadAllBytes(path);

            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false)
            {
                name = name,
                hideFlags = HideFlags.DontUnloadUnusedAsset,
                filterMode = FilterMode.Bilinear
            };

            texture.LoadImage(bytes);
            return texture;
        }

        public static bool ValidateAndLog(this string str, string name, string path)
        {
            if (str.IsNotValid())
            {
                ModLoader.Logger.Error($"'{name}' cannot be null, empty or whitespace: {path}");
                return false;
            }
            return true;
        }
    }
}
