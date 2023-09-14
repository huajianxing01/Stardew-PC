using System.Collections;
using UnityEngine;

public class Crop : MonoBehaviour
{
    private int harvestActionCount = 0;
    [HideInInspector] public Vector2Int cropGridPosition;
    [Tooltip("需来有对应子对象")]
    [SerializeField] private SpriteRenderer cropHarvestedSpriteRender = null;
    [Tooltip("需要子对象有对应特效")]
    [SerializeField] private Transform harvestActionEffectTransform = null;

    public void ProcessToolAction(ItemDetails equippedItemDetails, bool isToolRight, bool isToolLeft, bool isToolUp, bool isToolDown)
    {
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);
        if (gridPropertyDetails == null) return;

        ItemDetails seedItemDetails = InventoryManager.Instance.GetItemDetails(gridPropertyDetails.seedItemCode);
        if (seedItemDetails == null) return;

        CropDetails cropDetails = GridPropertiesManager.Instance.GetCropDetails(seedItemDetails.itemCode);
        if (cropDetails == null) return;

        Animator animator = GetComponentInChildren<Animator>();
        if(animator != null)
        {
            if (isToolRight || isToolUp)
            {
                animator.SetTrigger("usetoolright");
            }
            else if (isToolLeft || isToolDown)
            {
                animator.SetTrigger("usetoolleft");
            }
        }
        //触发收获动作的特效
        if (cropDetails.isHarvestActionEffect)
        {
            EventHandler.CallHarvestActionEffectEvent(harvestActionEffectTransform.position, cropDetails.harvestActionEffect);
        }

        harvestActionCount++;
        int requiredHarvestActions = cropDetails.RequiredHarvestActionForTool(equippedItemDetails.itemCode);
        if (harvestActionCount >= requiredHarvestActions)
        {
            HarvestCrop(isToolRight, isToolUp, cropDetails, gridPropertyDetails, animator);
        }
    }

    private void HarvestCrop(bool isToolRight, bool isToolUp, CropDetails cropDetails, GridPropertyDetails gridPropertyDetails, Animator animator)
    {
        if (cropDetails.isHarvestedAnimation && animator != null)
        {
            if(cropDetails.harvestedSprites != null)
            {
                if(cropHarvestedSpriteRender != null)
                {
                    cropHarvestedSpriteRender.sprite = cropDetails.harvestedSprites;
                }
            }

            if (isToolRight || isToolUp)
            {
                animator.SetTrigger("harvestright");
            }
            else
            {
                animator.SetTrigger("harvestleft");
            }
        }
        //收获农作物后，重置grid属性
        gridPropertyDetails.seedItemCode = -1;
        gridPropertyDetails.growthDays = -1;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        //收获动画播放前隐藏地里的农作物
        if (cropDetails.hideCropBeforeHarvestedAnimation)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
        //收获动画播放前关掉碰撞器
        if (cropDetails.disableCropCollidersBeforeHarvestedAnimation)
        {
            Collider2D[] collider2Ds = GetComponentsInChildren<Collider2D>();
            foreach(var collider2D in collider2Ds)
            {
                collider2D.enabled = false;
            }

        }
        //播放收获动画
        if(cropDetails.isHarvestedAnimation && animator != null)
        {
            StartCoroutine(ProcessHarvestActionsAfterAnimaiton(cropDetails, gridPropertyDetails, animator));
        }
        else
        {
            HarvestAction(cropDetails, gridPropertyDetails);
        }
    }

    private IEnumerator ProcessHarvestActionsAfterAnimaiton(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails, Animator animator)
    {
        //获取当前动画状态机的状态，没到Harvested就一直等待
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Harvested"))
        {
            yield return null;
        }

        HarvestAction(cropDetails, gridPropertyDetails);
    }

    private void HarvestAction(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        SpawnHarvestedItems(cropDetails);

        if (cropDetails.harvestedTransformItemCode > 0)
        {
            CreateHarvestedTransformCrop(cropDetails, gridPropertyDetails);
        }

        Destroy(gameObject);
    }

    private void CreateHarvestedTransformCrop(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        gridPropertyDetails.seedItemCode = cropDetails.harvestedTransformItemCode;
        gridPropertyDetails.growthDays = 0;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);
        GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);
    }

    private void SpawnHarvestedItems(CropDetails cropDetails)
    {
        for(int i = 0; i < cropDetails.cropProducedItemCode.Length; i++)
        {
            int cropsToProduce;
            if (cropDetails.cropProducedMinQuantity[i] >= cropDetails.cropProducedMaxQuantity[i])
            {
                cropsToProduce = cropDetails.cropProducedMinQuantity[i];
            }
            else
            {
                cropsToProduce = Random.Range(cropDetails.cropProducedMinQuantity[i], cropDetails.cropProducedMaxQuantity[i] + 1);

            }

            for(int j = 0; j < cropsToProduce; j++)
            {
                Vector3 spawnPosition;
                if (cropDetails.spawnCropProducedAtPlayerPosition)
                {
                    InventoryManager.Instance.AddItem(InventoryLocation.player, cropDetails.cropProducedItemCode[i]);
                }
                else
                {
                    spawnPosition = new Vector3(transform.position.x + Random.Range(-1, 1), transform.position.y + Random.Range(-1, 1), 0);
                    SceneItemsManager.Instance.InstantiateSceneItem(cropDetails.cropProducedItemCode[i], spawnPosition);
                }
            }
        }
        
    }
}
