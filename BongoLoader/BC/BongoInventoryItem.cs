using BongoCat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using BongoLoader.Patches;
using BongoLoader.Utils;

namespace BongoLoader.BC
{
    public class BongoInventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public BongoItem CatItem { get; private set; }

        protected CatCosmetics _catCosmetics;
        protected CatInventory _catInventory;
        protected ItemExchange _itemExchange;

        protected QualityColors _qualityColors;

        protected Image _itemImage;
        protected Image _borderImage;

        protected Image _background;
        protected Image _newIndicator;

        protected Image _favoriteImage;
        protected Image _favoriteHover;

        protected Color _equippedColor;
        protected Color _unequippedColor;

        // stripped games suck btw (why must i get child so much ughh)
        public void Setup(BongoItem catItem, InventoryItem inventoryItem)
        {
            GetComponent<Button>().onClick.AddListener(OnClick);

            _itemImage = transform.GetChild(2).GetChild(0).GetComponent<Image>();
            _borderImage = transform.GetChild(1).GetComponent<Image>();

            _background = transform.GetChild(0).GetComponent<Image>();
            _newIndicator = transform.GetChild(5).GetComponent<Image>();

            _favoriteImage = transform.GetChild(8).GetComponent<Image>();
            _favoriteHover = transform.GetChild(7).GetComponent<Image>();

            _favoriteImage.GetComponent<Button>().onClick.AddListener(ToggleFavorite);
            _favoriteHover.GetComponent<Button>().onClick.AddListener(ToggleFavorite);

            ///

            string[] dontDestroy = new string[]
            {
                _itemImage.transform.parent.name,
                _borderImage.transform.name,

                _background.transform.name,
                _newIndicator.transform.name,

                _favoriteImage.transform.name,
                _favoriteHover.transform.name
            };

            foreach (Transform transform in transform)
            {
                if (dontDestroy.Contains(transform.name))
                    continue;
                Destroy(transform.gameObject);
            }

            ///

            CopyFrom(inventoryItem);
            SetItem(catItem);
        }

        public void CopyFrom(InventoryItem inventoryItem)
        {
            //for (int i = 0; i < gameObject.transform.childCount; i++)
            //{
            //    var c = inventoryItem.transform.GetChild(i);
            //    if (c.IsNotNull())
            //        ModLoader.Logger.Msg(c.name + " " + i);
            //}

            //foreach (Transform transform in inventoryItem.transform)
            //{
            //    ModLoader.Logger.WriteLine();
            //    ModLoader.Logger.Msg(transform.name);
            //    ModLoader.Logger.WriteLine();

            //    foreach (Transform t in transform)
            //    {
            //        ModLoader.Logger.WriteLine();
            //        ModLoader.Logger.Msg(t.name);
            //        ModLoader.Logger.WriteLine();

            //        foreach (Component component in t.GetComponents<Component>())
            //            ModLoader.Logger.Msg(component.GetType().Name);

            //        ModLoader.Logger.WriteLine();
            //    }

            //    foreach (Component component in transform.GetComponents<Component>())
            //        ModLoader.Logger.Msg(component.GetType().Name);
            //}

            _qualityColors = inventoryItem._colors;
            _equippedColor = inventoryItem._unequippedColor;
            _unequippedColor = inventoryItem._unequippedColor;
        }

        public void SetItem(BongoItem catItem)
        {
            if (_catCosmetics.IsNull())
            {
                _catCosmetics = FindAnyObjectByType<CatCosmetics>(FindObjectsInactive.Include);
                _catInventory = FindAnyObjectByType<CatInventory>(FindObjectsInactive.Include);
                _itemExchange = FindAnyObjectByType<ItemExchange>(FindObjectsInactive.Include);
            }

            CatItem = catItem;
            CatItem.OnItemUpdated = (Action)Delegate.Combine(CatItem.OnItemUpdated, new Action(UpdateItem));
            UpdateItem();
        }

        ///

        public void UpdateItem()
        {
            _itemImage.sprite = CatItem.Icon;
            _borderImage.color = _qualityColors.GetColor(CatItem.Quality);

            string[] onceEquipped = BongoPrefs.GetString(BongoPrefs.ONCE_EQUIPPED_KEY).Split(BongoPrefs.CHAR_SEPARATOR);

            _background.color = CatItem.IsEquipped ? _equippedColor : _unequippedColor;
            _newIndicator.gameObject.SetActive(!onceEquipped.Contains(CatItem.Id));

            _favoriteImage.enabled = CatItem.IsFavorite;
            _favoriteHover.enabled = false;
        }

        public void OnClick()
        {
            if (_itemExchange.IsVisible())
                return;

            BongoCosmetics.EquipOrUnequipItem(_catCosmetics, CatItem, true, true);
            _newIndicator.gameObject.SetActive(false);

            Shop.Instance.HideTimer();

            if (ModLoader.OnceEquipped.Add(CatItem.Id))
                BongoPrefs.Set(BongoPrefs.ONCE_EQUIPPED_KEY, string.Join(BongoPrefs.STR_SEPARATOR, ModLoader.OnceEquipped));
        }

        public void ToggleFavorite()
        {
            if (_itemExchange.IsVisible())
                return;

            CatItem.SetFavorite(!CatItem.IsFavorite);
            _favoriteImage.enabled = CatItem.IsFavorite;

            _catInventory.SortItems();
        }

        ///

        public void OnDestroy()
        {
            if (CatItem.IsNull())
                return;
            CatItem.OnItemUpdated = (Action)Delegate.Remove(CatItem.OnItemUpdated, new Action(UpdateItem));
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_itemExchange.IsVisible() || _favoriteImage.enabled)
                return;
            _favoriteHover.enabled = true;
        }

        public void OnPointerExit(PointerEventData eventData) => _favoriteHover.enabled = false;
    }
}
