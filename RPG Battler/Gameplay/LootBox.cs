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
        private static List<LootItem>? _cachedTable;

        public static async Task<LootItem?> OpenAsync(Hero hero)
        {
            // path to JSON
            const string TABLE_PATH =
                "/Users/nishidafuta/final-battle-Futa0729-4/RPG Battler/Gameplay/LootTable.json";

            if (_cachedTable == null)
            {
                _cachedTable = await LoadTableAsync(TABLE_PATH);
            }
            var table = _cachedTable;

            if (table.Count == 0) return null;

            // weighted roll (luck affects odds)
            int Weight(LootItem i) => i.Rarity switch
            {
                LootRarity.Common    => 60,
                LootRarity.Uncommon  => 40,
                LootRarity.Rare      => 20,
                LootRarity.Epic      =>  8,
                LootRarity.Legendary =>  2,
                _                    => 60
            };

            var weighted = new List<(LootItem Item, int Weight)>();
            int total = 0;

            foreach (var i in table)
            {
                int weight = Weight(i) + hero.TotalLuck / 5;
                weighted.Add((i, weight));
                total += weight;
            }

            int roll  = _rng.Next(1, total + 1); // get a random number between 1 and total

            //// find the item that corresponds to the roll adding the weights one by one
            // if the sum exceeds the roll, return that item
            LootItem? loot = null;
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
        private static async Task<List<LootItem>> LoadTableAsync(string absolutePath)
        {
            if (!File.Exists(absolutePath))
            {
                Console.WriteLine("⚠️  Loot table not found at: " + absolutePath);
                return new();
            }

            await using var stream = File.OpenRead(absolutePath);

            // add a converter for enums stored as strings
            var opts = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            opts.Converters.Add(new JsonStringEnumConverter());   //  ← 1 new line

            return (await JsonSerializer.DeserializeAsync<List<LootItem>>(stream, opts))
                ?? new();
        }

        // side-effects & curses
        private static void ApplyEffects(Hero hero, LootItem loot)
        {
            
            var item = new Item(loot.Name, $"Looted ({loot.Rarity})")
            {
                Quantity = 1
            };

            // put curse them randomly
            if (string.IsNullOrEmpty(loot.CurseEffect) && _rng.NextDouble() < 0.1)
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
                case "HalvesLuck":
                    hero.TotalLuck /= 2;
                    Console.WriteLine($"⚠️  Curse! {hero.Name}'s luck is halved.");
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
