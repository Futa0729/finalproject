namespace RPG_Battler.Gameplay.Rules
{
    public class CombatEnvironment
    {
        public string Weather { get; }
        public string TimeOfDay { get; }

        public CombatEnvironment(string weather, string timeOfDay)
        {
            Weather = weather;
            TimeOfDay = timeOfDay;
        }
    }
}
