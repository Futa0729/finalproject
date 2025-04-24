namespace RPG_Battler.Gameplay.Rules
{
    public interface ICombatRule
    {
        void Apply(Character.Hero hero, Character.Monster monster);
        string Description { get; }          
    }
}
