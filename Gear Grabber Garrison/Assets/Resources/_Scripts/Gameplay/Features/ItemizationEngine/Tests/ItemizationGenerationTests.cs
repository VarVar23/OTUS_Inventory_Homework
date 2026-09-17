using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Itemization.Models;
using Gameplay.Itemization.Services;

namespace Gameplay.Itemization.Tests
{
    [TestFixture]
    public class ItemizationGenerationTests
    {
        private JSONItemizationRegistry _registry;

        [SetUp]
        public void Setup()
        {
            _registry = new JSONItemizationRegistry();
            InjectMockData();
        }

        private void InjectMockData()
        {
            // 1. Mock the Attribute (The Generator needs to know max levels)
            var mockAttr = new AttributeDataModel {
                AttributeId = "ATTR_TEST",
                Levels = new List<AttributeLevelData> {
                    new AttributeLevelData { Level = 1 },
                    new AttributeLevelData { Level = 2 },
                    new AttributeLevelData { Level = 3 },
                    new AttributeLevelData { Level = 4 },
                    new AttributeLevelData { Level = 5 }
                }
            };

            // 2. Inject into Registry
            _registry.SeedAttributes(new Dictionary<string, AttributeDataModel> { { "ATTR_TEST", mockAttr } });
        }

        [Test]
        public void Generator_BudgetStressTest_StaysWithinConstraints()
        {
            // Arrange
            var blueprint = new ItemBlueprintModel
            {
                BlueprintId = "BP_TEST",
                AttributeLevelBudget = 5,
                ForceFullBudget = true,
                Slots = new List<AttributeSlotModel> {
                    new AttributeSlotModel { 
                        MinAllowedLevel = 1, MaxAllowedLevel = 5, 
                        PossibleAttributes = new List<AttributeWeightModel> { new AttributeWeightModel { AttributeId = "ATTR_TEST", Weight = 100 } } 
                    },
                    new AttributeSlotModel { 
                        MinAllowedLevel = 1, MaxAllowedLevel = 5, 
                        PossibleAttributes = new List<AttributeWeightModel> { new AttributeWeightModel { AttributeId = "ATTR_TEST", Weight = 100 } } 
                    }
                },
                TypeWeights = new List<TypeWeightModel> { new TypeWeightModel { Type = ItemType.Chestpiece, Weight = 100 } }
            };

            var nameGen = new ItemNameGenerator(_registry);
            var iconPicker = new ItemIconPicker(_registry);
            var generator = new BuildInstructionsGenerator(_registry, nameGen, iconPicker);

            // Act & Assert (100 iterations)
            for (int i = 0; i < 100; i++)
            {
                var instructions = generator.CreateInstructions(blueprint);
                int totalLevel = instructions.AttributeMap.Values.Sum();

                Assert.AreEqual(5, totalLevel, $"Iteration {i}: Total budget reconciliation failed.");
                foreach (var level in instructions.AttributeMap.Values)
                {
                    Assert.GreaterOrEqual(level, 1, "Level below slot minimum.");
                    Assert.LessOrEqual(level, 5, "Level above slot maximum.");
                }
            }
        }
    }
}