using System.Collections.Generic;
using UnityEngine;

public class PauseMenuInventoryManagement : MonoBehaviour
{
    public GameObject inventoryManagementDraggedItemPrefab;
    [SerializeField] private PauseMenuInventoryManagenentSlot[] inventoryManagenentSlots = null;
    [SerializeField] private Sprite transparent16x16 = null;
    [HideInInspector] public GameObject inventoryTextBoxGameobject;

    private void OnEnable()
    {
        EventHandler.InventoryUpdatedEvent += PopulatePlayerInventory;
        if (InventoryManager.Instance != null)
        {
            PopulatePlayerInventory(InventoryLocation.player, InventoryManager.Instance.inventoryLists[(int)InventoryLocation.player]);
        }
    }

    private void OnDisable()
    {
        EventHandler.InventoryUpdatedEvent -= PopulatePlayerInventory;
        DestroyInventoryTextBoxGameobject();
    }

    public void DestroyInventoryTextBoxGameobject()
    {
        if (inventoryTextBoxGameobject != null)
        {
            Destroy(inventoryTextBoxGameobject);
        }
    }

    public void DestroyCurrentDraggedItems()
    {
        for (int i = 0; i < InventoryManager.Instance.inventoryLists[(int)InventoryLocation.player].Count; i++) 
        {
            if (inventoryManagenentSlots[i].draggedItem != null)
            {
                Destroy(inventoryManagenentSlots[i].draggedItem);
            }
        }
    }

    private void PopulatePlayerInventory(InventoryLocation location, List<InventoryItem> list)
    {
        if(location == InventoryLocation.player)
        {
            InitializeInventoryManagementSlots();
            //循环player的仓库item
            for(int i = 0; i < InventoryManager.Instance.inventoryLists[(int)InventoryLocation.player].Count; i++)
            {
                inventoryManagenentSlots[i].itemDetails = InventoryManager.Instance.GetItemDetails(list[i].itemCode);
                inventoryManagenentSlots[i].itemQuantity = list[i].itemQuantity;

                if (inventoryManagenentSlots[i].itemDetails != null)
                {
                    inventoryManagenentSlots[i].inventoryManagementSlotImage.sprite = inventoryManagenentSlots[i].itemDetails.itemSprite;
                    inventoryManagenentSlots[i].textMeshProUGUI.text = inventoryManagenentSlots[i].itemQuantity.ToString();
                }
            }
        }
    }

    private void InitializeInventoryManagementSlots()
    {
        //清除所有物品槽
        for(int i = 0; i < Settings.playerMaxInventoryCapacity; i++)
        {
            inventoryManagenentSlots[i].greyedOutImageGO.SetActive(false);
            inventoryManagenentSlots[i].itemDetails = null;
            inventoryManagenentSlots[i].itemQuantity = 0;
            inventoryManagenentSlots[i].inventoryManagementSlotImage.sprite = transparent16x16;
            inventoryManagenentSlots[i].textMeshProUGUI.text = string.Empty;
        }
        //超过玩家初始物品槽数量外的其他物品槽，设为灰色
        for(int i = InventoryManager.Instance.inventoryListCapacityIntArray[(int)InventoryLocation.player];
            i < Settings.playerMaxInventoryCapacity; i++)
        {
            inventoryManagenentSlots[i].greyedOutImageGO.SetActive(true);
        }
    }
}
