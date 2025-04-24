using System.Threading.Tasks;
using NUnit.Framework;
using RPG_Battler.Character;
using RPG_Battler.Gameplay;
using RPG_Battler.Gameplay.Rules;
using System.Text.Json;
using System.IO;
using RPG_Battler.Character.Upgrades;



namespace RPG_Battler.Tests
{
    [TestFixture]
    public class LootAndCombatTests
    {
        [Test]
        public async Task LuckyHero_Should_Get_More_Epic_Loot()
        {
            var unlucky = new Hero { TotalLuck = 0 };
            var lucky = new Hero { TotalLuck = 50 };

            int luckyEpic = 0, unluckyEpic = 0;

            for (int i = 0; i < 300; i++)
            {
                var loot1 = await LootBox.OpenAsync(lucky);
                var loot2 = await LootBox.OpenAsync(unlucky);

                if ((int)loot1!.Rarity >= (int)LootRarity.Epic) luckyEpic++;
                if ((int)loot2!.Rarity >= (int)LootRarity.Epic) unluckyEpic++;
            }

            // 🪵 Debug output
            Console.WriteLine($"Lucky: {luckyEpic}, Unlucky: {unluckyEpic}");

            // ✅ Use NUnit's modern syntax
            Assert.That(luckyEpic, Is.AtLeast(unluckyEpic));

        }

        [Test]
        public async Task Wizard_Power_Should_Double_In_Rain()
        {
            var wiz = new Hero
            {
                Name = "TestMage",
                CombatClass = CombatClass.Wizard,
                TotalPower = 20,
                TotalHealth = 50
            };
            var dummy = new Monster
            {
                Name = "TestDummy",
                TotalHealth = 1,
                TotalPower = 0
            };

            var rainyNight = new CombatEnvironment("Rainy", "Night");

            await Combat.StartBattleAsync(wiz, dummy, rainyNight);

            // 🪵 Debug output
            Console.WriteLine($"Wizard Power after rain rule: {wiz.TotalPower}");

            Assert.That(wiz.TotalPower, Is.EqualTo(40));
        }
        [Test]
        public void Warrior_LevelUp_Should_Increase_Health_And_Power()
        {
            var warrior = new Hero
            {
                Name = "TestTank",
                CombatClass = CombatClass.Warrior,
                Level = 1,
                TotalHealth = 100,
                TotalPower = 20
            };

            warrior.LevelUp();

            Assert.That(warrior.Level, Is.EqualTo(2));
            Assert.That(warrior.TotalHealth, Is.EqualTo(120));
            Assert.That(warrior.TotalPower, Is.EqualTo(25));
        }

        [Test]
public void SaveAndLoadHero_Should_PreserveData()
{
    var hero = new Hero
    {
        Name = "TestHero",
        Level = 5,
        TotalHealth = 120,
        TotalPower = 35,
        TotalLuck = 15,
        Mana = 60,
        Spells = new List<Spell>
        {
            new Spell("Test Spell", 5, 20)
        },
        Items = new List<Item>
        {
            new Item { ItemName = "Potion", Quantity = 2 }
        },
        Equipment = new List<Equipment>
        {
            new Equipment("Test Sword", EquipmentSlot.RightArm, StatBoostType.Power, 10, 3)
        }
    };

    var path = "test_save.json";

    // Save
    var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };
    File.WriteAllText(path, JsonSerializer.Serialize(hero, options));

    // Load
    string json = File.ReadAllText(path);
    var loaded = JsonSerializer.Deserialize<Hero>(json, options);

    // Assert
    Assert.That(loaded, Is.Not.Null);
    Assert.That(loaded!.Name, Is.EqualTo(hero.Name));
    Assert.That(loaded.Level, Is.EqualTo(hero.Level));
    Assert.That(loaded.TotalHealth, Is.EqualTo(hero.TotalHealth));
    Assert.That(loaded.Spells[0].SpellName, Is.EqualTo("Test Spell"));
    Assert.That(loaded.Items[0].ItemName, Is.EqualTo("Potion"));
    Assert.That(loaded.Equipment[0].EquipmentName, Is.EqualTo("Test Sword"));

    // Clean up
    File.Delete(path);
}

        

    }
}
