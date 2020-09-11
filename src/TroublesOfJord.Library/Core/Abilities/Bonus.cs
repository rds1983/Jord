using System.Collections.Generic;

namespace TroublesOfJord.Core.Abilities
{
    public enum BonusType
    {
        Attacks
    }    
    
    public class Bonus: AbilityEffect
    {
        public Dictionary<BonusType, int> Bonuses { get; } = new Dictionary<BonusType, int>();
        
    }
}