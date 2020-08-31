using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HitPointsTracker.Controllers;
using HitPointsTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

using static HitPointsTracker.Tests.Resources;

namespace HitPointsTracker.Tests
{
    [TestClass]
    public class TestCharacterCreation
    {
	    private const string Immunity = "immunity";

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        private string _dbFileName;
        private CharacterContext _db;
        private CharacterController _controller;
        private FullCharacter _fullCharacter;
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        [TestInitialize]
        public void Setup()
        {
            _dbFileName = Path.GetTempFileName();
            var options = new DbContextOptionsBuilder<CharacterContext>()
                .UseSqlite("Filename=" + _dbFileName)
		        .Options;
            string json = GetResourceString("Data/briv.json")!;

            _db = new CharacterContext(options);
            _db.Database.EnsureCreated();
            _controller = new CharacterController(_db);
            _fullCharacter = JsonConvert.DeserializeObject<FullCharacter>(json);
        }

        [TestCleanup]
        public void Teardown()
        {
            _db.Dispose();
            File.Delete(_dbFileName);
	    }

        [TestMethod]
        public async Task TInvalidCharacter()
        {
            _controller.ModelState.TryAddModelError("fullCharacter", "invalid");
            IActionResult response = await _controller.Create(_fullCharacter);
            var characters = await _db.Characters
                .Include(c => c.Defenses)
		        .ToListAsync();
            if (!(response is ObjectResult objectResponse))
            {
                Assert.IsTrue(false,
				    "controller response should be an object result");
                return;
            }
            Assert.AreEqual(400, objectResponse.StatusCode,
                "should have rejected the character");
            Assert.AreEqual(0, characters.Count,
                "no character should have been created");
	    }

        [TestMethod]
        public async Task TValidCharacter()
        {
            IActionResult response = await _controller.Create(_fullCharacter);
            var character = await _db.Characters
                .Include(c => c.Defenses)
		        .SingleAsync();
            if (!(response is ObjectResult objectResponse))
            {
                Assert.IsTrue(false,
				    "controller response should be an object result");
                return;
            }
            Assert.AreEqual(201, objectResponse.StatusCode,
                "should have accepted the character");
            if (!(objectResponse.Value is HitPointsResult hp))
            {
                Assert.IsTrue(false,
				    "controller should have responded with a hit points result");
                return;
            }
            Assert.AreEqual(_fullCharacter.Defenses?.Count, character.Defenses.Count,
                "should have the same number of defenses");
            var mismatched = character.Defenses!
                .GroupJoin(_fullCharacter.Defenses,
                    c => c.DamageType,
                    fc => fc.Type,
                    (cd, fcd) => (
                        DamageType: cd.DamageType,
                        InputIsImmune: (fcd.Single().Defense == Immunity),
                        RecordIsImmune: cd.IsImmune
                        )
                )
                .Where(join => join.InputIsImmune != join.RecordIsImmune)
                .Select(join => join.DamageType)
                .ToList();
            CollectionAssert.AreEqual(new string[0], mismatched,
                $"damage immunity mismatch");
        }

        [TestMethod]
        public async Task TMostlyValidCharacter()
        {
            var defenses = _fullCharacter.Defenses!;
            defenses.Add(new FullCharacter.CharDefense()
            {
                Type = defenses[0].Type,
                Defense = defenses[1].Defense
            });
            defenses.Add(new FullCharacter.CharDefense()
            {
                Type = defenses[1].Type,
                Defense = defenses[0].Defense
            });
            IActionResult response = await _controller.Create(_fullCharacter);
            var character = await _db.Characters
                .Include(c => c.Defenses)
		        .SingleAsync();
            if (!(response is ObjectResult objectResponse))
            {
                Assert.IsTrue(false,
				    "controller response should be an object result");
                return;
            }
            Assert.AreEqual(201, objectResponse.StatusCode,
                "should have accepted the character");
            if (!(objectResponse.Value is HitPointsResult hp))
            {
                Assert.IsTrue(false,
				    "controller should have responded with a hit points result");
                return;
            }
            Assert.AreEqual(_fullCharacter.Defenses!.Count - 2, character.Defenses.Count,
                "should have the same number of defenses");
            var mismatched = character.Defenses!
                .GroupJoin(_fullCharacter.Defenses,
                    c => c.DamageType,
                    fc => fc.Type,
                    (cd, fcd) => (
                        DamageType: cd.DamageType,
                        InputIsImmune: (fcd.OrderBy(fc => fc.Defense).First().Defense == Immunity),
                        RecordIsImmune: cd.IsImmune
                        )
                )
                .Where(join => join.InputIsImmune != join.RecordIsImmune)
                .Select(join => join.DamageType)
                .ToList();
            CollectionAssert.AreEqual(new string[0], mismatched,
                $"damage immunity mismatch");
        }

    }
}
