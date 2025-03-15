using BongoCat;
using BongoLoader.BC;
using BongoLoader.Utils;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BongoLoader.Patches
{
    [HarmonyPatch(typeof(CatCosmetics), nameof(CatCosmetics.EquipItem))]
    internal static class CatCosmeticsEquipItemPatch
    {
        private static void Prefix(CatCosmetics __instance, SteamItem steamItem)
        {
            if (BongoCosmetics.equipped.Count > 0)
            {
                string capital = steamItem.ItemSlot.ToTitleCase();

                if (!Enum.TryParse(capital, out BongoItem.ItemSlot slot))
                {
                    ModLoader.Logger.Error($"Failed to parse '{capital}' to '{slot.GetType().Name}' enum.");
                    return;
                }

                BongoItem item = BongoCosmetics.equipped.Find(x => x.IsEquipped && x.Slot == slot);

                if (item.IsNotNull())
                    BongoCosmetics.EquipOrUnequipItem(__instance, item, true, true);
            }
        }
    }
}
