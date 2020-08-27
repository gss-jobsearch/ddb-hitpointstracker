using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HitPointsTracker.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

using static HitPointsTracker.Tests.Resources;

namespace HitPointsTracker.Tests
{
    [TestClass]
    public class TestCharacterConversion
    {
        [TestMethod]
        public void TConvertCharacter()
        {
            string json = GetResourceString("Data/briv.json")!;
            var fullCharacter = JsonConvert.DeserializeObject<FullCharacter>(json);
            var result = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(fullCharacter, new ValidationContext(fullCharacter), result);

            Assert.IsTrue(isValid, "validation failed");

            var character = fullCharacter.ToCharacter();

            Assert.AreEqual(fullCharacter.Name, character.Name,
                "name should have been copied");
            Assert.AreEqual(41, character.MaxHitPoints,
                "hit point calculation should match");
        }

    }

}
