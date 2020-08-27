using System.Collections.Generic;
using System.Linq;

namespace HitPointsTracker.Models
{
    public partial class FullCharacter
    {
        private const string Immunity = "immunity";

        public Character ToCharacter()
        {
            int con = Stats!.Constitution + (
		        Items?
					.Select(i => i.Modifier)
					.OfType<FullCharacter.ItemModifier>()
					.Select(m => ((m.AffectedObject == "stats") && (m.AffectedValue == "constitution")) ? m.Value : 0)
					.Sum()
			    ?? 0);
            int conMod = (con / 2) - 5;
            int maxHP = (conMod * Level) + Classes
                .Select(c => c.ClassLevel * ((c.HitDiceValue + 2) / 2))
                .Sum();
            var character = new Character()
            {
                Name = Name!,
                MaxHitPoints = maxHP,
                CurHitPoints = maxHP
            };

            if (Defenses is ICollection<FullCharacter.CharDefense> defenses)
            {
                var bestDefenses = new Dictionary<string, Defense>();
                foreach (var defense in defenses)
                {
                    if (bestDefenses.TryGetValue(defense.Type!, out var existing))
                    {
                        // immune overrides resistant
                        if (!existing.IsImmune && (defense.Defense == Immunity))
                        {
                            existing.IsImmune = true;
                        }
                    }
                    else
                    {
                        var newDefense = new Defense()
                        {
                            DamageType = defense.Type!,
                            IsImmune = (defense.Defense == Immunity)
                        };
                        bestDefenses.Add(defense.Type!, newDefense);
                        character.Defenses.Add(newDefense);
                    }
                }
            }
            return character;
        }
    }
}
