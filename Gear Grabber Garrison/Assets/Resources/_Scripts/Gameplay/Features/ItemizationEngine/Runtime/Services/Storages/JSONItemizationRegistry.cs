using System.Collections.Generic;
using System.IO;
using Gameplay.Itemization.Models;
using UnityEngine;

namespace Gameplay.Itemization.Services
{
    public sealed class JSONItemizationRegistry
    {
        private Dictionary<string, AttributeDataModel> _attributes = new();
        private Dictionary<string, ItemBlueprintModel> _blueprints = new();
        private Dictionary<string, DropTableModel> _dropTables = new();
        private readonly Dictionary<ItemType, TypeDefinition> _typeConfigLookup = new();
        private ItemTypeConfigModel _typeConfig;
        private ItemIconConfigModel _iconConfig;

        public void Initialize()
        {
            string root = Path.Combine(Application.streamingAssetsPath, "Data");
            Debug.Log($"<color=cyan>[Registry]</color> Initializing from: {root}");

            _attributes = LoadFolder<AttributeDataModel>(Path.Combine(root, "Attributes"));
            _blueprints = LoadFolder<ItemBlueprintModel>(Path.Combine(root, "Item Blueprints"));
            _dropTables = LoadFolder<DropTableModel>(Path.Combine(root, "DropTables"));

            string configPath = Path.Combine(root, "Config/item_type_config.json");
            if (File.Exists(configPath))
            {
                _typeConfig = JsonUtility.FromJson<ItemTypeConfigModel>(File.ReadAllText(configPath));
            
                for (int i = 0; i < _typeConfig.Definitions.Count; i++)
                    _typeConfigLookup[_typeConfig.Definitions[i].Type] = _typeConfig.Definitions[i];
            }

            string iconConfigPath = Path.Combine(root, "Config/item_icon_config.json");
            if (File.Exists(iconConfigPath))
            {
                _iconConfig = JsonUtility.FromJson<ItemIconConfigModel>(File.ReadAllText(iconConfigPath));
            }

            Debug.Log($"<color=green>[Registry]</color> Hydrated: {_attributes.Count} Attrs, " +
                    $"{_blueprints.Count} BPs, {_dropTables.Count} Tables, Icon Config: {_iconConfig != null}.");
        }

        private Dictionary<string, T> LoadFolder<T>(string path)
        {
            var dict = new Dictionary<string, T>();
            if (!Directory.Exists(path)) return dict;

            foreach (var file in Directory.GetFiles(path, "*.json"))
            {
                T data = JsonUtility.FromJson<T>(File.ReadAllText(file));
                string id = Path.GetFileNameWithoutExtension(file);
                dict.Add(id, data);
            }
            return dict;
        }

        internal AttributeDataModel GetAttribute(string id) => _attributes.GetValueOrDefault(id);
        internal ItemBlueprintModel GetBlueprint(string id) => _blueprints.GetValueOrDefault(id);
        internal DropTableModel GetDropTable(string id) => _dropTables.GetValueOrDefault(id);
        internal TypeDefinition GetTypeConfig(ItemType type)
        {
            _typeConfigLookup.TryGetValue(type, out TypeDefinition definition);
            return definition;
        }
        internal ItemIconConfigModel GetIconConfig() => _iconConfig;

        #if UNITY_INCLUDE_TESTS
        internal void SeedAttributes(Dictionary<string, AttributeDataModel> attributes) => _attributes = attributes;
        internal void SeedBlueprints(Dictionary<string, ItemBlueprintModel> blueprints) => _blueprints = blueprints;
        internal void SeedDropTables(Dictionary<string, DropTableModel> dropTables) => _dropTables = dropTables;
        #endif
    }
}