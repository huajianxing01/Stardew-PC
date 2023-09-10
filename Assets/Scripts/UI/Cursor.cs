using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cursor : MonoBehaviour
{
    private Canvas canvas;
    private Camera mainCamera;
    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTransform;
    [SerializeField] private Sprite greenCursorSprite = null;
    [SerializeField] private Sprite transparentCursorSprite = null;
    [SerializeField] private GridCursor gridCursor = null;

    private bool _cursorPositionIsValid = false;
    public bool CursorPositionIsValid { get => _cursorPositionIsValid; set => _cursorPositionIsValid = value; }

    private float _itemUseRadius = 0;
    public float ItemUseRadius { get => _itemUseRadius; set => _itemUseRadius = value; }

    private ItemType _selectedItemType;
    public ItemType SelectedItemType { get => _selectedItemType; set => _selectedItemType = value; }

    private bool _cursorIsEnable = false;
    public bool CursorIsEnable { get => _cursorIsEnable; set => _cursorIsEnable = value; }

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (CursorIsEnable)
        {
            DisplayCursor();
        }
    }

    private void DisplayCursor()
    {
        //��ù���������λ��
        Vector3 cursorPosition = GetWorldPositionForCursor();
        //�жϹ���Ƿ���palyer�ķ�Χ��
        SetCursorValidity(cursorPosition, Player.Instance.GetPlayerCenterPosition());
        //��ù���recttransformλ��
        cursorRectTransform.position = GetRectTransformPositionForCursor();
    }

    private void SetCursorValidity(Vector3 cursorPosition, Vector3 playerPosition)
    {
        SetCursorToValid();
        //λ�ھŹ����ĸ�������ģ��ض����ڷ�Χ��
        if (cursorPosition.x > (playerPosition.x + ItemUseRadius / 2) && cursorPosition.y > (playerPosition.y + ItemUseRadius / 2)
            || cursorPosition.x > (playerPosition.x + ItemUseRadius / 2) && cursorPosition.y < (playerPosition.y - ItemUseRadius / 2)
            || cursorPosition.x < (playerPosition.x - ItemUseRadius / 2) && cursorPosition.y > (playerPosition.y + ItemUseRadius / 2)
            || cursorPosition.x < (playerPosition.x - ItemUseRadius / 2) && cursorPosition.y < (playerPosition.y - ItemUseRadius / 2))
        {
            SetCursorToInvalid();
            return;
        }
        //���Ź������ڵģ��ж��Ƿ���radius��
        if (Mathf.Abs(cursorPosition.x - playerPosition.x) > ItemUseRadius
            || Mathf.Abs(cursorPosition.y - playerPosition.y) > ItemUseRadius)
        {
            SetCursorToInvalid();
            return;
        }

        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);
        //�ж���Ʒ�����Ƿ�Ϊ��
        if (itemDetails == null)
        {
            SetCursorToInvalid();
            return;
        }

        switch (itemDetails.itemType)
        {
            case ItemType.Watering_tool:
            case ItemType.Hoeing_tool:
            case ItemType.Chopping_tool:
            case ItemType.Breaking_tool:
            case ItemType.Collecting_tool:
            case ItemType.Reaping_tool:
                if (!SetCursorValidityTool(cursorPosition, playerPosition, itemDetails))
                {
                    SetCursorToInvalid();
                    return;
                }
                break;
            case ItemType.none:
                break;
            case ItemType.count:
                break;
            default:
                break;
        }

    }

    private bool SetCursorValidityTool(Vector3 cursorPosition, Vector3 playerPosition, ItemDetails itemDetails)
    {
        switch(itemDetails.itemType)
        {
            case ItemType.Reaping_tool:
                return SetCursorValidityReapingTool(cursorPosition, playerPosition, itemDetails);
            default:
                return false;
        }
    }

    //�жϹ��Ӵ��������Ƿ���ո�
    private bool SetCursorValidityReapingTool(Vector3 cursorPosition, Vector3 playerPosition, ItemDetails itemDetails)
    {
        List<Item> itemsList = new List<Item>();
        if (HelperMethods.GetComponentsAtCursorLocation<Item>(out itemsList, cursorPosition))
        {
            if(itemsList.Count != 0)
            {
                foreach(var item in itemsList)
                {
                    if(InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType == ItemType.Reapable_scenary)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void SetCursorToValid()
    {
        //���sprite��Ϊ��ɫ�����λ������Ч��
        cursorImage.sprite = greenCursorSprite;
        CursorPositionIsValid = true;
        //�ر�grid�Ĺ��
        gridCursor.DisableCursor();
    }

    private void SetCursorToInvalid()
    {
        cursorImage.sprite = transparentCursorSprite;
        CursorPositionIsValid = false;

        gridCursor.EnableCursor();
    }

    public Vector3 GetWorldPositionForCursor()
    {
        //input.mouseposition��input.gettouch����Ļ���꣬��Ҫת��Ϊ����λ��
        Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }

    public Vector2 GetRectTransformPositionForCursor()
    {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        return RectTransformUtility.PixelAdjustPoint(screenPosition, cursorRectTransform, canvas);
    }

    public void DisableCursor()
    {
        cursorImage.color = new Color(1, 1, 1, 0);
        CursorIsEnable = false;
    }

    public void EnableCursor()
    {
        cursorImage.color = new Color(1, 1, 1, 1);
        CursorIsEnable = true;
    }
}
