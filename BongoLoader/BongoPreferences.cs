using MelonLoader;
using MelonLoader.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BongoLoader
{
    //public static class BongoPreferences
    //{
    //    public static bool IsSetup { get; private set; }

    //    ///

    //    public static bool DoNotLoadMods { get; private set; }

    //    public static bool DoAnimateItems { get; private set; }
    //    public static bool DoSeparateModdedItems { get; private set; }

    //    ///

    //    internal static MelonPreferences_Category _loader;
    //    internal static MelonPreferences_Category _mods;

    //    public static void Setup()
    //    {
    //        _loader = MelonPreferences.CreateCategory("Loader");
    //        _mods = MelonPreferences.CreateCategory("Mods");

    //        _loader.SetFilePath(Path.Combine(MelonEnvironment.UserDataDirectory, "BongoPreferences.cfg"), false);
    //        _mods.SetFilePath(Path.Combine(MelonEnvironment.UserDataDirectory, "BongoPreferences.cfg"), false);

    //        ///

    //        DoNotLoadMods = _loader.CreateEntry("bl_load_mods", false,
    //            "do_not_load_mods", "Prevents BongoLoader from loading mods on startup.").Value;

    //        DoAnimateItems = _mods.CreateEntry("bl[mods]_animate_items", true,
    //            "do_animate_items", "If toggled off, any animated items will use a static image. This can either be a provided fallback, or the first frame.").Value;
    //        DoSeparateModdedItems = _mods.CreateEntry("bl[mods]_separate_modded_items", true,
    //            "do_separate_modded_items", "If toggled off, modded items will be mixed in with native (game) items rather than separated.").Value;

    //        ///

    //        _loader.LoadFromFile();
    //        _mods.LoadFromFile();

    //        IsSetup = true;
    //    }
    //}
}
