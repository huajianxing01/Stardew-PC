using UnityEngine;

public class Item : MonoBehaviour
{
    //序列化私有字段itemCode在Inspector面板中显示，默认有值，并显示对应的description
    [ItemCodeDescription][SerializeField] private int _itemCode;
    private SpriteRenderer spriteRenderer;

    public int ItemCode {  get { return _itemCode; } set {  _itemCode = value; } }

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if (ItemCode != 0)
        {
            Init(ItemCode);
        }
    }

    public void Init( int itemCodeParam)
    {
        if(itemCodeParam != 0)
        {
            ItemCode = itemCodeParam;

            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(ItemCode);
            spriteRenderer.sprite = itemDetails.itemSprite;

            if(itemDetails.itemType == ItemType.Reapable_scenery)
            {
                gameObject.AddComponent<ItemNudge>();
            }
        }
    }
}
