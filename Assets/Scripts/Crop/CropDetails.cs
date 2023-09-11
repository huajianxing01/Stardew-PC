using System;
using UnityEngine;

[System.Serializable]
public class CropDetails
{
    [ItemCodeDescription] public int seedItemCode;
    public int[] growthDays;//不同阶段的生长天数
    public int totalGrowthDays;//总的生长天数
    public GameObject[] growthPrefab;//不同阶段的prefab
    public Sprite[] growthSprite;//不同阶段的sprite
    public Season[] seasons;
    public Sprite harvestedSprites;//收获时的农作物

    //某些作物变成另一种作物时用，例如树(果实和木柴)->树桩(木柴)
    [ItemCodeDescription] public int harvestedTransformItemCode;
    public bool hideCropBeforeHarvestedAnimation;
    public bool disableCropCollidersBeforeHarvestedAnimation;
    public bool isHarvestedAnimation;//农作物最后阶段收获时是否有收获动画
    public bool isHarvestActionEffect = false;
    public bool spawnCropProducedAtPlayerPosition;
    public HarvestActionEffect harvestActionEffect;

    //农作物收获时可用的不同工具，如果是空的则任何工具都能用
    [ItemCodeDescription] public int[] harvestToolItemCode;
    public int[] requiredHarvestAction;//农作物收获时不同工具要求的收获动作次数
    [ItemCodeDescription] public int[] cropProducedItemCode;//农作物产出有几种收获
    public int[] cropProducedMinQuantity;//农作物不同收获的最少产量
    public int[] cropProducedMaxQuantity;//农作物不同收获的最多产量，在min&max中取一个随机数
    public int daysToRegrow;//重新生长的时间or单季作物则是-1

    public bool CanUseToolToHarvestCrop(int toolItemCode)
    {
        if(RequiredHarvestActionForTool(toolItemCode) == -1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public int RequiredHarvestActionForTool(int toolItemCode)
    {
        for(int i = 0; i < harvestToolItemCode.Length; i++)
        {
            if (harvestToolItemCode[i] == toolItemCode)
            {
                return requiredHarvestAction[i];
            }
        }
        return -1;
    }
}
