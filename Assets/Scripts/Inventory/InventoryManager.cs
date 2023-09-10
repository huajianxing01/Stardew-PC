using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

//创建一个游戏库存管理者，管理游戏所有Item的信息
public class InventoryManager : SingletonMonobehaviour<InventoryManager>
{
    private Dictionary<int, ItemDetails> itemDetailsDictionary;
    public List<InventoryItem>[] inventoryLists;
    //select数组index对应inventoryLists[x]的索引，value是itemCode
    private int[] selectInventoryItem;
    //数组index是列表inventoryLists的索引，value是容量
    [HideInInspector] public int[] inventoryListCapacityIntArray;
    [SerializeField] private SO_ItemList itemList = null;

    protected override void Awake()
    {
        //不能放在Star中，会报错，Unity中无法确定哪个Object先Star
        //生命周期里Awake在Star之前，确保InventoryManager在Item调用其前就创建
        base.Awake();

        CreateInventoryLists();
        CreateItemDetailsDictionary();

        selectInventoryItem = new int[(int)InventoryLocation.count];
        foreach (int i in selectInventoryItem)
        {
            selectInventoryItem[i] = -1;
        }
    }

    private void CreateInventoryLists()
    {
        inventoryLists = new List<InventoryItem>[(int)InventoryLocation.count];

        for (int i = 0; i < (int)InventoryLocation.count; i++)
        {
            inventoryLists[i] = new List<InventoryItem>();
        }
        //实例化一个长度为count的Array
        inventoryListCapacityIntArray = new int[(int)InventoryLocation.count];
        //初始化玩家背包Array[0]的容量值
        inventoryListCapacityIntArray[(int)InventoryLocation.player] = Settings.playerInitialInventoryCapacity;
    }

    private void CreateItemDetailsDictionary()
    {
        itemDetailsDictionary = new Dictionary<int, ItemDetails>();
        //遍历存储ItemDetail的List，添加到Dictionary中
        foreach(ItemDetails itemDetails in itemList.ItemDetails)
        {
            itemDetailsDictionary.Add(itemDetails.itemCode, itemDetails);
        }
    }

    public ItemDetails GetItemDetails(int itemCode)
    {
        ItemDetails itemDetails;
        //查询itemCode是否在Dictionary中，是则返回对应的itemDetail
        if(itemDetailsDictionary.TryGetValue(itemCode, out itemDetails))
        {
            return itemDetails;
        }
        else
        {
            return null;
        }
    }

    public void AddItem(InventoryLocation location,Item item,GameObject deleteGameObject)
    {
        AddItem(location, item);
        //增加item后，销毁screne上的item
        Destroy(deleteGameObject);
        //foreach(InventoryItem i in inventoryLists[0])
        //{
        //    int index = inventoryLists[0].IndexOf(i);
        //    Debug.Log(GetItemDetails(inventoryLists[0][index].itemCode).itemDescription + " " + inventoryLists[0][index].itemQuantity);
        //}
    }

    //Add item到List里
    public void AddItem(InventoryLocation location,Item item)
    {
        int itemCode = item.ItemCode;
        List<InventoryItem> inventoryList = inventoryLists[(int)location];
        
        //检查List上是否已经含有item
        int itemPosition = FindItemInInventory(location, itemCode);

        if(itemPosition != -1)
        {
            AddItemPosition(inventoryList, itemCode, itemPosition);
        }
        else
        {
            AddItemPosition(inventoryList,itemCode);
        }

        EventHandler.CallInventoryUpdatedEvent(location, inventoryLists[(int)location]);
    }

    public int FindItemInInventory(InventoryLocation location, int itemCode)
    {
        List<InventoryItem> inventoryItems = inventoryLists[(int)location];

        foreach(InventoryItem item in inventoryItems)
        {
            if (item.itemCode == itemCode)
            {
                return inventoryItems.IndexOf(item);
            }
        }
        return -1;
    }

    private void AddItemPosition(List<InventoryItem> inventoryList, int itemCode)
    {
        InventoryItem inventoryItem = new InventoryItem();

        inventoryItem.itemCode = itemCode;
        inventoryItem.itemQuantity = 1;

        inventoryList.Add(inventoryItem);
    }

    private void AddItemPosition(List<InventoryItem> inventoryList, int itemCode,int itemPosition)
    {
        InventoryItem inventoryItem = new InventoryItem();
        int quantity = inventoryList[itemPosition].itemQuantity + 1;

        inventoryItem.itemCode = itemCode;
        inventoryItem.itemQuantity = quantity;

        inventoryList[itemPosition] = inventoryItem;
    }

    public void RemoveItem(InventoryLocation location, int itemCode)
    {
        List<InventoryItem> itemList = inventoryLists[(int)location];
        int itemPosition = FindItemInInventory(location, itemCode);

        if (itemPosition != -1)
        {
            RemoveItemAtPosition(itemList, itemCode, itemPosition);
        }

        EventHandler.CallInventoryUpdatedEvent(location, inventoryLists[(int)location]);
    }

    private void RemoveItemAtPosition(List<InventoryItem> itemList, int itemCode, int itemPosition)
    {
        InventoryItem newItem = new InventoryItem();
        int quantity = itemList[itemPosition].itemQuantity - 1;

        if(quantity > 0)
        {
            newItem.itemCode = itemCode;
            newItem.itemQuantity = quantity;
            itemList[itemPosition] = newItem;
        }
        else
        {
            itemList.RemoveAt(itemPosition);
        }
    }

    public void SwapInventoryItem(InventoryLocation location,int itemA, int itemB)
    {
        if(itemA < inventoryLists[(int)location].Count && itemB < inventoryLists[(int)location].Count
            && itemA != itemB && itemA >= 0 && itemB >= 0)
        {
            InventoryItem inventoryItemA = inventoryLists[(int)location][itemA];
            InventoryItem inventoryItemB = inventoryLists[(int)location][itemB];

            inventoryLists[(int)location][itemB] = inventoryItemA;
            inventoryLists[(int)location][itemA] = inventoryItemB;

            EventHandler.CallInventoryUpdatedEvent(location, inventoryLists[(int)location]);
        }
    }

    public string GetItemTypeDescription(ItemType itemType)
    {
        string itemTypeDescription;

        switch (itemType)
        {
            case ItemType.Breaking_tool:
                itemTypeDescription = Settings.BreakingTool;
                break;
            case ItemType.Chopping_tool:
                itemTypeDescription = Settings.ChoppingTool;
                break;
            case ItemType.Watering_tool:
                itemTypeDescription = Settings.WateringTool;
                break;
            case ItemType.Hoeing_tool:
                itemTypeDescription = Settings.HoeingTool;
                break;
            case ItemType.Collecting_tool:
                itemTypeDescription = Settings.CollectingTool;
                break;
            case ItemType.Reaping_tool:
                itemTypeDescription = Settings.ReapingTool;
                break;
            case ItemType.Seed:
                itemTypeDescription = Settings.Seed;
                break;
            case ItemType.Commodity:
                itemTypeDescription = Settings.Commodity;
                break;
            case ItemType.Funityre:
                itemTypeDescription = Settings.Funityre;
                break;
            case ItemType.Reapable_scenary:
                itemTypeDescription = Settings.OtherItem;
                break;
            default:
                itemTypeDescription = itemType.ToString();
                break;
        }
        return itemTypeDescription;
    }

    public void SetSelectedInventoryItem(InventoryLocation location,int code)
    {
        selectInventoryItem[(int)location] = code;
    }

    public void ClearSelectedInventoryItem(InventoryLocation location)
    {
        selectInventoryItem[(int)location] = -1;
    }

    private int GetSelectedInventoryItem(InventoryLocation location)
    {
        return selectInventoryItem[(int)location];
    }

    public ItemDetails GetSelectedInventoryItemDetails(InventoryLocation location)
    {
        int itemCode = GetSelectedInventoryItem(location);
        if (itemCode == -1)
        {
            return null;
        }
        else
        {
            return GetItemDetails(itemCode);
        }
    }
}
