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
        private const string Immunity = "immunity";

        private readonly CharacterContext _db;

        public CharacterController(CharacterContext db)
        {
            _db = db;
        }

        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(HitPointsResult), 201)]
        public async Task<IActionResult> Create([FromBody] FullCharacter character)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            Character newCharacter = character.ToCharacter();
            _db.Characters.Add(newCharacter);
            await _db.SaveChangesAsync();
            return Created(newCharacter.Id.ToString(),
                new HitPointsResult(newCharacter));
        }

        [HttpGet("{id}/current")]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(HitPointsResult), 200)]
        public async Task<IActionResult> GetHitPoints(long id)
        {
            Character? character = await _db.Characters
                .FirstOrDefaultAsync(c => c.Id == id);
            if (character == null) return NotFound();
            return Ok(new HitPointsResult(character));
        }

        [HttpPost("{id}/damage")]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(HitPointsResult), 200)]
        public async Task<IActionResult> Damage(long id,
            [FromQuery] string type, [FromQuery] int amount)
        {
            if (amount < 1)
            {
                return BadRequest("Damage must be greater than zero");
            }
            Character? character = await _db.Characters
                .Include(c => c.Defenses)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (character == null) return NotFound();
            var result = new HitPointsResult(character);
            Defense? defense = character.Defenses
                .FirstOrDefault(d => d.DamageType == type);

            if (defense != null)
            {
                amount = defense.IsImmune ?
                    0 : (amount / 2);
            }
            if (amount > 0)
            {
                if (character.TempHitPoints < amount)
                {
                    amount -= character.TempHitPoints;
                    character.TempHitPoints = 0;
                    character.CurHitPoints = (character.CurHitPoints < amount) ?
                        0 : (character.CurHitPoints - amount);
                }
                else
                {
                    character.TempHitPoints -= amount;
                }
                _db.Characters.Update(character);
                await _db.SaveChangesAsync();
            }
            result.Update(character);
            return Ok(result);
        }

        [HttpPost("{id}/heal")]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(HitPointsResult), 200)]
        public async Task<IActionResult> Heal(long id,
            [FromQuery] int amount)
        {
            if (amount < 1)
            {
                return BadRequest("Amount healed must be greater than zero");
            }
            Character? character = await _db.Characters
                .FirstOrDefaultAsync(c => c.Id == id);
            if (character == null) return NotFound();
            var result = new HitPointsResult(character);

            if (character.CurHitPoints < character.MaxHitPoints)
            {
                character.CurHitPoints += amount;
                if (character.CurHitPoints > character.MaxHitPoints)
                {
                    character.CurHitPoints = character.MaxHitPoints;
                }
                _db.Characters.Update(character);
                await _db.SaveChangesAsync();
            }
            result.Update(character);
            return Ok(result);
        }

        [HttpPost("{id}/temp")]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(HitPointsResult), 200)]
        public async Task<IActionResult> AddTemp(long id,
            [FromQuery] int amount)
        {
            if (amount < 1)
            {
                return BadRequest("Temporary hit points added must be greater than zero");
            }
            Character? character = await _db.Characters
                .FirstOrDefaultAsync(c => c.Id == id);
            if (character == null) return NotFound();
            var result = new HitPointsResult(character);

            if (amount > character.TempHitPoints)
            {
                character.TempHitPoints = amount;
                _db.Characters.Update(character);
                await _db.SaveChangesAsync();
            }
            result.Update(character);
            return Ok(result);
        }
    }
}
