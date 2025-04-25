// Gameplay/LootBox.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;  
using System.Threading.Tasks;
using RPG_Battler.Character;
using RPG_Battler.Character.Upgrades;   

namespace RPG_Battler.Gameplay
{
    public class LootBox
    {
        private static readonly Random _rng = new();
        private static List<LootItem>? _savedTable;

        public static async Task<LootItem?> OpenAsync(Hero hero)
        {
            // path to JSON
            const string TABLE_PATH =
                "/Users/nishidafuta/final-battle-Futa0729-4/RPG Battler/Gameplay/LootTable.json";

            if (_savedTable == null)
            {
                _savedTable = await LoadTableAsync(TABLE_PATH);
            }
            var table = _savedTable;

            if (table.Count == 0) return null;

            // weighted roll (luck affects odds)
            int Weight(LootItem item)
            {
                switch (item.Rarity)
                {
                    case LootRarity.Common: return 60;
                    case LootRarity.Uncommon: return 40;
                    case LootRarity.Rare: return 20;
                    case LootRarity.Epic: return 8;
                    case LootRarity.Legendary: return 2;
                    default: return 60;
                }
            }

            var weighted = new List<(LootItem Item, int Weight)>();

            int total = 0;

            foreach (var item in table)
            {
                int weight = Weight(item) + hero.TotalLuck / 5;
                weighted.Add((item, weight));
                total += weight;
            }

            int roll  = _rng.Next(1, total + 1); // get a random number between 1 and total

            //// find the item that corresponds to the roll adding the weights one by one
            // if the sum exceeds the roll, return that item
            LootItem loot = new LootItem();
            int runningTotal = 0;

            foreach (var pair in weighted)
            {
                runningTotal += pair.Weight;
                if (roll <= runningTotal)
                {
                    loot = pair.Item;
                    break;
                }
            }

            ApplyEffects(hero, loot);
            return loot;
        }

        //// loads JSON from the absolute path(above)
        private static async Task<List<LootItem>> LoadTableAsync(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("⚠️  Loot table not found at: " + path);
                return new List<LootItem>();
            }

            string json = await File.ReadAllTextAsync(path);

            // add a converter for enums stored as strings
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            options.Converters.Add(new JsonStringEnumConverter());   //  ← 1 new line

            return JsonSerializer.Deserialize<List<LootItem>>(json, options)
                    ?? new List<LootItem>();
        }

        // side-effects & curses
        private static void ApplyEffects(Hero hero, LootItem loot)
        {
            
            Item item = new Item(loot.Name, "Looted (" + loot.Rarity + ")");
            item.Quantity = 1;

            // put curse them randomly
            bool IfCurse = string.IsNullOrEmpty(loot.CurseEffect) && _rng.NextDouble() < 0.1;
            if(IfCurse)
            {
                item.IsCursed = true;
                item.ItemName = "Cursed " + item.ItemName;
                item.Description += " 💀 Cursed!";
                item.CurseEffectDescription = "HP -10 when used";
            }

            hero.Items.Add(item);
            hero.TotalPower += loot.PowerBoost;

            // Curse effect from LootItem (from JSON)
            switch (loot.CurseEffect)
            {
                case "MinusLuck":
                    hero.TotalLuck /= 2;
                    Console.WriteLine($"⚠️  Curse! {hero.Name}'s luck is down.");
                    break;

                case "Bleed":
                    hero.TotalHealth = (int)(hero.TotalHealth * 0.9);
                    Console.WriteLine($"⚠️  Bleeding! {hero.Name} loses 10% health.");
                    break;
            }


            Console.WriteLine($"{hero.Name} opens the loot box...");
            Console.WriteLine($"🎉 It's a {loot.Name}!");
        }
    }
}
