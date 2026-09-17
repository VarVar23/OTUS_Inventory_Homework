using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Itemization.InternalContracts;
using Gameplay.Itemization.Models;

namespace Gameplay.Itemization.Editor
{
    public class AttributeDatabaseEditor : EditorWindow
    {
        private AttributeDataModel _activeData = new();
        private string _rootPath;
        private Vector2 _mainScroll;
        private Vector2 _gridScroll;

        [MenuItem("Tools/Itemization/Attribute Database")]
        public static void ShowWindow() => GetWindow<AttributeDatabaseEditor>("Attribute Editor");

        private void OnEnable()
        {
            _rootPath = Path.Combine(Application.streamingAssetsPath, "Data/Attributes");
            if (!Directory.Exists(_rootPath)) Directory.CreateDirectory(_rootPath);
        }

        private void OnGUI()
        {
            _mainScroll = EditorGUILayout.BeginScrollView(_mainScroll);
            
            DrawTopToolbar();
            EditorGUILayout.Space(10);
            
            if (!string.IsNullOrEmpty(_activeData.AttributeId))
            {
                DrawValueDefinitionSection();
                EditorGUILayout.Space(15);
                DrawLevelGrid();
                EditorGUILayout.Space(15);
                DrawRuleChainEditor();
                EditorGUILayout.Space(15);
                DrawNamingSection();
            }
            else
            {
                EditorGUILayout.HelpBox("Select an Attribute or create a new one to begin.", MessageType.Info);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawTopToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("Current Attribute:", GUILayout.Width(110));
            _activeData.AttributeId = EditorGUILayout.TextField(_activeData.AttributeId, EditorStyles.toolbarTextField);
            
            if (GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.Width(60))) SaveAttribute();
            if (GUILayout.Button("Load", EditorStyles.toolbarButton, GUILayout.Width(60))) OpenLoadMenu();
            if (GUILayout.Button("New", EditorStyles.toolbarButton, GUILayout.Width(60))) NewAttribute();
            
            EditorGUILayout.EndHorizontal();
        }

        private void DrawNamingSection()
        {
            EditorGUILayout.BeginVertical("helpbox");
            if (_activeData.Naming == null)
            {
                if (GUILayout.Button("Add Naming Data (Prefixes/Postfixes)")) _activeData.Naming = new AttributeNamingData();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Naming Rules", EditorStyles.boldLabel);
                if (GUILayout.Button("Remove", EditorStyles.miniButton, GUILayout.Width(60))) _activeData.Naming = null;
                EditorGUILayout.EndHorizontal();

                if (_activeData.Naming != null)
                {
                    DrawStringList("Prefixes", _activeData.Naming.Prefixes);
                    DrawStringList("Postfixes", _activeData.Naming.Postfixes);
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawValueDefinitionSection()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Value Definitions (Global Keys)", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Adding a key here creates a column in the grid below.", MessageType.None);

            for (int i = 0; i < _activeData.ValueKeys.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                string oldKey = _activeData.ValueKeys[i];
                string newKey = EditorGUILayout.TextField(oldKey);
                
                if (newKey != oldKey) RenameKeyAcrossLevels(oldKey, newKey, i);

                if (GUILayout.Button("X", GUILayout.Width(20))) { RemoveKeyAcrossLevels(oldKey); break; }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+ Add Key Definition"))
            {
                _activeData.ValueKeys.Add("NewKey");
                SyncKeysToAllLevels();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawLevelGrid()
        {
            GUILayout.Label("Level Data Grid", EditorStyles.boldLabel);

            _gridScroll = EditorGUILayout.BeginScrollView(_gridScroll, "box", GUILayout.Height(250));

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("Lvl", GUILayout.Width(30));
            GUILayout.Label("Cost", GUILayout.Width(60));
            foreach (var key in _activeData.ValueKeys)
            {
                GUILayout.Label(key, GUILayout.Width(100));
            }
            GUILayout.Label("", GUILayout.Width(40));
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < _activeData.Levels.Count; i++)
            {
                var level = _activeData.Levels[i];
                EditorGUILayout.BeginHorizontal();
                
                GUILayout.Label($"{level.Level}", GUILayout.Width(30));
                level.Cost = EditorGUILayout.IntField(level.Cost, GUILayout.Width(60));

                foreach (var key in _activeData.ValueKeys)
                {
                    var entry = level.Values.FirstOrDefault(v => v.Key == key);
                    if (entry != null)
                    {
                        entry.Value = EditorGUILayout.IntField(entry.Value, GUILayout.Width(100));
                    }
                }

                if (GUILayout.Button("X", GUILayout.Width(40))) { _activeData.Levels.RemoveAt(i); break; }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("+ Add Level Row"))
            {
                int next = _activeData.Levels.Count > 0 ? _activeData.Levels.Max(l => l.Level) + 1 : 1;
                _activeData.Levels.Add(new AttributeLevelData { Level = next, Cost = 10 });
                SyncKeysToAllLevels();
            }
        }

        private void DrawStringList(string label, List<string> list)
        {
            GUILayout.Label(label, EditorStyles.miniLabel);
            for (int i = 0; i < list.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                list[i] = EditorGUILayout.TextField(list[i]);
                if (GUILayout.Button("-", GUILayout.Width(20))) { list.RemoveAt(i); break; }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button($"+ Add {label.TrimEnd('e', 's')}", EditorStyles.miniButton)) list.Add("");
        }

        private void SyncKeysToAllLevels()
        {
            foreach (var level in _activeData.Levels)
            {
                foreach (var key in _activeData.ValueKeys)
                {
                    if (!level.Values.Any(v => v.Key == key))
                        level.Values.Add(new AttributeValueEntry { Key = key, Value = 0 });
                }
                level.Values.RemoveAll(v => !_activeData.ValueKeys.Contains(v.Key));
            }
        }

        private void RenameKeyAcrossLevels(string old, string @new, int idx)
        {
            _activeData.ValueKeys[idx] = @new;
            foreach (var l in _activeData.Levels)
            {
                var entry = l.Values.FirstOrDefault(v => v.Key == old);
                if (entry != null) entry.Key = @new;
            }
        }

        private void RemoveKeyAcrossLevels(string key)
        {
            _activeData.ValueKeys.Remove(key);
            foreach (var l in _activeData.Levels) l.Values.RemoveAll(v => v.Key == key);
        }

        private void NewAttribute() { _activeData = new AttributeDataModel { AttributeId = "NEW_ATTR" }; /* _currentFilePath = ""; */ }

        private void OpenLoadMenu()
        {
            string path = EditorUtility.OpenFilePanel("Load Attribute JSON", _rootPath, "json");
            if (!string.IsNullOrEmpty(path)) { LoadAttribute(path); }
        }

        private void LoadAttribute(string path)
        {
            _activeData = JsonUtility.FromJson<AttributeDataModel>(File.ReadAllText(path));
            SyncKeysToAllLevels();
        }

        private void SaveAttribute()
        {
            File.WriteAllText(Path.Combine(_rootPath, $"{_activeData.AttributeId}.json"), JsonUtility.ToJson(_activeData, true));
            AssetDatabase.Refresh();
        }

        private void DrawRuleChainEditor()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Logic & Rule Chains", EditorStyles.boldLabel);

            for (int i = 0; i < _activeData.RuleChains.Count; i++)
            {
                var chain = _activeData.RuleChains[i];
                EditorGUILayout.BeginVertical("helpbox");
                
                EditorGUILayout.BeginHorizontal();
                chain.Trigger = (AttributeTrigger)EditorGUILayout.EnumPopup("Trigger", chain.Trigger);
                if (GUILayout.Button("Remove Chain", GUILayout.Width(100))) { _activeData.RuleChains.RemoveAt(i); break; }
                EditorGUILayout.EndHorizontal();

                // DRAW STEPS
                for (int j = 0; j < chain.Steps.Count; j++)
                {
                    DrawRuleStep(chain.Steps[j], chain.Steps);
                }

                if (GUILayout.Button("+ Add Logic Step")) chain.Steps.Add(new RuleStepModel());
                
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }

            if (GUILayout.Button("+ Add New Rule Chain (Trigger)")) _activeData.RuleChains.Add(new RuleChainModel());
            EditorGUILayout.EndVertical();
        }

        private void DrawRuleStep(RuleStepModel step, List<RuleStepModel> parentList)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            step.Type = (StepType)EditorGUILayout.EnumPopup(step.Type, GUILayout.Width(150));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("X", GUILayout.Width(20))) { parentList.Remove(step); return; }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Description Template", EditorStyles.miniLabel);
            step.DescriptionTemplate = EditorGUILayout.TextField(step.DescriptionTemplate);
            EditorGUILayout.HelpBox("Use {1} for the value placeholder.", MessageType.None);

            if (step.Type != StepType.RunEffect)
            {
                int currentIndex = _activeData.ValueKeys.IndexOf(step.ValueKey);
                int newIndex = EditorGUILayout.Popup("Value Key", currentIndex < 0 ? 0 : currentIndex, _activeData.ValueKeys.ToArray());
                if (_activeData.ValueKeys.Count > 0) step.ValueKey = _activeData.ValueKeys[newIndex];
            }

            if (step.Type == StepType.ModifyStat)
            {
                step.TargetStat = (Stat)EditorGUILayout.EnumPopup("Target Stat", step.TargetStat);
                step.Mod = (ModificationType)EditorGUILayout.EnumPopup("Mod Type", step.Mod);
            }
            else if (step.Type == StepType.RunEffect || step.Type == StepType.RunEffectWithValue)
            {
                step.Effect = (AttributeEffect)EditorGUILayout.EnumPopup("Effect", step.Effect);
            }

            EditorGUILayout.EndVertical();
        }
    }
}