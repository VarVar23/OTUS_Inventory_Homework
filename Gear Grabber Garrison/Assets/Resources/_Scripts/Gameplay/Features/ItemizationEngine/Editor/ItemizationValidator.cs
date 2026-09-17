using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Gameplay.Itemization.Models;

namespace Gameplay.Itemization.Editor
{
    [InitializeOnLoad]
    public static class ItemizationValidator
    {
        private static string Root => Application.streamingAssetsPath + "/Data";

        static ItemizationValidator()
        {
            // This runs automatically when Unity compiles or starts
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                ValidateAll();
            }
        }

        [MenuItem("Tools/Itemization/Run Full Validation")]
        public static bool ValidateAll()
        {
            Debug.Log("<color=cyan>[Validator]</color> Starting Full Data Integrity Check...");
            int errors = 0;

            // 1. Validate Attributes
            var attributeIds = ValidateAttributes(ref errors);

            // 2. Validate Blueprints (Cross-reference with Attributes)
            var blueprintIds = ValidateBlueprints(attributeIds, ref errors);

            // 3. Validate Drop Tables (Cross-reference with Blueprints)
            ValidateDropTables(blueprintIds, ref errors);

            // 4. Validate Type Config
            ValidateTypeConfig(ref errors);

            if (errors > 0)
            {
                Debug.LogError($"<color=red>[Validator] Found {errors} data errors!</color> Please fix them before testing.");
                return false;
            }

            Debug.Log("<color=green>[Validator] All Data Validated Successfully.</color>");
            return true;
        }

        private static HashSet<string> ValidateAttributes(ref int errors)
        {
            HashSet<string> ids = new();
            string path = $"{Root}/Attributes";
            if (!Directory.Exists(path)) return ids;

            foreach (var file in Directory.GetFiles(path, "*.json"))
            {
                var data = JsonUtility.FromJson<AttributeDataModel>(File.ReadAllText(file));
                string fileName = Path.GetFileNameWithoutExtension(file);

                if (data.AttributeId != fileName)
                {
                    Debug.LogError($"[Validator] Attribute ID Mismatch! File '{fileName}.json' contains ID '{data.AttributeId}'");
                    errors++;
                }
                if (data.Levels.Count == 0)
                {
                    Debug.LogWarning($"[Validator] Attribute '{data.AttributeId}' has 0 levels defined.");
                }
                ids.Add(data.AttributeId);
            }
            return ids;
        }

        private static HashSet<string> ValidateBlueprints(HashSet<string> validAttrs, ref int errors)
        {
            HashSet<string> ids = new();
            string path = $"{Root}/Item Blueprints";
            if (!Directory.Exists(path)) return ids;

            foreach (var file in Directory.GetFiles(path, "*.json"))
            {
                var data = JsonUtility.FromJson<ItemBlueprintModel>(File.ReadAllText(file));
                ids.Add(data.BlueprintId);

                foreach (var slot in data.Slots)
                {
                    foreach (var attr in slot.PossibleAttributes)
                    {
                        if (!validAttrs.Contains(attr.AttributeId))
                        {
                            Debug.LogError($"[Validator] Blueprint '{data.BlueprintId}' references missing Attribute '{attr.AttributeId}' in slot '{slot.SlotName}'");
                            errors++;
                        }
                    }
                }
            }
            return ids;
        }

        private static void ValidateDropTables(HashSet<string> validBPs, ref int errors)
        {
            string path = $"{Root}/DropTables";
            if (!Directory.Exists(path)) return;

            foreach (var file in Directory.GetFiles(path, "*.json"))
            {
                var data = JsonUtility.FromJson<DropTableModel>(File.ReadAllText(file));
                foreach (var drop in data.PotentialDrops)
                {
                    if (!validBPs.Contains(drop.BlueprintId))
                    {
                        Debug.LogError($"[Validator] DropTable '{data.TableId}' references missing Blueprint '{drop.BlueprintId}'");
                        errors++;
                    }
                }
            }
        }

        private static void ValidateTypeConfig(ref int errors)
        {
            string path = $"{Root}/Config/item_type_config.json";
            if (!File.Exists(path))
            {
                Debug.LogError("[Validator] Missing global item_type_config.json!");
                errors++;
            }
        }
    }
}