using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();

        if (item != null)
        {
            //获得物体详细数据
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(item.ItemCode);
            //Debug.Log(itemDetails.itemDescription+'\n'+itemDetails.itemLongDescription);

            if(itemDetails.canBePickedUp)
            {
                InventoryManager.Instance.AddItem(InventoryLocation.player, item, collision.gameObject);
            }
        }
    }
}
