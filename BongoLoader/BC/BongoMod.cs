using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BongoCat;
using BongoLoader.Utils;
using UnityEngine;

namespace BongoLoader.BC
{
    /// <summary>
    /// A mod that can be loaded by BongoLoader.
    /// </summary>
    public class BongoMod
    {
        [Serializable]
        public struct BongoInfo
        {
            public string id;
            public string version;

            public string title;
            public string author;
            public string description;

            public string path;
        }

        public BongoMod(BongoInfo info)
        {
            Info = info;
            CatItems = new BongoItem[] { };
        }

        public string BongoPath => Info.path;

        public BongoInfo Info { get; private set; }

        public string Id => Info.id;
        public string Version => Info.version;

        public string Title => Info.title;
        public string Author => Info.author;
        public string Description => Info.description;

        public BongoItem[] CatItems; 

        ///

        public void Load()
        {
            BongoItem.ItemInfo[] items = BongoSearcher.GetItems(this);

            if (items.IsNull())
                return;

            foreach (var info in items)
            {
                BongoItem item = ScriptableObject.CreateInstance<BongoItem>();
                item.name = info.name;

                item.Init(this, info.slot, null, null, info);
                ModLoader._items.Add(item);
            }
        }
    }
}
