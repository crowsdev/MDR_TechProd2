/*
 * R e a d m e
 * -----------
 * 
 * In this file you can include any instructions or other comments you want to have injected onto the 
 * top of your final script. You can safely delete this file if you do not want any such comments.
 */


public UpdateFrequency Frequency { get; set; } = UpdateFrequency.Update100;
public Dictionary<InventoryItem, ItemInfo> ItemDataMap { get; set; }
public Dictionary<InventoryItem, int> TotalExistingItems { get; set; }
public Dictionary<InventoryItem, ItemInfo> TechItemInfoMap { get; set; }



public List<IMyInventoryOwner> InventoryOwners;



public string ResultsPage
{
    get
    {
        if (ResultsList == null || ResultsList.Count == 0)
        {
            return "Calculating....";
        }
        return string.Join("\n", ResultsList);
    }
}
public List<string> ResultsList;


public Program()
{
    Runtime.UpdateFrequency = Frequency;
    ItemDataMap = MyData.BuildItemInfoMap();
    TotalExistingItems = MyData.BuildTotalExistingEmpty();
    InventoryOwners = GetInventoryOwners();
    ResultsList = new List<string>();
}

public void Save()
{
}

public void Main(string argument, UpdateType updateSource)
{
    ResultsList = new List<string>();
    InventoryOwners = GetInventoryOwners();
    TotalExistingItems = GetExistingItemTotal();
    ResultsList.Add("[+]----Tech-Production-Capacity-----[+] ");
    for (int i = 0; i < ItemDataMap.Count; i++)
    {
        InventoryItem invItem = (InventoryItem)i;
        ItemInfo itemInfo = ItemDataMap[invItem];

        if (!itemInfo.IsTech) continue;

        itemInfo.CalculateMaxCraftable(TotalExistingItems, ItemDataMap);

        ResultsList.Add($"[+]---------------MDR---------------[+]");
        ResultsList.Add($"[+] Tech = {itemInfo}");
        ResultsList.Add($"[+] Max = {itemInfo.CanCraftMax}");
        ResultsList.Add($"[+] Need More = {itemInfo.IngredientMostNeeded}");
        ResultsList.Add($"[+]---------------MDR---------------[+]");
    }

    Echo(ResultsPage);
    this.Me.CustomData = ResultsPage;
}


public List<IMyInventoryOwner> GetInventoryOwners()
{
    List<IMyInventoryOwner> result = new List<IMyInventoryOwner>();
    this.GridTerminalSystem.GetBlocksOfType<IMyInventoryOwner>(result);
    return result;
}

public Dictionary<InventoryItem, int> GetExistingItemTotal()
{
    var result = MyData.BuildTotalExistingEmpty();

    foreach (var invOwner in InventoryOwners) // Iterate through all blocks that have inventory.
    {
        for (int i = 0; i < invOwner.InventoryCount; i++) // Iterate through all inventory spaces in block. some have multiple.
        {
            var inv = invOwner.GetInventory(i);
            if (inv == null || inv.ItemCount == 0) // Check if inventory is empty and ignore it.
            {
                continue;
            }

            List<MyInventoryItem> items = new List<MyInventoryItem>();
            inv.GetItems(items);
            foreach (var item in items) // Iterate through all items in current inventory space.
            {
                for (int j = 0; j < result.Count; j++) // Check for each ingredient item.
                {
                    if (item.Type.SubtypeId.Equals(ItemDataMap[(InventoryItem)j].SubTypeName)) // Identify ingredient items.
                    {
                        if (item.Type.TypeId.Equals(ItemDataMap[(InventoryItem)j].TypeName))
                        {
                            result[(InventoryItem)j] += item.Amount.ToIntSafe(); // Accumulative addition of current items amount to total so far.
                        }
                    }
                }
            }
        }
    }

    return result;
}

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

public enum InventoryItem : int
{
    Cobalt = 0,
    Gold,
    Iron,
    Magnesium,
    Platinum,
    Silicon,
    Silver,
    Uranium,
    Common, // Tech2x
    Rare, // Tech4x
    Exotic, // Tech8x
    Prosonic, // Tech16x
    Tellurium, // Tech32x
}

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