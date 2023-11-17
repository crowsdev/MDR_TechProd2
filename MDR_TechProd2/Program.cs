using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        #region Properties.

        public UpdateFrequency Frequency { get; set; } = UpdateFrequency.Update100;
        public Dictionary<InventoryItem, ItemInfo> ItemDataMap { get; set; }
        public Dictionary<InventoryItem, int> TotalExistingItems { get; set; }
        public Dictionary<InventoryItem, ItemInfo> TechItemInfoMap { get; set; }

        #endregion

        #region Lists

        public List<IMyInventoryOwner> InventoryOwners;

        #endregion

        #region Display Results.

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

        #endregion

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

        #region MaffBwain

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

        #endregion
    }
}
