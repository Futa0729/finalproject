namespace RPG_Battler.Gameplay
{
    public enum LootRarity { Common, Uncommon, Rare, Epic, Legendary }

    public class LootItem
    {
        public string     Name         { get; set; } = string.Empty;
        public LootRarity Rarity       { get; set; } = LootRarity.Common;
        public string?    CurseEffect  { get; set; }
        public int        PowerBoost   { get; set; } = 0;
    }
}
