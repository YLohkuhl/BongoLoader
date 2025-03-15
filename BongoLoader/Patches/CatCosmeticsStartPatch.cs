using BongoCat;
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

namespace BongoLoader.Patches
{
    [HarmonyPatch(typeof(CatCosmetics), nameof(CatCosmetics.Start))]
    public static class BongoCosmetics
    {
        public static List<BongoItem> equipped = new List<BongoItem>();

        private static void Postfix(CatCosmetics __instance, ref IEnumerator __result) => MelonCoroutines.Start(Start_Coroutine(__instance, __result));

        private static IEnumerator Start_Coroutine(CatCosmetics __instance, IEnumerator __result)
        {
            while (__result.MoveNext())
                yield return __result.Current;

            if (BongoPrefs.Has(BongoPrefs.EQUIPPED_KEY))
            {
                string text = BongoPrefs.GetString(BongoPrefs.EQUIPPED_KEY);

                if (text.IsNotValid())
                    yield break;

                string[] split = text.Split(BongoPrefs.CHAR_SEPARATOR);

                foreach (var item in ModLoader._items)
                {
                    if (split.Contains(item.Id))
                        EquipOrUnequipItem(__instance, item);
                }
            }
        }

        public static void EquipOrUnequipItem(CatCosmetics cosmetics, BongoItem catItem, bool playAnimation = true, bool unequipIfSameItemIsEquipped = false)
        {
            if (catItem.IsNull())
            {
                ModLoader.Logger.Msg($"Unable to equip item that is null: {catItem.Id}");
                return;
            }

            BongoItem.ItemSlot itemSlot = catItem.Slot;

            if (!equipped.Any(x => x.Id.Equals(catItem.Id)) || !unequipIfSameItemIsEquipped)
            {
                switch (itemSlot)
                {
                    case BongoItem.ItemSlot.Hat:
                        {
                            cosmetics._hatImage.sprite = catItem.FullImage;
                            cosmetics._hatImage.enabled = true;

                            cosmetics.UpdateFlip();

                            if (playAnimation)
                            {
                                if (cosmetics._hatImage.TryGetComponent(out OpenScaleAnimation component))
                                {
                                    bool flag = cosmetics._flipVisuals.IsFlipped && cosmetics._cache.LetterSprites.Contains(cosmetics._hatImage.sprite);
                                    component.PlayAnimation(flag ? new Vector2(-1f, 1f) : Vector2.one);
                                }
                            }
                            break;
                        }

                    case BongoItem.ItemSlot.Skin:
                        cosmetics._cat.SetSkin(catItem.ItemName);
                        break;
                }

                catItem.IsEquipped = true;
                catItem.OnItemUpdated?.Invoke();
                equipped.Add(catItem);

                ///

                // Unequip modded AND native items
                foreach (var item in equipped)
                {
                    if (item.Slot == catItem.Slot && !item.Id.Equals(catItem.Id))
                    {
                        item.IsEquipped = false;
                        item.OnItemUpdated?.Invoke();
                    }
                }

                equipped.RemoveAll(x => x.Slot == catItem.Slot && !x.Id.Equals(catItem.Id));

                foreach (var item in cosmetics._equippedItems)
                {
                    if (item.ItemSlot == catItem.Slot.ToString().ToLower())
                    {
                        item.IsEquipped = false;
                        item.OnItemUpdated?.Invoke();
                    }
                }

                cosmetics._equippedItems.RemoveAll(x => x.ItemSlot == catItem.Slot.ToString().ToLower());

                ///

                BongoPrefs.Set(BongoPrefs.EQUIPPED_KEY, string.Join(BongoPrefs.STR_SEPARATOR, equipped.Select(x => x.Id).ToArray()));
                return;
            }

            equipped.RemoveAll(x => x.Id.Equals(catItem.Id));
            BongoPrefs.Set(BongoPrefs.EQUIPPED_KEY, string.Join(BongoPrefs.STR_SEPARATOR, equipped.Select(x => x.Id).ToArray()));

            switch (catItem.Slot)
            {
                case BongoItem.ItemSlot.Hat:
                    cosmetics._hatImage.sprite = null;
                    cosmetics._hatImage.enabled = cosmetics._hatImage.sprite.IsNotNull();
                    break;

                case BongoItem.ItemSlot.Skin:
                    cosmetics._cat.SetSkin(null);
                    break;
            }

            catItem.IsEquipped = false;
            catItem.OnItemUpdated?.Invoke();
        }
    }
}
