using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {
        public class IngotOnlyTechRecipe
        {
            public int[] ValueArray;
            public Dictionary<InventoryItem, int> Ingredients;

            public IngotOnlyTechRecipe(int[] _values = null)
            {
                ValueArray = _values;
                Ingredients = InitRecipe(_values);
            }

            private static Dictionary<InventoryItem, int> InitRecipe(int[] _valueArray)
            {
                Dictionary<InventoryItem, int> result = BuildEmptyIngredientsDictionary(); // Only ingots in result dict.
                if (_valueArray == null) return result;

                for (int i = 0; i < result.Count; i++) // Ingots only.
                {
                    result[(InventoryItem)i] = _valueArray[i];
                }

                return result;
            }

            public static Dictionary<InventoryItem, int> BuildEmptyIngredientsDictionary() // IngotsOnlyRecipe. Not counting existing tech for crafting yet.
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