using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {
        public class MyData
        {
            public static class InventoryItemData
            { // Values for IngotOnly Tech-Recipes sorced from :: https://docs.google.com/spreadsheets/d/16h2c_EG2KuWjvYbkzYewGYWC4vc2YEWvoRbl9pr-XOc/edit?usp=sharing
                public static ItemInfo Cobalt = new ItemInfo("MyObjectBuilder_Ingot", "Cobalt");
                public static ItemInfo Gold = new ItemInfo("MyObjectBuilder_Ingot", "Gold") ;
                public static ItemInfo Iron = new ItemInfo("MyObjectBuilder_Ingot", "Iron");
                public static ItemInfo Magnesium = new ItemInfo("MyObjectBuilder_Ingot", "Magnesium" );
                public static ItemInfo Platinum = new ItemInfo("MyObjectBuilder_Ingot", "Platinum");
                public static ItemInfo Silicon = new ItemInfo("MyObjectBuilder_Ingot", "Silicon");
                public static ItemInfo Silver = new ItemInfo("MyObjectBuilder_Ingot", "Silver");
                public static ItemInfo Uranium = new ItemInfo("MyObjectBuilder_Ingot", "Uranium");
                public static ItemInfo Common = new ItemInfo("MyObjectBuilder_Component", "Tech2x",new int[]{ 32, 16, 90, 0, 0, 80, 24, 0, 0, 0, 0, 0, 0 }); // These int[] recipe values are IngotOnly.
                public static ItemInfo Rare = new ItemInfo("MyObjectBuilder_Component", "Tech4x", new int[] { 160, 80, 450, 0, 0, 400, 120, 10, 0, 0, 0, 0, 0 });
                public static ItemInfo Exotic = new ItemInfo("MyObjectBuilder_Component", "Tech8x", new int[] { 800, 400, 2250, 0, 10, 2000, 600, 50, 0, 0, 0, 0, 0, });
                public static ItemInfo Prosonic = new ItemInfo("MyObjectBuilder_Component", "Tech16x", new int[] { 200050, 100000, 562700, 1, 2510, 500100, 150000, 12501, 0, 0, 0, 0, 0 });
                public static ItemInfo Tellurium = new ItemInfo("MyObjectBuilder_Component", "Tech32x", new int[] { 400100, 200010, 1125650, 2, 5021, 1000200, 300050, 25012, 0, 0, 0, 0, 0 });
            }

            public static Dictionary<InventoryItem, ItemInfo> BuildItemInfoMap()
            {
                Dictionary<InventoryItem, ItemInfo> result = new Dictionary<InventoryItem, ItemInfo>
                {
                    { InventoryItem.Cobalt, InventoryItemData.Cobalt },
                    { InventoryItem.Gold, InventoryItemData.Gold },
                    { InventoryItem.Iron, InventoryItemData.Iron },
                    { InventoryItem.Magnesium, InventoryItemData.Magnesium },
                    { InventoryItem.Platinum, InventoryItemData.Platinum },
                    { InventoryItem.Silicon, InventoryItemData.Silicon },
                    { InventoryItem.Silver, InventoryItemData.Silver },
                    { InventoryItem.Uranium, InventoryItemData.Uranium },
                    { InventoryItem.Common, InventoryItemData.Common },
                    { InventoryItem.Rare, InventoryItemData.Rare },
                    { InventoryItem.Exotic, InventoryItemData.Exotic },
                    { InventoryItem.Prosonic, InventoryItemData.Prosonic },
                    { InventoryItem.Tellurium, InventoryItemData.Tellurium }
                };
                return result;
            }

            public static Dictionary<InventoryItem, int> BuildTotalExistingEmpty() // IngotsOnlyRecipe. Not counting existing tech for crafting yet.
            {
                Dictionary<InventoryItem, int> result = new Dictionary<InventoryItem, int>
                {
                    { InventoryItem.Cobalt, 0 },
                    { InventoryItem.Gold, 0 },
                    { InventoryItem.Iron, 0 },
                    { InventoryItem.Magnesium, 0 },
                    { InventoryItem.Platinum, 0 },
                    { InventoryItem.Silicon, 0 },
                    { InventoryItem.Silver, 0 },
                    { InventoryItem.Uranium, 0 }
                };
                return result;
            }
        }
    }
}