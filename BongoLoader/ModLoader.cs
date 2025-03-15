using BongoCat;
using BongoLoader.BC;
using BongoLoader.Patches;
using BongoLoader.Utils;
using MelonLoader;
using MelonLoader.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BongoLoader
{
    public static class ModLoader
    {
        public static bool IsSetup { get; private set; }
        public static bool HasLoadedMods { get; private set; }

        public static MelonLogger.Instance Logger { get; private set; }
        public static HashSet<string> OnceEquipped => _onceEquipped;

        public static List<BongoMod> Bongos => _bongos;
        public static List<BongoItem> Items => _items;

        ///

        internal static HashSet<string> _onceEquipped = new HashSet<string>();

        internal static List<BongoMod> _bongos = new List<BongoMod>();
        internal static List<BongoItem> _items = new List<BongoItem>();

        ///

        /// <summary>
        /// Calls the necessary methods to setup BongoLoader.
        /// </summary>
        public static void Setup()
        {
            if (IsSetup)
            {
                Logger.Msg("BongoLoader has already been called to setup. You cannot call setup more than once.");
                return;
            }

            Logger = new MelonLogger.Instance("BongoLoader", ColorARGB.Salmon);
            SetupEnvironment();

            IsSetup = true;
        }

        /// <summary>
        /// Calls the necessary methods to setup BongoLoader after initial setup.
        /// </summary>
        public static void PostSetup()
        {
            if (!IsSetup)
            {
                Logger.Error("BongoLoader has not been setup yet. Post setup cannot be called without setup.");
                return;
            }

            if (!HasLoadedMods)
            {
                Logger.Error("BongoLoader has not loaded any mods yet. Post setup cannot be called without loading mods.");
                return;
            }

            if (BongoPrefs.Has(BongoPrefs.ONCE_EQUIPPED_KEY))
            {
                string text = BongoPrefs.GetString(BongoPrefs.ONCE_EQUIPPED_KEY);

                if (text.IsValid())
                {
                    List<string> split = text.Split(BongoPrefs.CHAR_SEPARATOR).ToList();
                    List<string> remove = new List<string>();

                    foreach (string item in split)
                    {
                        if (!item.IsValid())
                            continue;

                        if (!_items.Any(x => x.Id.Equals(item)))
                            remove.Add(item);
                        else
                            OnceEquipped.Add(item);
                    }

                    foreach (string item in remove)
                        split.Remove(item);

                    BongoPrefs.Set(BongoPrefs.ONCE_EQUIPPED_KEY, string.Join(BongoPrefs.STR_SEPARATOR, split));
                }
            }
        }

        /// <summary>
        /// Initiates the loading process for Bongos (mods).
        /// </summary>
        internal static void LoadMods()
        {
            if (!IsSetup)
            {
                Logger.Error("BongoLoader has not been setup yet. Loading mods cannot be called without setup.");
                return;
            }

            if (HasLoadedMods)
            {
                Logger.Error("BongoLoader has already been called to load mods. You cannot load mods more than once.");
                return;
            }

            Logger.WriteSpacer();
            Logger.Msg("Loading Bongos..");

            int count = 0;

            foreach (var bongo in BongoSearcher.GetBongos())
            {
                Logger.WriteLine(ColorARGB.Magenta);

                Logger.Msg(ColorARGB.DarkGray, $"Bongo Identifier: '{bongo.id}'");
                Logger.Msg(ColorARGB.DarkGray, $"Bongo Version: v{bongo.version}");
                Logger.WriteSpacer();

                Logger.WriteLine(ColorARGB.Green);

                Logger.Msg(ColorARGB.LightSalmon, bongo.title);
                Logger.Msg($"by {bongo.author}");

                Logger.WriteLine(ColorARGB.Green);

                if (bongo.description.IsValid())
                    Logger.Msg(ColorARGB.DarkGray, $"\"{bongo.description}\"");

                Logger.WriteLine(ColorARGB.Magenta);

                ///

                BongoMod mod = new BongoMod(bongo);
                mod.Load();
                Bongos.Add(mod);

                count++;
            }

            Logger.Msg(string.Format("{0} {1} loaded.", count, count > 1 ? "Bongos" : "Bongo"));
            Logger.WriteSpacer();

            HasLoadedMods = true;
        }

        /// <summary>
        /// Setting up the Bongo environment. This is done by creating the necessary directories for the loader (and game) to function.
        /// </summary>
        internal static void SetupEnvironment()
        {
            Logger.Msg("Setting up environment..");

            string[] directories = new string[]
            {
                BongoEnvironment.LoaderDirectory,
                BongoEnvironment.PersistentDirectory,
                BongoEnvironment.ModsDirectory,
                BongoEnvironment.PrefsDirectory,
                BongoEnvironment.FavoritePrefsDirectory
            };

            foreach (var dir in directories)
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);

                    if (Directory.Exists(dir))
                        Logger.Msg($"Successfully created Bongo directory: {dir}");
                    else
                        Logger.Error($"Failed to create Bongo directory: {dir}");
                }
            }
        }

        ///// <summary>
        ///// Extracts the embedded libraries. This is done by extracting the libraries from the embedded resources to the libraries directory.
        ///// </summary>
        //private static void ExtractEmbeddedLibraries()
        //{
        //    string basePath = "BongoLoader.Embedded.Libraries";

        //    string[] libPaths = new string[]
        //    {
        //        basePath,
        //        $"{basePath}.UnstrippedCorlibs"
        //    };

        //    ///

        //    Logger.Msg("Attempting to Extract Embedded Libraries...");

        //    foreach (string p in libPaths)
        //    {
        //        foreach (string n in Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(x => x.StartsWith(p)))
        //        {
        //            string dirname = p.Substring(basePath.Length).Replace(".", "");
        //            string filename = n.Substring(p.Length).Replace(".", "");

        //            using (var res = Assembly.GetExecutingAssembly().GetManifestResourceStream(n))
        //            {
        //                var filePath = Path.Combine(BongoEnvironment.LibrariesDirectory, dirname, filename);

        //                if (File.Exists(filePath))
        //                    continue;

        //                using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        //                {
        //                    res.CopyTo(file);

        //                    if (File.Exists(filePath))
        //                        Logger.Msg($"Embedded Library Extracted: {dirname}/{filename}");
        //                    else
        //                        Logger.Error($"Failed to Extract Embedded Library: {dirname}/{filename}");
        //                }
        //            }
        //        }
        //    }
        //}
    }
}
