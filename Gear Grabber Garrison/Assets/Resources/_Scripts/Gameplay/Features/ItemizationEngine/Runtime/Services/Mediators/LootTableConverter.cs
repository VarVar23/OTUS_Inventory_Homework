using System;

namespace Gameplay.Itemization.Services
{
    internal static class LootTableConverter
    {
        public static string ToId(LootTable table)
        {
            return table switch
            {
                LootTable.TestDropTable => "TEST_DROP_TABLE",  
                LootTable.TestCraftTable => "TEST_CRAFT_TABLE", 
                _ => throw new ArgumentOutOfRangeException(nameof(table), table, null)
            };
        }
    }
}