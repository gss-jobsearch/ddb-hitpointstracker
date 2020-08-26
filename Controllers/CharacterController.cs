using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HitPointsTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HitPointsTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly CharacterContext _db;

        public CharacterController(CharacterContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FullCharacter character)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int con = character.Stats!.Constitution + (character.Items?
                .Select(i => i.Modifier)
                .OfType<FullCharacter.ItemModifier>()
                .Select(m => ((m.AffectedObject == "stats") && (m.AffectedValue == "constitution")) ? m.Value : 0)
                .Sum() ?? 0);
            int conMod = (con / 2) - 5;
            int maxHP = (conMod * character.Level) + character.Classes
                .Select(c => c.ClassLevel * ((c.HitDiceValue + 2) / 2))
                .Sum();
            var newCharacter = new Character()
            {
                Name = character.Name!,
                MaxHitPoints = maxHP,
                CurHitPoints = maxHP
            };

            if (character.Defenses is ICollection<FullCharacter.CharDefense> defenses)
            {
                var bestDefenses = new Dictionary<string, Defense>();
                foreach (var defense in defenses)
                {
                    if (bestDefenses.TryGetValue(defense.Type!, out var existing))
                    {
                        // immune overrides resistant
                        if (!existing.IsImmune && (defense.Defense == "immune"))
                        {
                            existing.IsImmune = true;
                        }
                    }
                    else
                    {
                        var newDefense = new Defense()
                        {
                            DamageType = defense.Type!,
                            IsImmune = (defense.Defense == "immune")
                        };
                        bestDefenses.Add(defense.Type!, newDefense);
                        newCharacter.Defenses.Add(newDefense);
                    }
                }
            }
            _db.Characters.Add(newCharacter);
            await _db.SaveChangesAsync();
            return Ok(newCharacter.Id);
        }

        [HttpGet("{id}/current")]
        public async Task<IActionResult> GetHitPoints(long id)
        {
            return Ok(0); //FIXME
        }

        [HttpPut("{id}/damage")]
        public async Task<IActionResult> Damage(long id,
            [FromQuery] string type, [FromQuery] int amount)
        {
            return Ok(0); //FIXME
        }

        [HttpPut("{id}/heal")]
        public async Task<IActionResult> Heal(long id,
            [FromQuery] int amount)
        {
            return Ok(0); //FIXME
        }

        [HttpPut("{id}/temp")]
        public async Task<IActionResult> AddTemp(long id,
            [FromQuery] int amount)
        {
            return Ok(0); //FIXME
        }
    }
}
