using NUnit.Framework;
using System.Collections.Generic;
using System;
using Gameplay.Itemization.InternalContracts;
using Gameplay.Itemization.Models;
using Gameplay.Itemization.Services;
using UnityEngine;
using System.Reflection;

namespace Gameplay.Itemization.Tests
{

    [TestFixture]
    public class ItemizationLogicTests
    {
        private JSONItemizationRegistry _registry;
        private ItemValueService _valueService;
        private ItemUpgradeService _upgradeService;

        [SetUp]
        public void Setup()
        {
            _registry = new JSONItemizationRegistry();
            InjectMockData();

            _valueService = new ItemValueService(_registry);
            _upgradeService = new ItemUpgradeService(_valueService);
        }

        private void InjectMockData()
        {
            var healthBoost = new AttributeDataModel
            {
                AttributeId = "ATTR_HEALTH_BOOST",
                Levels = new List<AttributeLevelData>
                {
                    new AttributeLevelData
                    {
                        Level = 1,
                        Cost = 50,
                        Values = new List<AttributeValueEntry>
                        {
                            new AttributeValueEntry { Key = "BonusHP", Value = 50 }
                        }
                    },
                    new AttributeLevelData
                    {
                        Level = 2,
                        Cost = 500,
                        Values = new List<AttributeValueEntry>
                        {
                            new AttributeValueEntry { Key = "BonusHP", Value = 100 }
                        }
                    }
                },
                RuleChains = new List<RuleChainModel>
                {
                    new RuleChainModel
                    {
                        Trigger = AttributeTrigger.OnItemEquipped,
                        Steps = new List<RuleStepModel>
                        {
                            new RuleStepModel
                            {
                                Type = StepType.ModifyStat,
                                TargetStat = Stat.MaxHealth,
                                ValueKey = "BonusHP",
                                Mod = ModificationType.Add
                            }
                        }
                    }
                }
            };

            _registry.SeedAttributes(new Dictionary<string, AttributeDataModel> { { "ATTR_HEALTH_BOOST", healthBoost } });
        }

        private static void SetPrivateField<TTarget, TValue>(TTarget target, string fieldName, TValue value)
        {
            FieldInfo field = typeof(TTarget).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(target, value);
        }

        private ItemAttribute CreateAttributeWithChains(params AttributeTrigger[] triggers)
        {
            var attr = new ItemAttribute("ATTR_HEALTH_BOOST");

            for (int i = 0; i < triggers.Length; i++)
            {
                var chain = new RuleChain(triggers[i]);
                chain.ChangeStat(Stat.MaxHealth, ModificationType.Add).WithValues("BonusHP");
                attr.AddChain(chain);
            }

            attr.Initialize(_registry.GetAttribute("ATTR_HEALTH_BOOST"), 1);
            return attr;
        }

        private ItemizationSystem CreateCleanSystemWithItem(Item item, AttributeEffectManager effects = null, ItemOwnerRegistry ownerRegistry = null)
        {
            ItemPool pool = new ItemPool();
            pool.RegisterItemWithStatus(item, ItemStatus.Received);

            ItemOwnerRegistry sharedOwnerRegistry = ownerRegistry ?? new ItemOwnerRegistry();

            ItemPipelineManager pipeline = new ItemPipelineManager();
            SetPrivateField(pipeline, "_pool", pool);

            ItemizationSystem system = new ItemizationSystem();
            SetPrivateField(system, "_pipeline", pipeline);
            SetPrivateField(system, "_effects", effects ?? new AttributeEffectManager(sharedOwnerRegistry));
            SetPrivateField(system, "_ownerRegistry", sharedOwnerRegistry);

            return system;
        }

        [Test]
        public void ValueService_CumulativeHealthBoost_CalculatesLvl2Correct()
        {
            var attr = new ItemAttribute("ATTR_HEALTH_BOOST");
            attr.Initialize(_registry.GetAttribute("ATTR_HEALTH_BOOST"), 2);
            var item = new Item("Sturdy Plate", ItemType.Chestpiece, Resources.Load<Sprite>("Sprites/Itemization/icons/chestpiece/plate"), new List<ItemAttribute> { attr });

            _valueService.UpdateItemValue(item);

            Assert.AreEqual(550, item.ValueData.CurrentValue, "Cumulative cost of Health Boost Lvl 2 should be 550.");
        }

        [Test]
        public void UpgradeService_HealthBoost_IncrementsLevelAndValue()
        {
            var attr = new ItemAttribute("ATTR_HEALTH_BOOST");
            attr.Initialize(_registry.GetAttribute("ATTR_HEALTH_BOOST"), 1);
            var item = new Item("Healthy Vest", ItemType.Chestpiece, Resources.Load<Sprite>("Sprites/Itemization/icons/chestpiece/vest"), new List<ItemAttribute> { attr });
            _valueService.UpdateItemValue(item);

            _upgradeService.UpgradeItem(item);

            Assert.AreEqual(2, item.Attributes[0].Level);
            Assert.AreEqual(550, item.ValueData.CurrentValue);
        }

        [Test]
        public void AttributeManager_RegisterItemAndResolve_ReturnsExpectedStatPayload()
        {
            var manager = new AttributeEffectManager(new ItemOwnerRegistry());
            var attr = CreateAttributeWithChains(AttributeTrigger.OnItemEquipped);
            Guid ownerId = Guid.NewGuid();
            Guid itemId = Guid.NewGuid();

            IReadOnlyList<ResolutionContext> results = manager.RegisterItemAndResolve(ownerId, itemId, new List<ItemAttribute> { attr });

            Assert.AreEqual(1, results.Count);
            Assert.IsInstanceOf<StatResolutionContext>(results[0]);

            var stat = (StatResolutionContext)results[0];
            Assert.AreEqual(ownerId, stat.ItemOwnerId);
            Assert.AreEqual(Stat.MaxHealth, stat.Stat);
            Assert.AreEqual(ModificationType.Add, stat.ModificationType);
            Assert.AreEqual(50f, stat.Value);
        }

        [Test]
        public void AttributeManager_CleanRegisterItemAndResolve_ReturnsStatPayload()
        {
            var manager = new AttributeEffectManager(new ItemOwnerRegistry());
            var attr = CreateAttributeWithChains(AttributeTrigger.OnItemEquipped);
            Guid ownerId = Guid.NewGuid();
            Guid itemId = Guid.NewGuid();

            IReadOnlyList<ResolutionContext> results = manager.RegisterItemAndResolve(ownerId, itemId, new List<ItemAttribute> { attr });

            Assert.AreEqual(1, results.Count);
            Assert.IsInstanceOf<StatResolutionContext>(results[0]);

            var stat = (StatResolutionContext)results[0];
            Assert.AreEqual(ownerId, stat.ItemOwnerId);
            Assert.AreEqual(Stat.MaxHealth, stat.Stat);
            Assert.AreEqual(ModificationType.Add, stat.ModificationType);
            Assert.AreEqual(50f, stat.Value);
        }

        [Test]
        public void AttributeManager_CleanUnregisterItemAndResolve_ReturnsStatPayload()
        {
            var manager = new AttributeEffectManager(new ItemOwnerRegistry());
            var attr = CreateAttributeWithChains(AttributeTrigger.OnItemEquipped, AttributeTrigger.OnItemUnequipped);
            Guid ownerId = Guid.NewGuid();
            Guid itemId = Guid.NewGuid();

            manager.RegisterItemAndResolve(ownerId, itemId, new List<ItemAttribute> { attr });
            IReadOnlyList<ResolutionContext> results = manager.UnregisterItemAndResolve(ownerId, itemId);

            Assert.AreEqual(1, results.Count);
            Assert.IsInstanceOf<StatResolutionContext>(results[0]);

            var stat = (StatResolutionContext)results[0];
            Assert.AreEqual(ownerId, stat.ItemOwnerId);
            Assert.AreEqual(Stat.MaxHealth, stat.Stat);
            Assert.AreEqual(ModificationType.Add, stat.ModificationType);
            Assert.AreEqual(50f, stat.Value);
        }

        [Test]
        public void ItemizationSystem_EquipItem_RaisesInventoryResolved()
        {
            var attr = CreateAttributeWithChains(AttributeTrigger.OnItemEquipped);
            var item = new Item("Clean Equip", ItemType.Chestpiece, null, new List<ItemAttribute> { attr });
            var system = CreateCleanSystemWithItem(item);
            IReadOnlyList<IResolutionPayload> raised = null;
            Guid ownerId = Guid.NewGuid();

            system.OnInventoryResolved += payloads => raised = payloads;
            system.EquipItem(ownerId, item.Id);

            Assert.IsNotNull(raised);
            Assert.AreEqual(1, raised.Count);
            Assert.IsInstanceOf<IStatResolutionPayload>(raised[0]);

            IStatResolutionPayload stat = (IStatResolutionPayload)raised[0];
            Assert.AreEqual(ownerId, stat.ItemOwnerId);
            Assert.AreEqual(Stat.MaxHealth, stat.Stat);
            Assert.AreEqual(ModificationType.Add, stat.ModificationType);
            Assert.AreEqual(50f, stat.Value);
        }

        [Test]
        public void ItemizationSystem_UnequipItem_RaisesInventoryResolved()
        {
            var attr = CreateAttributeWithChains(AttributeTrigger.OnItemEquipped, AttributeTrigger.OnItemUnequipped);
            var item = new Item("Clean Unequip", ItemType.Chestpiece, null, new List<ItemAttribute> { attr });
            var system = CreateCleanSystemWithItem(item);
            IReadOnlyList<IResolutionPayload> raised = null;
            Guid ownerId = Guid.NewGuid();

            system.EquipItem(ownerId, item.Id);
            system.OnInventoryResolved += payloads => raised = payloads;
            system.UnequipItem(ownerId, item.Id);

            Assert.IsNotNull(raised);
            Assert.AreEqual(1, raised.Count);
            Assert.IsInstanceOf<IStatResolutionPayload>(raised[0]);

            IStatResolutionPayload stat = (IStatResolutionPayload)raised[0];
            Assert.AreEqual(ownerId, stat.ItemOwnerId);
            Assert.AreEqual(Stat.MaxHealth, stat.Stat);
            Assert.AreEqual(ModificationType.Add, stat.ModificationType);
            Assert.AreEqual(50f, stat.Value);
        }

        [Test]
        public void AttributeManager_ActivateTrigger_ReturnsPayloadsForSourceOwnerOnly()
        {
            var ownerRegistry = new ItemOwnerRegistry();
            var manager = new AttributeEffectManager(ownerRegistry);

            Guid owner1 = Guid.NewGuid();
            Guid owner2 = Guid.NewGuid();

            var attr1 = CreateAttributeWithChains(AttributeTrigger.OnDamageDealt);
            var attr2 = CreateAttributeWithChains(AttributeTrigger.OnDamageDealt);

            var item1 = new Item("Owner1 Item", ItemType.Chestpiece, null, new List<ItemAttribute> { attr1 });
            var item2 = new Item("Owner2 Item", ItemType.Chestpiece, null, new List<ItemAttribute> { attr2 });

            ownerRegistry.RegisterEquip(owner1, item1);
            ownerRegistry.RegisterEquip(owner2, item2);

            manager.RegisterItemAndResolve(owner1, item1.Id, item1.Attributes);
            manager.RegisterItemAndResolve(owner2, item2.Id, item2.Attributes);

            TriggerContext context = new TriggerContext
            {
                ItemOwnerId = owner1,
                TargetId = Guid.NewGuid(),
                Trigger = AttributeTrigger.OnDamageDealt
            };

            IReadOnlyList<ResolutionContext> results = manager.ActivateTrigger(context);

            Assert.AreEqual(1, results.Count);
            Assert.IsInstanceOf<StatResolutionContext>(results[0]);

            var stat = (StatResolutionContext)results[0];
            Assert.AreEqual(owner1, stat.ItemOwnerId);
            Assert.AreEqual(Stat.MaxHealth, stat.Stat);
            Assert.AreEqual(ModificationType.Add, stat.ModificationType);
            Assert.AreEqual(50f, stat.Value);
        }

        [Test]
        public void ItemizationSystem_ActivateTrigger_ReturnsMappedPayloads()
        {
            var attr = CreateAttributeWithChains(AttributeTrigger.OnDamageDealt);
            var item = new Item("Trigger Payload", ItemType.Chestpiece, null, new List<ItemAttribute> { attr });
            var system = CreateCleanSystemWithItem(item);
            Guid ownerId = Guid.NewGuid();

            system.EquipItem(ownerId, item.Id);

            TriggerContext context = new TriggerContext
            {
                ItemOwnerId = ownerId,
                TargetId = Guid.NewGuid(),
                Trigger = AttributeTrigger.OnDamageDealt
            };

            IReadOnlyList<IResolutionPayload> payloads = system.ActivateTrigger(context);

            Assert.IsNotNull(payloads);
            Assert.AreEqual(1, payloads.Count);
            Assert.IsInstanceOf<IStatResolutionPayload>(payloads[0]);

            IStatResolutionPayload stat = (IStatResolutionPayload)payloads[0];
            Assert.AreEqual(ownerId, stat.ItemOwnerId);
            Assert.AreEqual(Stat.MaxHealth, stat.Stat);
            Assert.AreEqual(ModificationType.Add, stat.ModificationType);
            Assert.AreEqual(50f, stat.Value);
        }
    }

    [TestFixture]
    public class WeightedRandomServiceTests
    {
        // --- 1. ZERO WEIGHT GUARD ---
        [Test]
        public void TryPick_ZeroTotalWeight_ReturnsFalse()
        {
            // A table with no drops and no no-drop weight — degenerate case
            var table = new DropTableModel
            {
                NoItemDropWeight = 0,
                PotentialDrops = new List<BlueprintWeightModel>()
            };

            bool result = WeightedRandomService.TryPick(table, out string id);

            Assert.IsFalse(result);
            Assert.IsNull(id);
        }

        // --- 2. NO-ITEM WEIGHT ALWAYS WINS ---
        [Test]
        public void TryPick_NoItemWeightDominates_ReturnsFalse()
        {
            // NoItemDropWeight = 1000, item weight = 0.001 — practically guaranteed no drop
            // We can't control Random, but we CAN verify it returns false when NoItemDropWeight
            // is the entire weight (items have 0 weight).
            var table = new DropTableModel
            {
                NoItemDropWeight = 100,
                PotentialDrops = new List<BlueprintWeightModel>
                {
                    new BlueprintWeightModel { BlueprintId = "BP_TEST", Weight = 0 }
                }
            };

            // With item weight = 0, total = 100, all rolls go to no-drop
            // Run 100 times — should never return an item
            for (int i = 0; i < 100; i++)
            {
                bool result = WeightedRandomService.TryPick(table, out _);
                Assert.IsFalse(result, $"Iteration {i}: Should not drop when all item weights are 0.");
            }
        }

        // --- 3. SINGLE ITEM, NO NO-DROP WEIGHT — must always return it ---
        [Test]
        public void TryPick_SingleItemNoNoDrop_AlwaysReturnsThatItem()
        {
            var table = new DropTableModel
            {
                NoItemDropWeight = 0,
                PotentialDrops = new List<BlueprintWeightModel>
                {
                    new BlueprintWeightModel { BlueprintId = "BP_ONLY", Weight = 100 }
                }
            };

            for (int i = 0; i < 50; i++)
            {
                bool result = WeightedRandomService.TryPick(table, out string id);
                Assert.IsTrue(result, $"Iteration {i}: Expected an item to drop.");
                Assert.AreEqual("BP_ONLY", id);
            }
        }

        // --- 4. LAST ITEM FALLBACK IS REACHABLE ---
        [Test]
        public void TryPick_MultipleItems_LastItemIsReachable()
        {
            // If the boundary condition were roll < cursor (strict less-than),
            // the last item could only be hit by the fallback return — not the loop.
            // With roll <= cursor, the last item IS reachable inside the loop itself.
            // We verify last item is returned at least once across many rolls.
            var table = new DropTableModel
            {
                NoItemDropWeight = 0,
                PotentialDrops = new List<BlueprintWeightModel>
                {
                    new BlueprintWeightModel { BlueprintId = "BP_FIRST", Weight = 50 },
                    new BlueprintWeightModel { BlueprintId = "BP_LAST", Weight = 50 }
                }
            };

            bool lastItemSeen = false;
            for (int i = 0; i < 200; i++)
            {
                WeightedRandomService.TryPick(table, out string id);
                if (id == "BP_LAST") { lastItemSeen = true; break; }
            }

            Assert.IsTrue(lastItemSeen, "BP_LAST was never returned in 200 rolls — boundary condition may be broken.");
        }
    }

    [TestFixture]
    public class ItemPoolTests
    {
        private ItemPool _pool;

        [SetUp]
        public void Setup()
        {
            _pool = new ItemPool();
        }

        private static Item MakeItem() =>
            new Item("Test Item", ItemType.Chestpiece, null, new List<ItemAttribute>());

        // --- 1. NORMAL FLOW ---
        [Test]
        public void PopNextGenerated_NormalFlow_ReturnsRegisteredItem()
        {
            Item item = MakeItem();
            _pool.RegisterItem(item);

            Item popped = _pool.PopNextGenerated();

            Assert.AreEqual(item.Id, popped.Id);
        }

        // --- 2. UNCLAIMED COUNT IS ACCURATE ---
        [Test]
        public void UnclaimedItemCount_AfterRegisterAndPop_IsAccurate()
        {
            _pool.RegisterItem(MakeItem());
            _pool.RegisterItem(MakeItem());

            Assert.AreEqual(2, _pool.UnclaimedItemCount);

            _pool.PopNextGenerated();

            Assert.AreEqual(1, _pool.UnclaimedItemCount);
        }

        // --- 3. ZOMBIE ITEM: DESTROY BEFORE POP ---
        [Test]
        public void PopNextGenerated_DestroyedItemInQueue_IsSkippedGracefully()
        {
            Item zombie = MakeItem();
            Item live = MakeItem();

            _pool.RegisterItem(zombie);
            _pool.RegisterItem(live);

            // Destroy zombie while it's still at the front of the queue
            _pool.DestroyItem(zombie.Id);

            // Should skip the zombie and return the live item — not throw
            Item result = _pool.PopNextGenerated();

            Assert.AreEqual(live.Id, result.Id);
        }

        // --- 4. ZOMBIE ITEM: ALL QUEUED ITEMS DESTROYED ---
        [Test]
        public void PopNextGenerated_AllQueuedItemsDestroyed_ThrowsInvalidOperation()
        {
            Item zombie = MakeItem();
            _pool.RegisterItem(zombie);
            _pool.DestroyItem(zombie.Id);

            // Queue has 1 dead guid — should throw, not crash with KeyNotFoundException
            Assert.Throws<InvalidOperationException>(() => _pool.PopNextGenerated());
        }

        // --- 5. UNCLAIMED COUNT AFTER ZOMBIE SKIP ---
        [Test]
        public void UnclaimedItemCount_AfterZombieSkip_ReflectsTrueCount()
        {
            Item zombie = MakeItem();
            Item live = MakeItem();

            _pool.RegisterItem(zombie);
            _pool.RegisterItem(live);
            _pool.DestroyItem(zombie.Id);

            // Trigger the skip
            _pool.PopNextGenerated();

            // After popping, queue should be empty
            Assert.AreEqual(0, _pool.UnclaimedItemCount);
        }
    }

    [TestFixture]
    public class RuleChainTests
    {
        // --- 1. FILTER PASSES: payload IS emitted ---
        [Test]
        public void RuleChain_FilterPasses_EmitsPayload()
        {
            var chain = new RuleChain(AttributeTrigger.OnItemEquipped);
            chain.Where(_ => true)
                .ChangeStat(Stat.MaxHealth, ModificationType.Add)
                .WithValues("BonusHP");

            var attr = new ItemAttribute("ATTR_TEST");
            attr.AddChain(chain);
            attr.Initialize(new AttributeDataModel
            {
                AttributeId = "ATTR_TEST",
                Levels = new List<AttributeLevelData>
                {
                    new AttributeLevelData { Level = 1, Values = new List<AttributeValueEntry>
                        { new AttributeValueEntry { Key = "BonusHP", Value = 50 } } }
                }
            }, 1);

            Guid ownerId = Guid.NewGuid();
            var context = new TriggerContext { ItemOwnerId = ownerId, Trigger = AttributeTrigger.OnItemEquipped };
            List<ResolutionContext> results = attr.ResolveTrigger(context);

            Assert.AreEqual(1, results.Count, "A matching filtered chain should emit one payload.");
            Assert.IsInstanceOf<StatResolutionContext>(results[0]);

            var stat = (StatResolutionContext)results[0];
            Assert.AreEqual(ownerId, stat.ItemOwnerId);
            Assert.AreEqual(Stat.MaxHealth, stat.Stat);
            Assert.AreEqual(ModificationType.Add, stat.ModificationType);
            Assert.AreEqual(50f, stat.Value);
        }

        // --- 2. FILTER BLOCKS: payload is NOT emitted ---
        [Test]
        public void RuleChain_FilterBlocks_EmitsNoPayload()
        {
            var chain = new RuleChain(AttributeTrigger.OnItemEquipped);
            chain.Where(_ => false)   // always blocks
                .ChangeStat(Stat.MaxHealth, ModificationType.Add)
                .WithValues("BonusHP");

            var attr = new ItemAttribute("ATTR_TEST");
            attr.AddChain(chain);
            attr.Initialize(new AttributeDataModel
            {
                AttributeId = "ATTR_TEST",
                Levels = new List<AttributeLevelData>
                {
                    new AttributeLevelData { Level = 1, Values = new List<AttributeValueEntry>
                        { new AttributeValueEntry { Key = "BonusHP", Value = 50 } } }
                }
            }, 1);

            var context = new TriggerContext { ItemOwnerId = Guid.NewGuid(), Trigger = AttributeTrigger.OnItemEquipped };
            List<ResolutionContext> results = attr.ResolveTrigger(context);

            Assert.AreEqual(0, results.Count, "A blocked chain must not emit payloads.");
        }

        // --- 3. WRONG TRIGGER: chain does not fire ---
        [Test]
        public void RuleChain_WrongTrigger_EmitsNoPayload()
        {
            var chain = new RuleChain(AttributeTrigger.OnItemUnequipped); // unequip chain
            chain.ChangeStat(Stat.MaxHealth, ModificationType.Add)
                .WithValues("BonusHP");

            var attr = new ItemAttribute("ATTR_TEST");
            attr.AddChain(chain);
            attr.Initialize(new AttributeDataModel
            {
                AttributeId = "ATTR_TEST",
                Levels = new List<AttributeLevelData>
                {
                    new AttributeLevelData { Level = 1, Values = new List<AttributeValueEntry>
                        { new AttributeValueEntry { Key = "BonusHP", Value = 50 } } }
                }
            }, 1);

            var context = new TriggerContext { ItemOwnerId = Guid.NewGuid(), Trigger = AttributeTrigger.OnItemEquipped };
            List<ResolutionContext> results = attr.ResolveTrigger(context);

            Assert.AreEqual(0, results.Count, "A non-matching trigger must not emit payloads.");
        }
    }

    [TestFixture]
    public class DescriptionGeneratorTests
    {
        private static AttributeDataModel MakeModel(string template, int lvl1Value, int lvl2Value = 0)
        {
            return new AttributeDataModel
            {
                AttributeId = "ATTR_DESC_TEST",
                Levels = new List<AttributeLevelData>
                {
                    new AttributeLevelData
                    {
                        Level = 1,
                        Cost = 10,
                        Values = new List<AttributeValueEntry>
                            { new AttributeValueEntry { Key = "Val", Value = lvl1Value } }
                    },
                    new AttributeLevelData
                    {
                        Level = 2,
                        Cost = 20,
                        Values = new List<AttributeValueEntry>
                            { new AttributeValueEntry { Key = "Val", Value = lvl2Value } }
                    }
                },
                RuleChains = new List<RuleChainModel>
                {
                    new RuleChainModel
                    {
                        Trigger = AttributeTrigger.OnItemEquipped,
                        Steps = new List<RuleStepModel>
                        {
                            new RuleStepModel
                            {
                                Type = StepType.ModifyStat,
                                DescriptionTemplate = template,
                                ValueKey = "Val"
                            }
                        }
                    }
                }
            };
        }

        // --- 1. STATIC TEMPLATE (NO PLACEHOLDER) ---
        [Test]
        public void FillAttributeDescription_NoPlaceholder_CopiesTemplateVerbatim()
        {
            var model = MakeModel("Cursed item.", 50);
            var attr = new ItemAttribute("ATTR_DESC_TEST");
            attr.Initialize(model, 1);

            DescriptionGenerator.FillAttributeDescription(attr);

            Assert.AreEqual("Cursed item.", attr.Description);
            Assert.AreEqual("Cursed item.", attr.UpgradeDescription);
        }

        // --- 2. VALUE SUBSTITUTION ---
        [Test]
        public void FillAttributeDescription_WithPlaceholder_SubstitutesCurrentValue()
        {
            var model = MakeModel("+{1} Max Health", 50, 100);
            var attr = new ItemAttribute("ATTR_DESC_TEST");
            attr.Initialize(model, 1);

            DescriptionGenerator.FillAttributeDescription(attr);

            Assert.AreEqual("+50 Max Health", attr.Description);
        }

        // --- 3. UPGRADE ARROW: value changes at next level ---
        [Test]
        public void FillAttributeDescription_CanUpgrade_ShowsArrowWhenValueChanges()
        {
            var model = MakeModel("+{1} Max Health", 50, 100);
            var attr = new ItemAttribute("ATTR_DESC_TEST");
            attr.Initialize(model, 1); // Level 1 of 2 → CanUpgrade = true

            DescriptionGenerator.FillAttributeDescription(attr);

            Assert.AreEqual("+50 -> 100 Max Health", attr.UpgradeDescription);
        }

        // --- 4. UPGRADE ARROW: value unchanged at next level → no arrow ---
        [Test]
        public void FillAttributeDescription_CanUpgrade_NoArrowWhenValueUnchanged()
        {
            var model = MakeModel("+{1} Max Health", 50, 50); // same value at both levels
            var attr = new ItemAttribute("ATTR_DESC_TEST");
            attr.Initialize(model, 1);

            DescriptionGenerator.FillAttributeDescription(attr);

            Assert.AreEqual("+50 Max Health", attr.UpgradeDescription);
        }

        // --- 5. AT MAX LEVEL: upgrade description equals current description ---
        [Test]
        public void FillAttributeDescription_AtMaxLevel_UpgradeDescriptionMatchesCurrent()
        {
            var model = MakeModel("+{1} Max Health", 50, 100);
            var attr = new ItemAttribute("ATTR_DESC_TEST");
            attr.Initialize(model, 2); // Level 2 of 2 → CanUpgrade = false

            DescriptionGenerator.FillAttributeDescription(attr);

            Assert.AreEqual("+100 Max Health", attr.Description);
            Assert.AreEqual("+100 Max Health", attr.UpgradeDescription);
        }

        // --- 6. OWNER SUMMARY: aggregates same stat from multiple attributes ---
        [Test]
        public void GenerateOwnerSummary_MultipleAttributes_AggregatesSameStat()
        {
            var model = MakeModel("+{1} Max Health", 50, 100);

            var attr1 = new ItemAttribute("ATTR_DESC_TEST");
            attr1.Initialize(model, 1); // +50

            var attr2 = new ItemAttribute("ATTR_DESC_TEST");
            attr2.Initialize(model, 1); // +50

            var item = new Item("Test Vest", ItemType.Chestpiece, null, new List<ItemAttribute> { attr1, attr2 });

            string summary = DescriptionGenerator.GenerateOwnerSummary(new List<Item> { item });

            // Both attributes contribute +50 → total should be +100
            Assert.IsTrue(summary.Contains("100"), $"Expected aggregated value 100 in summary, got: '{summary}'");
        }
    }
}