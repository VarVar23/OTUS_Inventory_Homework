using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using Gameplay.Itemization.Models;

namespace Gameplay.Itemization.Editor
{
    public class ItemIconDatabaseEditor : EditorWindow
    {
        private ItemIconConfigModel _iconConfig = new();
        private ItemTypeConfigModel _typeConfig = new();
        
        private string _iconPath;
        private string _typePath;
        private Vector2 _scrollPos;

        [SerializeField] private DefaultAsset _iconFolder; 
        private string _lastFolderPath;

        [MenuItem("Tools/Itemization/Item Icon Config")]
        public static void ShowWindow() => GetWindow<ItemIconDatabaseEditor>("Item Icon Config");

        private void OnEnable()
        {
            _iconPath = Path.Combine(Application.streamingAssetsPath, "Data/Config/item_icon_config.json");
            _typePath = Path.Combine(Application.streamingAssetsPath, "Data/Config/item_type_config.json");
            Load();
        }

        private void OnGUI()
        {
            GUILayout.Label("Item Icon Mapping", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Select the Root Icon Folder and use 'Auto-Scan' to link sprites automatically.", MessageType.Info);

            // --- 1. THE FOLDER PICKER ---
            EditorGUILayout.BeginVertical("box");
            _iconFolder = (DefaultAsset)EditorGUILayout.ObjectField("Icon Root Folder:", _iconFolder, typeof(DefaultAsset), false);
            
            if (_iconFolder != null)
            {
                string path = AssetDatabase.GetAssetPath(_iconFolder);
                if (!AssetDatabase.IsValidFolder(path))
                {
                    EditorGUILayout.HelpBox("Selected object is not a folder!", MessageType.Error);
                    _iconFolder = null;
                }
                else
                {
                    _lastFolderPath = path;
                    EditorGUILayout.LabelField("Path:", _lastFolderPath, EditorStyles.miniLabel);
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(5);

            // --- 2. ACTIONS ---
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save Configuration", GUILayout.Height(30))) Save();
            
            GUI.enabled = _iconFolder != null; // Disable Auto-Scan if no folder is picked
            GUI.backgroundColor = Color.cyan;
            if (GUILayout.Button("Auto-Scan Icons", GUILayout.Height(30))) AutoScan();
            GUI.backgroundColor = Color.white;
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            for (int i = 0; i < _iconConfig.Mappings.Count; i++)
            {
                DrawMappingRow(_iconConfig.Mappings[i], i);
            }

            EditorGUILayout.Space(10);
            if (GUILayout.Button("+ Add New Manual Mapping")) _iconConfig.Mappings.Add(new IconMapping());
            
            EditorGUILayout.EndScrollView();
        }

        private void AutoScan()
        {
            if (_typeConfig.Definitions.Count == 0) return;

            // Determine the base path relative to Resources or the Assets folder
            // Usually, for runtime loading, we want the path inside "Resources/"
            string rootPath = AssetDatabase.GetAssetPath(_iconFolder);
            
            foreach (var typeDef in _typeConfig.Definitions)
            {
                string typeFolder = typeDef.Type.ToString().ToLower();
                
                foreach (var baseName in typeDef.Names)
                {
                    // Construct: [RootPath]/[Type]/[Name]
                    // We trim the 'Assets/Resources/' prefix if we want it to be usable by Resources.Load later
                    string fullPath = $"{rootPath}/{typeFolder}/{baseName.ToLower()}";
                    string runtimePath = fullPath;

                    if (fullPath.Contains("Resources/"))
                    {
                        runtimePath = fullPath.Substring(fullPath.IndexOf("Resources/") + 10);
                    }

                    var existing = _iconConfig.Mappings.FirstOrDefault(m => m.BaseName == baseName);
                    if (existing != null) existing.SpritePath = runtimePath;
                    else _iconConfig.Mappings.Add(new IconMapping { BaseName = baseName, SpritePath = runtimePath });
                }
            }
            Debug.Log($"<color=cyan>[IconEditor]</color> Auto-scan complete using root: {rootPath}");
        }

        // ... (DrawMappingRow, ShowNestedMenu, Save, Load remain same) ...

        private void DrawMappingRow(IconMapping mapping, int index)
        {
            EditorGUILayout.BeginVertical("helpbox");
            EditorGUILayout.BeginHorizontal();

            string displayLabel = string.IsNullOrEmpty(mapping.BaseName) ? "Select Name..." : mapping.BaseName;
            if (EditorGUILayout.DropdownButton(new GUIContent(displayLabel), FocusType.Keyboard, GUILayout.Width(200)))
            {
                ShowNestedMenu(mapping);
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                _iconConfig.Mappings.RemoveAt(index);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                return;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            mapping.SpritePath = EditorGUILayout.TextField("Runtime Path:", mapping.SpritePath);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
        }

        private void ShowNestedMenu(IconMapping mapping)
        {
            GenericMenu menu = new GenericMenu();
            foreach (var typeDef in _typeConfig.Definitions)
            {
                string category = typeDef.Type.ToString();
                foreach (var nameInType in typeDef.Names)
                {
                    string menuPath = $"{category}/{nameInType}";
                    menu.AddItem(new GUIContent(menuPath), mapping.BaseName == nameInType, () => {
                        mapping.BaseName = nameInType;
                        Repaint();
                    });
                }
            }
            menu.ShowAsContext();
        }

        private void Save() => File.WriteAllText(_iconPath, JsonUtility.ToJson(_iconConfig, true));
        private void Load()
        {
            if (File.Exists(_iconPath)) _iconConfig = JsonUtility.FromJson<ItemIconConfigModel>(File.ReadAllText(_iconPath));
            if (File.Exists(_typePath)) _typeConfig = JsonUtility.FromJson<ItemTypeConfigModel>(File.ReadAllText(_typePath));
        }
    }
}