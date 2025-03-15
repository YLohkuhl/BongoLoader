using BongoCat;
using BongoLoader.BC;
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
        [HarmonyPrefix]
        [HarmonyPatch(nameof(CatCosmetics.Validate))]
        private static bool Validate(CatCosmetics __instance)
        {
            if (!BongoPrefs.Has(BongoPrefs.EQUIPPED_KEY))
                return true;

            string text = BongoPrefs.GetString(BongoPrefs.EQUIPPED_KEY);

            if (text.IsNotValid())
                return true;

            string[] equipped = text.Split(BongoPrefs.CHAR_SEPARATOR);
            BongoItem[] items = BongoInventory.items.Where(x => equipped.Contains(x.Id)).ToArray();
            
            if (items.Count(x => x.Slot == BongoItem.ItemSlot.Hat) < 1)
            {
                __instance._hatImage.sprite = null;
                __instance._hatImage.enabled = false;
            }
            if (items.Count(x => x.Slot == BongoItem.ItemSlot.Skin) < 1)
                __instance._cat.SetSkin(null);

            foreach (BongoItem item in items)
                BongoCosmetics.EquipOrUnequipItem(__instance, item, false);

            return false;
        }
    }
}
