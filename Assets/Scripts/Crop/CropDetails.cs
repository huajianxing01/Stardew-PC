using System;
using UnityEngine;

[System.Serializable]
public class CropDetails
{
    [ItemCodeDescription] public int seedItemCode;
    public int[] growthDays;//��ͬ�׶ε���������
    public int totalGrowthDays;//�ܵ���������
    public GameObject[] growthPrefab;//��ͬ�׶ε�prefab
    public Sprite[] growthSprite;//��ͬ�׶ε�sprite
    public Season[] seasons;
    public Sprite harvestedSprites;//�ջ�ʱ��ũ����

    //ĳЩ��������һ������ʱ�ã�������(��ʵ��ľ��)->��׮(ľ��)
    [ItemCodeDescription] public int harvestedTransformItemCode;
    public bool hideCropBeforeHarvestedAnimation;
    public bool disableCropCollidersBeforeHarvestedAnimation;
    public bool isHarvestedAnimation;//ũ�������׶��ջ�ʱ�Ƿ����ջ񶯻�
    public bool isHarvestActionEffect = false;
    public bool spawnCropProducedAtPlayerPosition;
    public HarvestActionEffect harvestActionEffect;

    //ũ�����ջ�ʱ���õĲ�ͬ���ߣ�����ǿյ����κι��߶�����
    [ItemCodeDescription] public int[] harvestToolItemCode;
    public int[] requiredHarvestAction;//ũ�����ջ�ʱ��ͬ����Ҫ����ջ�������
    [ItemCodeDescription] public int[] cropProducedItemCode;//ũ��������м����ջ�
    public int[] cropProducedMinQuantity;//ũ���ﲻͬ�ջ�����ٲ���
    public int[] cropProducedMaxQuantity;//ũ���ﲻͬ�ջ������������min&max��ȡһ�������
    public int daysToRegrow;//����������ʱ��or������������-1

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
