using MelonLoader.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BongoLoader.Utils
{
    /// <summary>
    /// Directories used by BongoLoader. Otherwise known as the Bongo Environment.
    /// </summary>
    public static class BongoEnvironment
    {
        public static string LoaderDirectory { get; } = Path.Combine(MelonEnvironment.ModsDirectory, "BongoLoader");
        public static string PersistentDirectory { get; } = Path.Combine(Application.persistentDataPath, "BongoLoader");

        public static string ModsDirectory { get; } = Path.Combine(LoaderDirectory, "Mods");
        public static string PrefsDirectory { get; } = Path.Combine(PersistentDirectory, "Prefs");

        public static string FavoritePrefsDirectory { get; } = Path.Combine(PrefsDirectory, "Favorite");
    }
}
