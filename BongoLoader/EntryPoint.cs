using MelonLoader;

[assembly: MelonInfo(typeof(BongoLoader.EntryPoint), "BongoLoader", "0.1.0", "YLohkuhl")]
[assembly: MelonGame("Irox Games", "BongoCat")]
[assembly: MelonColor(255, 223, 159, 128)]

namespace BongoLoader
{
    internal class EntryPoint : MelonMod
    {
        public override void OnInitializeMelon()
        {
            ModLoader.Setup();
            ModLoader.Logger.Msg("Loader is fully setup!");
        }

        public override void OnLateInitializeMelon() => ModLoader.LoadMods();
    }
}
