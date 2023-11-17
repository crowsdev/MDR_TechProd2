using System;
using System.Collections.Generic;
using VRage.Game.ModAPI.Ingame;

namespace IngameScript
{
    partial class Program
    {
        public class ItemInfo
        {
            public string TypeName;
            public string SubTypeName;
            public bool IsTech;
            public IngotOnlyTechRecipe Recipe;
            public int CanCraftMax;
            public string IngredientMostNeeded;

            public ItemInfo(string _type, string _stype, int[] _valueArray = null)
            {
                TypeName = _type;
                SubTypeName = _stype;
                Recipe = new IngotOnlyTechRecipe(_valueArray);
                IsTech = _valueArray != null;
                CanCraftMax = 0;
                IngredientMostNeeded = "NotCalculated.";
            }

            public static ItemInfo FromInventoryItem(MyInventoryItem _invItem)
            {
                return new ItemInfo(_invItem.Type.TypeId, _invItem.Type.SubtypeId);
            }

            public void CalculateMaxCraftable(Dictionary<InventoryItem, int> _totalExistingDict, Dictionary<InventoryItem, ItemInfo> _itemDataMap)
            {
                int result = int.MaxValue;
                string neededIngredient = "";
                for (int i = 0; i < Recipe.Ingredients.Count; i++)
                {
                    int canCraft;
                    InventoryItem ii = (InventoryItem)i;
                    int perUnit = Recipe.Ingredients[ii];
                    if (perUnit == 0) { continue; }
                    int inStock = _totalExistingDict[ii];
                    canCraft = (int)Math.Floor((double)inStock / (double)perUnit);
                    if (canCraft < result)
                    {
                        result = canCraft;
                        neededIngredient = _itemDataMap[ii].ToString();
                    }
                }

                CanCraftMax = result;
                IngredientMostNeeded = neededIngredient;
            }

            public override string ToString()
            {
                string result = SubTypeName;
                if (SubTypeName.StartsWith("Tech"))
                {
                    switch (SubTypeName)
                    {
                        case "Tech2x":
                        {
                            result = "Common";
                            break;
                        }
                        case "Tech4x":
                        {
                            result = "Rare";
                            break;
                        }
                        case "Tech8x":
                        {
                            result = "Exotic";
                            break;
                        }
                        case "Tech16x":
                        {
                            result = "Prosonic";
                            break;
                        }
                        case "Tech32x":
                        {
                            result = "Tellurium";
                            break;
                        }
                    }
                }

                return result;
            }
        }
    }
}