using System;
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
        public void TCharacterValidation()
        {
            string json = GetResourceString("Data/briv.json")!;
            var fullCharacter = JsonConvert.DeserializeObject<FullCharacter>(json);

            MutateAndValidate(fullCharacter,
                fc => 0,
                fc =>
                {
                    fc.Classes![0].HitDiceValue = 7;
                    return 1;
                },
                fc =>
                {
                    fc.Stats!.Strength = -1;
                    return 1;
                },
                fc =>
                {
                    fc.Classes![0].ClassLevel -= 1;
                    return 1;
                },
                fc =>
                {
                    fc.Level -= 1;
                    return -1;
                },
                fc =>
                {
                    fc.Items![0].Modifier!.AffectedObject = null;
                    return 1;
                },
                fc =>
                {
                    fc.Items![0].Modifier!.AffectedValue = null;
                    return 1;
                },
                fc =>
                {
                    fc.Items![0].Modifier = null;
                    return -2;
                },
                fc =>
                {
                    fc.Defenses![0].Defense = "weak";
                    return 1;
                },
                fc =>
                {
                    fc.Defenses![0].Defense = null;
                    return 0;
                },
                fc =>
                {
                    fc.Defenses![0].Type = null;
                    return 1;
                },
                fc =>
                {
                    fc.Defenses = null;
                    return -2;
                },
                fc =>
                {
                    fc.Classes![0] = fc.Classes![1];
                    return -1;
                },
                fc =>
                {
                    fc.Stats!.Charisma = 23;
                    return 1;
                },
                fc =>
                {
                    fc.Stats!.Strength = fc.Stats!.Charisma = 18;
                    return -2;
                }
            );
        }

        private static void MutateAndValidate(
            FullCharacter fullCharacter, params Func<FullCharacter, int>[] mutate)
        {
            int errors = 0;
            var result = new List<ValidationResult>();
            var context = new ValidationContext(fullCharacter);
            bool isValid;
            int index = 0;

            foreach (var func in mutate)
            {
                errors += func(fullCharacter);
                isValid = Validator.TryValidateObject(fullCharacter, context, result, true);
                if (errors > 0)
                {
                    Assert.IsFalse(isValid, $"({index}) validation should have failed");
                }
                else
                {
                    Assert.IsTrue(isValid, $"({index}) validation shouldn't have failed");
                }
                Assert.AreEqual(errors, result.Count,
                    $"({index}) wrong number of validation results");
                result.Clear();
                ++index;
            }
        }

        [TestMethod]
        public void TConvertCharacter()
        {
            string json = GetResourceString("Data/briv.json")!;
            var fullCharacter = JsonConvert.DeserializeObject<FullCharacter>(json);
            var character = fullCharacter.ToCharacter();

            Assert.AreEqual(fullCharacter.Name, character.Name,
                "name should have been copied");
            Assert.AreEqual(41, character.MaxHitPoints,
                "hit point calculation should match");
        }

    }

}
