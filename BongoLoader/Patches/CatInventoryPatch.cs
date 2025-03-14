using BongoCat;
using BongoCat.SteamJsonParser;
using BongoLoader.BC;
using BongoLoader.Utils;
using HarmonyLib;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static MelonLoader.MelonLogger;

namespace BongoLoader.Patches
{
    [HarmonyPatch(typeof(CatInventory))]
    internal static class CatInventoryPatch
    {
        internal static List<CatItem> _items = new List<CatItem>();
        internal static bool _isInitialized;

        ///

        [HarmonyPostfix]
        [HarmonyPatch(nameof(CatInventory.Start))]
        private static void Start(CatInventory __instance)
        {
            if (ModLoader.Items.Count < 1)
                return;
            MelonCoroutines.Start(Start_Coroutine(__instance));
        }

        private static IEnumerator Start_Coroutine(CatInventory __instance)
        {
            yield return new WaitUntil(() => __instance.IsInitialized);

            InventoryItem inventoryItem = __instance._hatsRoot.GetComponentInChildren<InventoryItem>();

            ///

            foreach (var catItem in ModLoader.Items)
            {
                if (_items.Any(x => x.Id.Equals(catItem.Id)))
                    continue;

                GameObject gameObject = UnityEngine.Object.Instantiate(__instance._inventoryItemPrefab, 
                    catItem.Slot == CatItem.ItemSlot.Hat ? __instance._hatsRoot : __instance._skinsRoot);

                UnityEngine.Object.Destroy(gameObject.GetComponent<InventoryItem>());

                ///

                CatInventoryItem catInventoryItem = gameObject.AddComponent<CatInventoryItem>();
                catInventoryItem.Setup(catItem, inventoryItem);

                _items.Add(catItem);
            }

            _isInitialized = true;

            ///

            __instance._catCosmetics.Validate();
            SortItems(__instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(CatInventory.SortItems))]
        private static bool SortItems(CatInventory __instance)
        {
            MelonCoroutines.Start(SortItems_Coroutine(__instance));
            return false;
        }

        private static IEnumerator SortItems_Coroutine(CatInventory __instance)
        {
            yield return new WaitUntil(() => _isInitialized);

            Component[] hats = __instance._hatsRoot.GetComponentsInChildren<Component>()
                .OrderByDescending(x =>
                {
                    if (x is InventoryItem native)
                        return native.SteamItem.IsFavorite;

                    if (x is CatInventoryItem mod)
                        return mod.CatItem.IsFavorite;

                    return false;
                })
                .ThenByDescending(x =>
                {
                    if (x is InventoryItem native)
                        return native.SteamItem.QualityCategory;

                    if (x is CatInventoryItem mod)
                        return mod.CatItem.Quality;

                    return QualityCategory.Common;
                })
                .ThenByDescending(x => (x as InventoryItem)?.SteamItem.OldestItemTimestamp).ToArray();

            Component[] skins = __instance._skinsRoot.GetComponentsInChildren<Component>()
                .OrderByDescending(x =>
                {
                    if (x is InventoryItem native)
                        return native.SteamItem.IsFavorite;

                    if (x is CatInventoryItem bongo)
                        return bongo.CatItem.IsFavorite;

                    return false;
                })
                .ThenByDescending(x =>
                {
                    if (x is InventoryItem native)
                        return native.SteamItem.QualityCategory;

                    if (x is CatInventoryItem bongo)
                        return bongo.CatItem.Quality;

                    return QualityCategory.Common;
                })
                .ThenByDescending(x => (x as InventoryItem)?.SteamItem.OldestItemTimestamp).ToArray();

            ///

            foreach (Component hat in hats)
            {
                if (hat is InventoryItem native || hat is CatInventoryItem bongo)
                    hat.transform.SetAsLastSibling();
            }

            foreach (Component skin in skins)
            {
                if (skin is InventoryItem native || skin is CatInventoryItem bongo)
                    skin.transform.SetAsLastSibling();
            }

            ///

            if (!__instance._seperator.activeSelf)
                __instance._seperator.SetActive(skins.Length > 0 && hats.Length > 0);
        }
    }
}
