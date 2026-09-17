using UnityEditor;
using UnityEngine;
using System.IO;
using Gameplay.Itemization.Models;

namespace Gameplay.Itemization.Editor
{
    public class ItemTypeDatabaseEditor : EditorWindow
    {
        private ItemTypeConfigModel _config = new();
        private string _filePath;
        private Vector2 _scrollPos;

        [MenuItem("Tools/Itemization/Item Type Config")]
        public static void ShowWindow() => GetWindow<ItemTypeDatabaseEditor>("Item Type Config");

        private void OnEnable()
        {
            _filePath = Path.Combine(Application.streamingAssetsPath, "Data/Config/item_type_config.json");
            Load();
        }

        private void OnGUI()
        {
            GUILayout.Label("Global Item Type Configuration", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Maps ItemType Enums to Base Values and Name Pools.", MessageType.Info);

            if (GUILayout.Button("Save Configuration", GUILayout.Height(30))) Save();

            EditorGUILayout.Space(10);
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            for (int i = 0; i < _config.Definitions.Count; i++)
            {
                DrawTypeDefinition(_config.Definitions[i], i);
            }

            EditorGUILayout.Space(10);
            if (GUILayout.Button("+ Add New Type Mapping")) _config.Definitions.Add(new TypeDefinition());

            EditorGUILayout.EndScrollView();
        }

        private void DrawTypeDefinition(TypeDefinition def, int index)
        {
            EditorGUILayout.BeginVertical("helpbox");
            
            // --- HEADER ROW (Type + Value + Remove) ---
            EditorGUILayout.BeginHorizontal();
            
            // Set a fixed width for the label to give the dropdown more space
            EditorGUILayout.LabelField("Type:", GUILayout.Width(40));
            def.Type = (ItemType)EditorGUILayout.EnumPopup(def.Type, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));
            
            GUILayout.Space(10);
            
            EditorGUILayout.LabelField("Base Cost:", GUILayout.Width(70));
            def.BaseValue = EditorGUILayout.IntField(def.BaseValue, GUILayout.Width(60));
            
            if (GUILayout.Button("X", GUILayout.Width(25))) 
            { 
                _config.Definitions.RemoveAt(index); 
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                return; 
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // --- NAME POOL SECTION ---
            EditorGUILayout.LabelField("Base Names Pool", EditorStyles.miniBoldLabel);
            
            // We can draw the names in a more compact grid or vertical list
            for (int j = 0; j < def.Names.Count; j++)
            {
                EditorGUILayout.BeginHorizontal();
                def.Names[j] = EditorGUILayout.TextField(def.Names[j]);
                if (GUILayout.Button("-", GUILayout.Width(20))) { def.Names.RemoveAt(j); break; }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+ Add Name to Pool", EditorStyles.miniButton)) def.Names.Add("New Name");

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        private void Save()
        {
            string dir = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            string json = JsonUtility.ToJson(_config, true);
            File.WriteAllText(_filePath, json);
            AssetDatabase.Refresh();
            Debug.Log($"[ItemTypeEditor] Config saved to: {_filePath}");
        }

        private void Load()
        {
            if (File.Exists(_filePath))
            {
                _config = JsonUtility.FromJson<ItemTypeConfigModel>(File.ReadAllText(_filePath));
            }
        }
    }
}