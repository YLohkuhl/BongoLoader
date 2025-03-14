using BongoLoader.BC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BongoLoader.Utils
{
    public static class BongoSearcher
    {
        public const string BONGO_FILENAME = "bongo.json";
        public const string ITEM_FILENAME = "item.json";

        /// <summary>
        /// Searches for all "Bongos" (mods) in the mods directory. Returns null if no bongos are found.
        /// </summary>
        /// <returns><see cref="BongoInfo"/>[]</returns>
        public static BongoMod.BongoInfo[] GetBongos()
        {
            DirectoryInfo dir = new DirectoryInfo(BongoEnvironment.ModsDirectory);

            if (!dir.Exists)
            {
                ModLoader.Logger.Error("Failed to locate the mods directory, unable to load any mods.");
                return null;
            }

            List<BongoMod.BongoInfo> bongos = new List<BongoMod.BongoInfo>();

            foreach (var file in dir.EnumerateFiles(BONGO_FILENAME, SearchOption.AllDirectories))
            {
                using (StreamReader reader = file.OpenText())
                {
                    string text = reader.ReadToEnd();

                    if (text.IsNotValid())
                        continue;

                    try
                    {
                        BongoMod.BongoInfo info = JsonConvert.DeserializeObject<BongoMod.BongoInfo>(text);
                        info.id = info.id.StripSeparators();
                        info.title = info.title.ToTitleCase();

                        if (!info.id.ValidateAndLog("Bongo Identifier", file.FullName))
                            continue;
                        if (!info.version.ValidateAndLog("Bongo Version", file.FullName))
                            continue;
                        if (!info.title.ValidateAndLog("Bongo Title", file.FullName))
                            continue;
                        if (!info.author.ValidateAndLog("Bongo Author", file.FullName))
                            continue;

                        info.path = file.Directory.FullName;
                        bongos.Add(info);
                    }
                    catch (JsonSerializationException e)
                    {
                        ModLoader.Logger.Error($"Failed to parse {BONGO_FILENAME}: {file.FullName}", e);
                    }
                }
            }

            return bongos.Count > 0 ? bongos.ToArray() : null;
        }

        /// <summary>
        /// Searches for all items in the specified mod. Returns null if no items are found.
        /// </summary>
        /// <param name="mod">The mod to search items in.</param>
        /// <returns><see cref="ItemInfo"/>[]</returns>
        public static CatItem.ItemInfo[] GetItems(BongoMod mod)
        {
            if (mod.IsNull())
                return null;

            DirectoryInfo[] modDirs = new DirectoryInfo[]
            {
                new DirectoryInfo(Path.Combine(mod.BongoPath, "hats")),
                new DirectoryInfo(Path.Combine(mod.BongoPath, "skins"))
            };

            List<CatItem.ItemInfo> items = new List<CatItem.ItemInfo>();

            foreach (var dir in modDirs)
            {
                if (!dir.Exists)
                    continue;

                foreach (var file in dir.EnumerateFiles(ITEM_FILENAME, SearchOption.AllDirectories))
                {
                    using (StreamReader reader = file.OpenText())
                    {
                        string text = reader.ReadToEnd();

                        if (text.IsNotValid())
                            continue;

                        try
                        {
                            CatItem.ItemInfo info = JsonConvert.DeserializeObject<CatItem.ItemInfo>(text);
                            info.name = info.name.ToTitleCase();

                            if (!info.name.ValidateAndLog("Item Name", file.FullName))
                                continue;

                            string location = file.Directory.FullName.Substring(mod.BongoPath.Length + 1);

                            switch (location)
                            {
                                case string loc when loc.StartsWith("hats"):
                                    info.slot = CatItem.ItemSlot.Hat;
                                    break;

                                case string loc when loc.StartsWith("skins"):
                                    info.slot = CatItem.ItemSlot.Skin;
                                    break;

                                default:
                                    info.slot = CatItem.ItemSlot.Hat;
                                    break;
                            }

                            info.path = file.Directory.FullName;
                            items.Add(info);
                        }
                        catch (JsonSerializationException e)
                        {
                            ModLoader.Logger.Error($"Failed to parse {ITEM_FILENAME}: {file.FullName}", e);
                        }
                    }
                }
            }

            return items.Count > 0 ? items.ToArray() : null;
        }
    }
}
