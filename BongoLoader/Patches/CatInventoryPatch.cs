using BongoCat;
using BongoCat.SteamJsonParser;
using BongoLoader.BC;
using BongoLoader.Utils;
using HarmonyLib;
using MelonLoader;
using MelonLoader.TinyJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BongoLoader.Patches
{
    [HarmonyPatch(typeof(CatInventory))]
    public static class BongoInventory
    {
        public static bool IsInitialized { get; private set; }

        public static List<BongoItem> items = new List<BongoItem>();

        public static GameObject modsSeparator;
        public static Transform modsRoot;

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
            yield return new WaitUntil(() => __instance.IsInitialized && __instance._hatsRoot.GetComponentInChildren<InventoryItem>());

            if (!modsRoot)
            {
                modsSeparator = UnityEngine.Object.Instantiate(__instance._seperator, __instance._seperator.transform.parent);
                modsSeparator.name = "Mods Separator";

                modsSeparator.transform.SetAsFirstSibling();
                modsSeparator.SetActive(false);

                ///

                modsRoot = UnityEngine.Object.Instantiate(__instance._hatsRoot.gameObject, __instance._hatsRoot.parent).transform;
                modsRoot.name = "Mods";

                foreach (Transform transform in modsRoot.transform)
                    UnityEngine.Object.Destroy(transform.gameObject);

                modsRoot.SetAsFirstSibling();
            }

            ///

            InventoryItem inventoryItem = __instance._hatsRoot.GetComponentInChildren<InventoryItem>();

            foreach (var catItem in ModLoader.Items)
            {
                if (items.Any(x => x.Id.Equals(catItem.Id)))
                    continue;

                Transform parent = BongoPreferences.DoSeparateModdedItems ? modsRoot.transform : (catItem.Slot == BongoItem.ItemSlot.Hat ? __instance._hatsRoot : __instance._skinsRoot);
                
                GameObject gameObject = UnityEngine.Object.Instantiate(__instance._inventoryItemPrefab, parent);
                UnityEngine.Object.Destroy(gameObject.GetComponent<InventoryItem>());

                BongoInventoryItem catInventoryItem = gameObject.AddComponent<BongoInventoryItem>();
                catInventoryItem.Setup(catItem, inventoryItem);

                items.Add(catItem);
            }

            ///

            IsInitialized = true;
        }

        // call the original method to use the modded methods
        [HarmonyPrefix]
        [HarmonyPatch(nameof(CatInventory.SortItems))]
        private static bool SortItems(CatInventory __instance)
        {
            if (BongoPreferences.DoSeparateModdedItems)
            {
                MelonCoroutines.Start(SortBongoItems());
                return true;
            }

            MelonCoroutines.Start(SortAllItems(__instance));
            return false;
        }

        private static IEnumerator SortBongoItems()
        {
            yield return new WaitUntil(() => IsInitialized);

            BongoInventoryItem[] mods = modsRoot.GetComponentsInChildren<BongoInventoryItem>()
                .OrderByDescending(x => x.CatItem.IsFavorite)
                .ThenByDescending(x => x.CatItem.Quality)
                .ToArray();

            foreach (BongoInventoryItem item in mods)
                item.transform.SetAsLastSibling();

            if (!modsSeparator.activeSelf)
                modsSeparator.SetActive(true);

            ModLoader.Logger.Msg("sorted");
        }

        private static IEnumerator SortAllItems(CatInventory __instance)
        {
            yield return new WaitUntil(() => IsInitialized);

            Component[] hats = __instance._hatsRoot.GetComponentsInChildren<Component>()
                .OrderByDescending(x =>
                {
                    if (x is InventoryItem native)
                        return native.SteamItem.IsFavorite;

                    if (x is BongoInventoryItem mod)
                        return mod.CatItem.IsFavorite;

                    return false;
                })
                .ThenByDescending(x =>
                {
                    if (x is InventoryItem native)
                        return native.SteamItem.QualityCategory;

                    if (x is BongoInventoryItem mod)
                        return mod.CatItem.Quality;

                    return QualityCategory.Common;
                })
                .ThenByDescending(x => (x as InventoryItem)?.SteamItem.OldestItemTimestamp).ToArray();

            Component[] skins = __instance._skinsRoot.GetComponentsInChildren<Component>()
                .OrderByDescending(x =>
                {
                    if (x is InventoryItem native)
                        return native.SteamItem.IsFavorite;

                    if (x is BongoInventoryItem bongo)
                        return bongo.CatItem.IsFavorite;

                    return false;
                })
                .ThenByDescending(x =>
                {
                    if (x is InventoryItem native)
                        return native.SteamItem.QualityCategory;

                    if (x is BongoInventoryItem bongo)
                        return bongo.CatItem.Quality;

                    return QualityCategory.Common;
                })
                .ThenByDescending(x => (x as InventoryItem)?.SteamItem.OldestItemTimestamp).ToArray();

            ///

            foreach (Component hat in hats)
            {
                if (hat is InventoryItem native || hat is BongoInventoryItem bongo)
                    hat.transform.SetAsLastSibling();
            }

            foreach (Component skin in skins)
            {
                if (skin is InventoryItem native || skin is BongoInventoryItem bongo)
                    skin.transform.SetAsLastSibling();
            }

            ///

            if (!__instance._seperator.activeSelf)
                __instance._seperator.SetActive(skins.Length > 0 && hats.Length > 0);
        }
    }
}
