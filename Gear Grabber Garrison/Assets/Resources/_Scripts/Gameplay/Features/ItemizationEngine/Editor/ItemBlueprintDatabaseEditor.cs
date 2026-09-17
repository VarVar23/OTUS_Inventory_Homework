using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using Gameplay.Itemization.Models;

namespace Gameplay.Itemization.Editor
{
    public class ItemBlueprintDatabaseEditor : EditorWindow
    {
        private ItemBlueprintModel _activeData = new();
        private string _rootPath;
        private string[] _availableAttributes;
        private Vector2 _mainScroll;

        [MenuItem("Tools/Itemization/Item Blueprint Database")]
        public static void ShowWindow() => GetWindow<ItemBlueprintDatabaseEditor>("Blueprint Editor");

        private void OnEnable()
        {
            _rootPath = Path.Combine(Application.streamingAssetsPath, "Data/Item Blueprints");
            if (!Directory.Exists(_rootPath)) Directory.CreateDirectory(_rootPath);
            RefreshAttributeList();
        }

        private void RefreshAttributeList()
        {
            string attrPath = Path.Combine(Application.streamingAssetsPath, "Data/Attributes");
            if (Directory.Exists(attrPath))
            {
                _availableAttributes = Directory.GetFiles(attrPath, "*.json")
                    .Select(Path.GetFileNameWithoutExtension)
                    .ToArray();
            }
        }

        private void OnGUI()
        {
            _mainScroll = EditorGUILayout.BeginScrollView(_mainScroll);
            DrawToolbar();
            
            if (!string.IsNullOrEmpty(_activeData.BlueprintId))
            {
                DrawBudgetSection();
                EditorGUILayout.Space(10);
                DrawTypeWeights();
                EditorGUILayout.Space(10);
                DrawSlots();
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("Current Blueprint:", GUILayout.Width(110));
            _activeData.BlueprintId = EditorGUILayout.TextField(_activeData.BlueprintId, EditorStyles.toolbarTextField);

            if (GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.Width(60))) Save();
            if (GUILayout.Button("Load", EditorStyles.toolbarButton, GUILayout.Width(60))) Load();
            if (GUILayout.Button("New", EditorStyles.toolbarButton, GUILayout.Width(60))) _activeData = new() { BlueprintId = "NEW_BLUEPRINT" };
            EditorGUILayout.EndHorizontal();
        }

        private void DrawBudgetSection()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Rarity & Power", EditorStyles.boldLabel);
            _activeData.AttributeLevelBudget = EditorGUILayout.IntField("Level Budget", _activeData.AttributeLevelBudget);
            _activeData.ForceFullBudget = EditorGUILayout.Toggle("Force Full Budget", _activeData.ForceFullBudget);
            EditorGUILayout.EndVertical();
        }

        private void DrawTypeWeights()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Item Type Weights", EditorStyles.boldLabel);
            for (int i = 0; i < _activeData.TypeWeights.Count; i++)
            {
                var tw = _activeData.TypeWeights[i];
                EditorGUILayout.BeginHorizontal();
                tw.Type = (ItemType)EditorGUILayout.EnumPopup(tw.Type);
                tw.Weight = EditorGUILayout.FloatField(tw.Weight);
                if (GUILayout.Button("X", GUILayout.Width(20))) { _activeData.TypeWeights.RemoveAt(i); break; }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("+ Add Type Weight")) _activeData.TypeWeights.Add(new());
            EditorGUILayout.EndVertical();
        }

        private void DrawSlots()
        {
            GUILayout.Label("Attribute Slots", EditorStyles.boldLabel);
            for (int i = 0; i < _activeData.Slots.Count; i++)
            {
                var slot = _activeData.Slots[i];
                EditorGUILayout.BeginVertical("helpbox");
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Slot Name:", GUILayout.Width(70));
                slot.SlotName = EditorGUILayout.TextField(slot.SlotName);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Remove Slot", GUILayout.Width(100))) { _activeData.Slots.RemoveAt(i); break; }
                EditorGUILayout.EndHorizontal();

                slot.MinAllowedLevel = EditorGUILayout.IntField("Min Level", slot.MinAllowedLevel);
                slot.MaxAllowedLevel = EditorGUILayout.IntField("Max Level", slot.MaxAllowedLevel);

                GUILayout.Label("Possible Attributes", EditorStyles.miniBoldLabel);
                for (int j = 0; j < slot.PossibleAttributes.Count; j++)
                {
                    var attr = slot.PossibleAttributes[j];
                    EditorGUILayout.BeginHorizontal();
                    
                    // Dropdown from available JSON attributes
                    int idx = System.Array.IndexOf(_availableAttributes, attr.AttributeId);
                    int nextIdx = EditorGUILayout.Popup(idx < 0 ? 0 : idx, _availableAttributes);
                    if (_availableAttributes.Length > 0) attr.AttributeId = _availableAttributes[nextIdx];

                    attr.Weight = EditorGUILayout.FloatField(attr.Weight);
                    if (GUILayout.Button("-", GUILayout.Width(20))) { slot.PossibleAttributes.RemoveAt(j); break; }
                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button("+ Add Attribute to Slot")) slot.PossibleAttributes.Add(new());
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }
            if (GUILayout.Button("+ Add New Slot")) _activeData.Slots.Add(new());
        }

        private void Save()
        {
            string path = Path.Combine(_rootPath, $"{_activeData.BlueprintId}.json");
            File.WriteAllText(path, JsonUtility.ToJson(_activeData, true));
            AssetDatabase.Refresh();
        }

        private void Load()
        {
            string path = EditorUtility.OpenFilePanel("Load Blueprint", _rootPath, "json");
            if (!string.IsNullOrEmpty(path)) _activeData = JsonUtility.FromJson<ItemBlueprintModel>(File.ReadAllText(path));
            RefreshAttributeList();
        }
    }
}