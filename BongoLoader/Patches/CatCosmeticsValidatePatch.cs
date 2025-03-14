using BongoCat;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BongoLoader.Patches
{
    [HarmonyPatch(typeof(CatCosmetics))]
    internal static class CatCosmeticsValidatePatch
    {
        //[HarmonyPrefix]
        //[HarmonyPatch(nameof(CatCosmetics.Validate))]
        //private static void Validate(CatCosmetics __instance)
        //{
        //    if (!PlayerPrefs.HasKey("EQUIPPED_ITEMS"))
        //    {
        //        return;
        //    }
        //    string s = PlayerPrefs.GetString("EQUIPPED_ITEMS");
        //    if (string.IsNullOrEmpty(s))
        //    {
        //        return;
        //    }
        //    List<int> ids = Enumerable.ToList<int>(Enumerable.Select<string, int>(s.Split(',', StringSplitOptions.None), new Func<string, int>(int.Parse)));
        //    List<SteamItem> list = Enumerable.ToList<SteamItem>(Enumerable.Where<SteamItem>(this._catInventory.Items, (SteamItem i) => ids.Contains(i.SteamItemDefId)));
        //    if (Enumerable.Count<SteamItem>(list, (SteamItem i) => i.ItemSlot == "hat") == 0)
        //    {
        //        this._hatImage.sprite = null;
        //        this._hatImage.enabled = false;
        //    }
        //    if (Enumerable.Count<SteamItem>(list, (SteamItem i) => i.ItemSlot == "skin") == 0)
        //    {
        //        this._cat.SetSkin(null);
        //    }
        //    foreach (SteamItem item in list)
        //    {
        //        this.EquipItem(item, false, false);
        //    }
        //}
    }
}
