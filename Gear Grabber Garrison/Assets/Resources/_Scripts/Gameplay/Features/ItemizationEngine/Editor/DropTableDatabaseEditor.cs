using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using Gameplay.Itemization.Models;

namespace Gameplay.Itemization.Editor
{
    public class DropTableDatabaseEditor : EditorWindow
    {
        private DropTableModel _activeData = new();
        private string _rootPath;
        private string[] _availableBlueprints;
        private Vector2 _mainScroll;

        [MenuItem("Tools/Itemization/Drop Table Database")]
        public static void ShowWindow() => GetWindow<DropTableDatabaseEditor>("Drop Table Editor");

        private void OnEnable()
        {
            _rootPath = Path.Combine(Application.streamingAssetsPath, "Data/DropTables");
            if (!Directory.Exists(_rootPath)) Directory.CreateDirectory(_rootPath);
            RefreshBlueprintList();
        }

        private void RefreshBlueprintList()
        {
            string bpPath = Path.Combine(Application.streamingAssetsPath, "Data/Item Blueprints");
            if (Directory.Exists(bpPath))
            {
                _availableBlueprints = Directory.GetFiles(bpPath, "*.json")
                    .Select(Path.GetFileNameWithoutExtension)
                    .ToArray();
            }
        }

        private void OnGUI()
        {
            _mainScroll = EditorGUILayout.BeginScrollView(_mainScroll);
            DrawToolbar();
            
            if (!string.IsNullOrEmpty(_activeData.TableId))
            {
                DrawProbabilitySection();
                EditorGUILayout.Space(10);
                DrawBlueprintList();
            }
            else
            {
                EditorGUILayout.HelpBox("Select a Drop Table or create a new one.", MessageType.Info);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("Table ID:", GUILayout.Width(110));
            _activeData.TableId = EditorGUILayout.TextField(_activeData.TableId, EditorStyles.toolbarTextField);
            if (GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.Width(60))) Save();
            if (GUILayout.Button("Load", EditorStyles.toolbarButton, GUILayout.Width(60))) Load();
            if (GUILayout.Button("New", EditorStyles.toolbarButton, GUILayout.Width(60))) _activeData = new() { TableId = "NEW_DROP_TABLE" };
            EditorGUILayout.EndHorizontal();
        }

        private void DrawProbabilitySection()
        {
            EditorGUILayout.BeginVertical("helpbox");
            GUILayout.Label("Global Weights", EditorStyles.boldLabel);
            
            _activeData.NoItemDropWeight = EditorGUILayout.FloatField("No-Drop Weight", _activeData.NoItemDropWeight);
            
            // Visual indicator of probability
            float total = _activeData.NoItemDropWeight + _activeData.PotentialDrops.Sum(d => d.Weight);
            float noDropChance = total > 0 ? (_activeData.NoItemDropWeight / total) * 100 : 0;
            
            EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(), noDropChance / 100f, $"No-Drop Chance: {noDropChance:F1}%");
            EditorGUILayout.EndVertical();
        }

        private void DrawBlueprintList()
        {
            GUILayout.Label("Blueprint Distribution", EditorStyles.boldLabel);
            
            // 1. Calculate the TOTAL weight first for math
            float totalTableWeight = _activeData.NoItemDropWeight + _activeData.PotentialDrops.Sum(d => d.Weight);
            if (totalTableWeight <= 0) totalTableWeight = 1; // Prevent divide by zero

            for (int i = 0; i < _activeData.PotentialDrops.Count; i++)
            {
                var drop = _activeData.PotentialDrops[i];
                
                // --- ITEM BOX ---
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();
                
                // Blueprint Selection
                int idx = System.Array.IndexOf(_availableBlueprints, drop.BlueprintId);
                int nextIdx = EditorGUILayout.Popup(idx < 0 ? 0 : idx, _availableBlueprints);
                if (_availableBlueprints.Length > 0) drop.BlueprintId = _availableBlueprints[nextIdx];

                // Weight Input
                EditorGUILayout.LabelField("W:", GUILayout.Width(20));
                drop.Weight = EditorGUILayout.FloatField(drop.Weight, GUILayout.Width(40));
                if (drop.Weight < 0) drop.Weight = 0;

                if (GUILayout.Button("X", GUILayout.Width(25))) { _activeData.PotentialDrops.RemoveAt(i); break; }
                EditorGUILayout.EndHorizontal();

                // 2. THE PROBABILITY BAR FOR THIS ITEM
                float itemChance = (drop.Weight / totalTableWeight) * 100f;
                Rect barRect = EditorGUILayout.GetControlRect(false, 14); // 14 pixels high
                
                // Color the bar based on rarity (Green for common, Gold for rare)
                Color barColor = itemChance > 50 ? Color.green : (itemChance < 5 ? Color.yellow : Color.cyan);
                EditorGUI.DrawRect(barRect, new Color(0.1f, 0.1f, 0.1f, 1)); // Background
                
                // Draw the progress part
                Rect progressRect = new Rect(barRect.x, barRect.y, barRect.width * (itemChance / 100f), barRect.height);
                EditorGUI.DrawRect(progressRect, barColor * 0.7f);
                
                // Text Label inside the bar
                GUIStyle centeredLabel = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleCenter };
                EditorGUI.LabelField(barRect, $"Chance: {itemChance:F1}%", centeredLabel);

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(2);
            }

            if (GUILayout.Button("+ Add Blueprint to Table", GUILayout.Height(30)))
            {
                _activeData.PotentialDrops.Add(new BlueprintWeightModel 
                { 
                    BlueprintId = _availableBlueprints.FirstOrDefault(),
                    Weight = 1.0f
                });
            }
        }

        private void Save()
        {
            string path = Path.Combine(_rootPath, $"{_activeData.TableId}.json");
            File.WriteAllText(path, JsonUtility.ToJson(_activeData, true));
            AssetDatabase.Refresh();
            Debug.Log($"[DropTableEditor] Saved: {_activeData.TableId}");
        }

        private void Load()
        {
            string path = EditorUtility.OpenFilePanel("Load Drop Table", _rootPath, "json");
            if (!string.IsNullOrEmpty(path)) _activeData = JsonUtility.FromJson<DropTableModel>(File.ReadAllText(path));
            RefreshBlueprintList();
        }
    }
}