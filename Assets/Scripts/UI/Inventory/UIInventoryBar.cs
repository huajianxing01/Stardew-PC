using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryBar : MonoBehaviour
{
    [SerializeField] private Sprite blank16x16Sprite = null;
    //用来存储每一个物品槽的东西，以便后期对Bar的Slot进行操作
    [SerializeField] private UIInventorySlot[] inventorySlot = null;
    [SerializeField] public GameObject inventoryTextBoxGameobject;
    public GameObject inventoryBarDraggedItem;

    private RectTransform rectTransform;
    private bool _isInventoryBarPositionBottom = true;

    public bool IsInventoryBarPositionBottom { get => _isInventoryBarPositionBottom;
        set => _isInventoryBarPositionBottom = value;}


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        SwitchInventoryBarPosition();
    }

    private void OnEnable()
    {
        EventHandler.InventoryUpdatedEvent += InventoryUpDated;
    }

    private void OnDisable()
    {
        EventHandler.InventoryUpdatedEvent -= InventoryUpDated;
    }

    private void SwitchInventoryBarPosition()
    {
        //获取player在View窗口的坐标，世界坐标已在Player类中转换处理
        Vector3 vector3 = Player.Instance.GetPlayerViewportPosition();
        
        if (vector3.y > 0.3f && _isInventoryBarPositionBottom == false)
        {
            rectTransform.pivot = new Vector2(0.5f, 0f);
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.anchoredPosition = new Vector2(0, 2.5f);

            IsInventoryBarPositionBottom = true;
        }
        else if(vector3.y <=0.3f && _isInventoryBarPositionBottom == true)
        {
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0, -2.5f);

            IsInventoryBarPositionBottom = false;
        }
         
    }

    private void InventoryUpDated(InventoryLocation Location, List<InventoryItem> List)
    {
        if(Location == InventoryLocation.player)
        {
            //判断存储在玩家背包里后，重新绘制物品槽
            ClearInventorySlots();
            
            if(inventorySlot.Length > 0 && List.Count > 0)
            {
                for(int i = 0; i < inventorySlot.Length; i++)
                {
                    if (i < List.Count)
                    {
                        int itemCode = List[i].itemCode;
                        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);

                        if(itemDetails != null)
                        {
                            inventorySlot[i].inventorySlotImage.sprite = itemDetails.itemSprite;
                            inventorySlot[i].TextMeshProUGUI.text = List[i].itemQuantity.ToString();
                            inventorySlot[i].itemDetails = itemDetails;
                            inventorySlot[i].itemQuantity = List[i].itemQuantity;

                            SetHighlightedInventorySlots(i);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    private void ClearInventorySlots()
    {
        if(inventorySlot.Length > 0)
        {
            for (int i = 0; i < inventorySlot.Length; i++)
            {
                inventorySlot[i].inventorySlotImage.sprite = blank16x16Sprite;
                inventorySlot[i].TextMeshProUGUI.text = string.Empty;
                inventorySlot[i].itemDetails = null;
                inventorySlot[i].itemQuantity = 0;

                SetHighlightedInventorySlots(i);
            }
        }
    }

    public void SetHighlightedInventorySlots()
    {
        if(inventorySlot.Length > 0)
        {
            for(int i = 0; i < inventorySlot.Length; i++)
            {
                SetHighlightedInventorySlots(i);
            }
        }
    }

    public void SetHighlightedInventorySlots(int location)
    {
        if(inventorySlot.Length > 0 && inventorySlot[location].itemDetails != null)
        {
            if (inventorySlot[location].isSelected)
            {
                inventorySlot[location].inventorySlotHighlight.color = new Color(1, 1, 1, 1);
                InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player,
                    inventorySlot[location].itemDetails.itemCode);
            }
        }
    }

    public void ClearHighlightOnInventorySlots()
    {
        if(inventorySlot.Length > 0)
        {
            for(int i = 0;i < inventorySlot.Length; i++)
            {
                if (inventorySlot[i].isSelected)
                {
                    inventorySlot[i].isSelected = false;
                    inventorySlot[i].inventorySlotHighlight.color = new Color(0,0,0,0);
                    InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);
                }
            }
        }
    }
}
