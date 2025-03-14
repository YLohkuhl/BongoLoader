using BongoCat.SteamJsonParser;
using BongoLoader.Patches;
using BongoLoader.Utils;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BongoLoader.BC
{
    /// <summary>
    /// A modded item for Bongo Cat.
    /// </summary>
    public class CatItem : ScriptableObject
    {
        [Serializable]
        public struct ItemInfo
        {
            public string name;
            public QualityCategory quality;
            public bool animated;

            public ItemSlot slot;
            public string path;
        }

        public enum ItemSlot
        {
            Hat,
            Skin
        }

        public string ItemPath => Info.path;

        public BongoMod Mod { get; private set; }
        public ItemInfo Info { get; private set; }

        public string Id { get; private set; }

        public string ItemName { get; set; }
        public Sprite Icon { get; set; }
        public Sprite FullImage { get; set; }

        public ItemSlot Slot { get; set; }
        public QualityCategory Quality { get; set; }

        public bool IsEquipped { get; set; }

        public bool IsAnimated => Info.animated;
        public bool IsFavorite => BongoPrefs.HasFavorite(FavoriteKey);

        public Action OnItemUpdated { get; }

        ///

        internal string FavoriteKey => Id.ToString() + ".fav";

        public void SetFavorite(bool value)
        {
            if (value)
                BongoPrefs.AddFavorite(FavoriteKey);
            else
                BongoPrefs.RemoveFavorite(FavoriteKey);
        }

        ///

        public void Init(BongoMod mod, ItemSlot slot, Sprite icon, Sprite fullImage, ItemInfo info)
        {
            Info = info;
            Id = $"{mod.Id}.{info.name}".ToLower().StripSeparators();

            ItemName = info.name.ToTitleCase();

            Icon = icon;
            FullImage = fullImage;

            Slot = slot;
            Quality = info.quality;

            Register(mod);
        }

        protected void Register(BongoMod mod)
        {
            Mod = mod;

            if (!Mod.CatItems.Any(x => x.Id.Equals(Id)))
                Mod.CatItems = Mod.CatItems.AddToArray(this);
        }
    }
}
