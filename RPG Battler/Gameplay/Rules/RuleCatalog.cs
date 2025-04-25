using System.Collections.Generic;
using RPG_Battler.Character;

namespace RPG_Battler.Gameplay.Rules
{

    public static class RuleCatalog
    {
        public static List<ICombatRule> GetRules(CombatEnvironment env, Character.CombatClass heroClass)
        {
            var rules = new List<ICombatRule>();

            // Close-range bonus for Warriors
            if (heroClass == Character.CombatClass.Warrior && env.TimeOfDay == "Day")
                rules.Add(new CloseRangeBoostRule());

            // Storm spell amp for Wizards
            if (heroClass == Character.CombatClass.Wizard && env.Weather == "Rainy")
                rules.Add(new StormSpellAmpRule());

            return rules;
        }
    }

    file class CloseRangeBoostRule : ICombatRule
    {
        public string Description => "+20% Power (daylight bonus)";
        public void Apply(Character.Hero hero, Character.Monster _)
            => hero.TotalPower = (int)(hero.TotalPower * 1.2);
    }

    file class StormSpellAmpRule : ICombatRule
    {
        public string Description => "Spell power doubled by the storm";
        public void Apply(Character.Hero hero, Character.Monster _)
            => hero.TotalPower *= 2;   
    }

    public class EnvironmentMonsterRule : ICombatRule
{
    private string _weather;
    private HabitatType _habitat;

    public EnvironmentMonsterRule(string weather, HabitatType habitat)
    {
        _weather = weather;
        _habitat = habitat;
    }

    public string Description => $"Monster adapts to {_weather} if it's from {_habitat}";

    public void Apply(Hero hero, Monster monster)
    {
        if (monster.Habitat == _habitat && _weather == "Rainy" && _habitat == HabitatType.Forest)
        {
            monster.TotalPower += 5;
            Console.WriteLine("🌲 Monster gains power from the rainy forest!");
        }
    }
}

}
