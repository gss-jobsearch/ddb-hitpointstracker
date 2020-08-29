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

namespace HitPointsTracker.Tests
{
    [TestClass]
    public class TestHitPointOperations
    {
	    private const string CharName = "foo";
        private const int CharHP = 80;
        private const int CharTempHP = 10;
        private const string DamageImmune = "fire";
        private const string DamageResist = "thunder";
        private const string DamageNormal = "bludgeoning";

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        private string _dbFileName;
        private CharacterContext _db;
        private CharacterController _controller;
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        [TestInitialize]
        public void Setup()
        {
            _dbFileName = Path.GetTempFileName();
            var options = new DbContextOptionsBuilder<CharacterContext>()
                .UseSqlite("Filename=" + _dbFileName)
		        .Options;

            _db = new CharacterContext(options);
            _db.Database.EnsureCreated();
            _controller = new CharacterController(_db);
            var character = new Character()
            {
                Name = CharName,
                MaxHitPoints = CharHP,
                CurHitPoints = CharHP,
                TempHitPoints = CharTempHP
            };
            character.Defenses.Add(new Defense()
            {
                DamageType = DamageImmune,
                IsImmune = true
            });
            character.Defenses.Add(new Defense()
            {
                DamageType = DamageResist,
                IsImmune = false
            });
            _db.Characters.Add(character);
            _db.SaveChanges();
        }

        [TestCleanup]
        public void Teardown()
        {
            _db.Dispose();
            File.Delete(_dbFileName);
	    }

        [TestMethod]
        public async Task TGetHP()
        {
            IActionResult response = await _controller.GetHitPoints(1);
            var hp = await CheckResponse(response,
                CharHP,
                "unmodified",
                CharTempHP,
                "unmodified"
                );
            Assert.IsNull(hp.Previous,
                "there should be no previous values");
        }

        [TestMethod]
        public async Task TDamageImmune()
        {
            IActionResult response = await _controller.Damage(1, DamageImmune, 100);
            var hp = await CheckResponse(response,
                CharHP,
                "unmodified",
                CharTempHP,
                "unmodified"
                );
            Assert.IsNotNull(hp.Previous,
                "there should be previous values");
            CheckSnapshot(hp.Previous!,
                CharHP,
                "unmodified",
                CharTempHP,
                "unmodified"
                );
        }

        [TestMethod]
        public async Task TDamageResist()
        {
            IActionResult response = await _controller.Damage(1, DamageResist, CharHP / 2);
            var hp = await CheckResponse(response,
                CharHP - (CharHP / 4 - CharTempHP),
                "reduced",
                0,
                "zeroed"
                );
            Assert.IsNotNull(hp.Previous,
                "there should be previous values");
            CheckSnapshot(hp.Previous!,
                CharHP,
                "unmodified",
                CharTempHP,
                "unmodified"
                );
        }

        [TestMethod]
        public async Task TDamageNormal()
        {
            IActionResult response = await _controller.Damage(1, DamageNormal, CharHP / 2);
            var hp = await CheckResponse(response,
                CharHP - (CharHP / 2 - CharTempHP),
                "reduced",
                0,
                "zeroed"
                );
            Assert.IsNotNull(hp.Previous,
                "there should be previous values");
            CheckSnapshot(hp.Previous!,
                CharHP,
                "unmodified",
                CharTempHP,
                "unmodified"
                );
        }

        [TestMethod]
        public async Task TOverDamage()
        {
            IActionResult response = await _controller.Damage(1, DamageNormal, CharHP * 2);
            var hp = await CheckResponse(response,
                0,
                "zeroed",
                0,
                "zeroed"
                );
            Assert.IsNotNull(hp.Previous,
                "there should be previous values");
            CheckSnapshot(hp.Previous!,
                CharHP,
                "unmodified",
                CharTempHP,
                "unmodified"
                );
        }

        [TestMethod]
        public async Task THeal()
        {
            await SetHP(CharHP / 2, CharTempHP);
            IActionResult response = await _controller.Heal(1, CharHP / 4);
            var hp = await CheckResponse(response,
                CharHP * 3 / 4,
                "increased",
                CharTempHP,
                "unmodified"
                );
            Assert.IsNotNull(hp.Previous,
                "there should be previous values");
            CheckSnapshot(hp.Previous!,
                CharHP / 2,
                "unmodified",
                CharTempHP,
                "unmodified"
                );
        }

        [TestMethod]
        public async Task TOverHeal()
        {
            await SetHP(CharHP / 2, 0);
            IActionResult response = await _controller.Heal(1, CharHP * 2);
            var hp = await CheckResponse(response,
                CharHP,
                "maximum",
                0,
                "unmodified"
                );
            Assert.IsNotNull(hp.Previous,
                "there should be previous values");
            CheckSnapshot(hp.Previous!,
                CharHP / 2,
                "unmodified",
                0,
                "unmodified"
                );
        }

        [TestMethod]
        public async Task TAddTemp()
        {
            await SetHP(CharHP / 2, CharTempHP);
            IActionResult response = await _controller.AddTemp(1, CharTempHP + 2);
            var hp = await CheckResponse(response,
                CharHP / 2,
                "unmodified",
                CharTempHP + 2,
                "increased"
                );
            Assert.IsNotNull(hp.Previous,
                "there should be previous values");
            CheckSnapshot(hp.Previous!,
                CharHP / 2,
                "unmodified",
                CharTempHP,
                "unmodified"
                );
        }

        [TestMethod]
        public async Task TUnderAddTemp()
        {
            await SetHP(CharHP / 2, CharTempHP);
            IActionResult response = await _controller.AddTemp(1, CharTempHP - 2);
            var hp = await CheckResponse(response,
                CharHP / 2,
                "unmodified",
                CharTempHP,
                "unmodified"
                );
            Assert.IsNotNull(hp.Previous,
                "there should be previous values");
            CheckSnapshot(hp.Previous!,
                CharHP / 2,
                "unmodified",
                CharTempHP,
                "unmodified"
                );
        }

        private void CheckSnapshot(
            HitPointsResult.HitPointsSnapshot snapshot,
            int expectedHP,
            string hpStatus,
            int expectedTemp,
            string tempStatus
            )
        {
            Assert.AreEqual(expectedHP, snapshot.HitPoints,
                "snapshot hit points should be " + hpStatus);
            Assert.AreEqual(expectedTemp, snapshot.TempHitPoints,
                "snapshot temp hit points should be " + tempStatus);
        }

        private async Task<HitPointsResult> CheckResponse(
            IActionResult response,
            int expectedHP,
            string hpStatus,
            int expectedTemp,
            string tempStatus
            )
        {
            var character = await _db.Characters.SingleAsync();
            if (!(response is ObjectResult objectResponse))
            {
                Assert.IsTrue(false,
				    "controller response should be an object result");
                throw new InvalidOperationException("unreachable");
            }
            Assert.AreEqual(200, objectResponse.StatusCode,
                "should have responded successfully");
            if (!(objectResponse.Value is HitPointsResult hp))
            {
                Assert.IsTrue(false,
				    "controller should have responded with a hit points result");
                throw new InvalidOperationException("unreachable");
            }
            CheckSnapshot(hp.Current, expectedHP, hpStatus, expectedTemp, tempStatus);
            Assert.AreEqual(expectedHP, character.CurHitPoints,
                "database hit points should be " + hpStatus);
            Assert.AreEqual(expectedTemp, character.TempHitPoints,
                "database temp hit points should be " + tempStatus);
            return hp;
        }

        private async Task SetHP(int hp, int tempHP)
        {
            var character = await _db.Characters.SingleAsync();
            character.CurHitPoints = hp;
            character.TempHitPoints = tempHP;
            _db.Characters.Update(character);
            await _db.SaveChangesAsync();
        }

    }
}
