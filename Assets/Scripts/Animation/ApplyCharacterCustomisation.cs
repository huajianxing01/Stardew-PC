using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class colorSwap
{
    public Color fromColor;
    public Color toColor;

    public colorSwap(Color fromColor, Color toColor)
    {
        this.fromColor = fromColor;
        this.toColor = toColor;
    }
}

public class ApplyCharacterCustomisation : MonoBehaviour
{
    //输入纹理
    [Header("基础纹理")]
    [SerializeField] private Texture2D maleFarmerBaseTexture = null;
    [SerializeField] private Texture2D femaleFarmerBaseTexture = null;
    [SerializeField] private Texture2D shirtsBaseTexture = null;
    [SerializeField] private Texture2D hairBaseTexture = null;
    [SerializeField] private Texture2D hatsBaseTexture = null;
    [SerializeField] private Texture2D adornmentsBaseTexture = null;
    private Texture2D farmerBaseTexture;
    //创建纹理
    [Header("给要用的Animation输出基础纹理")]
    [SerializeField] private Texture2D farmerBaseCustomised = null;
    [SerializeField] private Texture2D hairCustomised = null;
    [SerializeField] private Texture2D hatsCustomised = null;
    private Texture2D farmerBaseShirtsUpdated;
    private Texture2D selectedShirt;
    private Texture2D farmerBaseAdornmentsUpdated;
    private Texture2D selectedAdornment;
    //unity编辑界面创建范围滑动器
    [Header("选择衬衫款式")][Range(0, 1)][SerializeField] public int inputShirtStyleNo = 0;
    [Header("选择性别，0：男性，1：女性")][Range(0, 1)][SerializeField] private int inputSex = 0;
    [Header("选择发型")][Range(0, 1)][SerializeField] public int inputHairStyleNo = 0;
    [Header("选择发型颜色")][SerializeField] private Color inputHairColor = Color.black;
    [Header("选择肤色")][Range(0, 3)][SerializeField] public int inputSkinStyleNo = 0;
    [Header("选择裤子颜色")][SerializeField] private Color inputTrouserColor = Color.yellow;
    [Header("选择帽子款式")][Range(0, 1)][SerializeField] public int inputHatsStyleNo = 0;
    [Header("选择装饰品")][Range(0, 3)][SerializeField] public int inputAdornmentsStyleNo = 0;

    //建立行数相同的规则二维数组
    private Facing[,] bodyFacingArray;
    private Vector2Int[,] bodyShirtOffsetArray;
    private Vector2Int[,] bodyAdornmentsOffsetArray;

    //用于身体更换的Sprite尺寸
    private int bodyRows = 21;
    private int bodyColumns = 6;
    private int farmerSpriteWidth = 16;
    private int farmerSpriteHeight = 32;
    //用于shirt更换的在Sprite的位置和尺寸
    private int shirtTextureWidth = 9;
    private int shirtTextureHeight = 36;
    private int shirtSpriteWidth = 9;
    private int shirtSpriteHeight = 9;
    private int shirtStylesNumberInSpriteWidth = 16;
    //Hair尺寸
    private int hairTextureWidth = 16;
    private int hairTextureHeight = 96;
    private int hairStylesNumberInSpriteWidth = 8;
    //Hats尺寸
    private int hatsTextureWidth = 20;
    private int hatsTextureHeight = 80;
    private int hatsStylesNumberInSpriteWidth = 12;
    //Adornment尺寸
    private int adornmentsTextureWidth = 16;
    private int adornmentsTextureHeight = 32;
    private int adornmentsSpriteWidth = 16;
    private int adornmentsSpriteHeight = 16;
    private int adornmentsStylesNumberInSpriteWidth = 8;

    private List<colorSwap> colorSwapList;
    //手臂颜色，最暗到最亮
    private Color32 armTargetColor1 = new Color32(77, 13, 13, 255);
    private Color32 armTargetColor2 = new Color32(138, 41, 41, 255);
    private Color32 armTargetColor3 = new Color32(172, 50, 50, 255);
    //身体肤色，最暗到最亮
    private Color32 skinTargetColor1 = new Color32(145, 117, 90, 255);  // darkest
    private Color32 skinTargetColor2 = new Color32(204, 155, 108, 255);  // next darkest
    private Color32 skinTargetColor3 = new Color32(207, 166, 128, 255);  // next darkest
    private Color32 skinTargetColor4 = new Color32(238, 195, 154, 255);  // lightest


    private void Awake()
    {
        colorSwapList = new List<colorSwap>();
        ProcessCustomisation();
    }

    private void ProcessCustomisation()
    {
        ProcessGender();

        ProcessShirt();

        ProcessArms();

        ProcessHair();

        ProcessSkin();

        ProcessTrousers();

        ProcessHats();

        ProcessAdornments();
        //合并出定制化Sprite
        MergeCustomisation();
    }

    private void ProcessGender()
    {
        if(inputSex == 0)
        {
            farmerBaseTexture = maleFarmerBaseTexture;
        }
        else if(inputSex == 1)
        {
            farmerBaseTexture = femaleFarmerBaseTexture;
        }
        //从纹理种获取像素颜色数组
        Color[] farmerBasePixels = farmerBaseTexture.GetPixels();
        farmerBaseCustomised.SetPixels(farmerBasePixels);
        //实际应用之前的setpixel变更，不然不起作用
        farmerBaseCustomised.Apply();
    }
    private void ProcessShirt()
    {
        bodyFacingArray = new Facing[bodyColumns, bodyRows];
        PopulateBodyFacingArray();

        bodyShirtOffsetArray = new Vector2Int[bodyColumns, bodyRows];
        PopulateBodyShirtOffsetArray();

        AddShirtToTexture(inputShirtStyleNo);
        ApplyShirtTextureToBase();
    }

    private void ProcessArms()
    {
        //获得手臂像素
        Color[] farmerPixelsToRecolour = farmerBaseTexture.GetPixels(0, 0, 288, farmerBaseTexture.height);
        //获得衣服上对应要改变的颜色
        PopulateArmColorSwapList();
        //改变手臂颜色
        ChangePixelColors(farmerPixelsToRecolour, colorSwapList);

        farmerBaseCustomised.SetPixels(0, 0, 288, farmerBaseTexture.height, farmerPixelsToRecolour);
        farmerBaseCustomised.Apply();
    }

    private void PopulateArmColorSwapList()
    {
        colorSwapList.Clear();
        //前者是三种不同的红色，后者是shirt上对应这三个点时不同的颜色
        colorSwapList.Add(new colorSwap(armTargetColor1, selectedShirt.GetPixel(0, 7)));
        colorSwapList.Add(new colorSwap(armTargetColor2, selectedShirt.GetPixel(0, 6)));
        colorSwapList.Add(new colorSwap(armTargetColor3, selectedShirt.GetPixel(0, 5)));
    }

    private void ChangePixelColors(Color[] baseArray, List<colorSwap> colorSwapList)
    {
        //遍历纹理Sprite中已选的区域的颜色块，遇到相同颜色就改成衣服对应的颜色
        for (int i = 0; i < baseArray.Length; i++)
        {
            if (colorSwapList.Count > 0)
            {
                for (int j = 0; j < colorSwapList.Count; j++)
                {
                    if (isSameColor(baseArray[i], colorSwapList[j].fromColor))
                    {
                        baseArray[i] = colorSwapList[j].toColor;
                    }
                }
            }
        }
    }

    private bool isSameColor(Color color1, Color color2)
    {
        //必须int后比较，不然float下比较浮点数精确度会导致三种颜色里两种颜色染不上的bug
        if ((int)(color1.r * 255) == (int)(color2.r * 255) &&
            (int)(color1.g * 255) == (int)(color2.g * 255) &&
            (int)(color1.b * 255) == (int)(color2.b * 255) &&
            color1.a == color2.a)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ProcessTrousers()
    {
        Color[] farmerTrouserPixels = farmerBaseTexture.GetPixels(288, 0, 96, farmerBaseTexture.height);
        //给裤子染色，前者是基础底色(原图是白色)，后者是着色剂的颜色
        TintPixelColors(farmerTrouserPixels, inputTrouserColor);

        farmerBaseCustomised.SetPixels(288, 0, 96, farmerBaseTexture.height, farmerTrouserPixels);
        farmerBaseCustomised.Apply();
    }

    private void TintPixelColors(Color[] baseArray, Color tintColor)
    {
        for (int i = 0; i < baseArray.Length; i++)
        {
            //基于颜色叠加的染色方法，有用，但适用性小，亮度低于原来亮度
            //这个算法是正片叠底
            baseArray[i].r = baseArray[i].r * tintColor.r;
            baseArray[i].g = baseArray[i].g * tintColor.g;
            baseArray[i].b = baseArray[i].b * tintColor.b;
        }
    }

    private void ProcessHair()
    {
        AddHairtToTexture(inputHairStyleNo);
        
        Color[] farmerSelectedHairPixels = hairCustomised.GetPixels();
        TintPixelColors(farmerSelectedHairPixels, inputHairColor);

        hairCustomised.SetPixels(farmerSelectedHairPixels);
        hairCustomised.Apply();
    }

    private void ProcessSkin()
    {
        Color[] farmerSkinPixels = farmerBaseCustomised.GetPixels(0, 0, 288, farmerBaseTexture.height);

        PopulateSkinColorSwapList(inputSkinStyleNo);
        ChangePixelColors(farmerSkinPixels, colorSwapList);

        farmerBaseCustomised.SetPixels(0, 0, 288, farmerBaseTexture.height, farmerSkinPixels);
        farmerBaseCustomised.Apply();
    }

    private void PopulateSkinColorSwapList(int inputSkinType)
    {
        colorSwapList.Clear();

        switch (inputSkinType)
        {
            case 0:
                colorSwapList.Add(new colorSwap(skinTargetColor1, skinTargetColor1));
                colorSwapList.Add(new colorSwap(skinTargetColor2, skinTargetColor2));
                colorSwapList.Add(new colorSwap(skinTargetColor3, skinTargetColor3));
                colorSwapList.Add(new colorSwap(skinTargetColor4, skinTargetColor4));
                break;

            case 1:
                colorSwapList.Add(new colorSwap(skinTargetColor1, new Color32(187, 157, 128, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor2, new Color32(231, 187, 144, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor3, new Color32(221, 186, 154, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor4, new Color32(213, 189, 167, 255)));
                break;

            case 2:
                colorSwapList.Add(new colorSwap(skinTargetColor1, new Color32(105, 69, 2, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor2, new Color32(128, 87, 12, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor3, new Color32(145, 103, 26, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor4, new Color32(161, 114, 25, 255)));
                break;

            case 3:
                colorSwapList.Add(new colorSwap(skinTargetColor1, new Color32(151, 132, 0, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor2, new Color32(187, 166, 15, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor3, new Color32(209, 188, 39, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor4, new Color32(211, 199, 112, 255)));
                break;

            default:
                colorSwapList.Add(new colorSwap(skinTargetColor1, skinTargetColor1));
                colorSwapList.Add(new colorSwap(skinTargetColor2, skinTargetColor2));
                colorSwapList.Add(new colorSwap(skinTargetColor3, skinTargetColor3));
                colorSwapList.Add(new colorSwap(skinTargetColor4, skinTargetColor4));
                break;
        }
    }

    private void ProcessHats()
    {
        AddHatToTexture(inputHatsStyleNo);
    }

    private void ProcessAdornments()
    {
        bodyAdornmentsOffsetArray = new Vector2Int[bodyColumns, bodyRows];

        PopulateBodyAdornmentsOffsetArray();
        AddAdornmentsToTexture(inputAdornmentsStyleNo);

        farmerBaseAdornmentsUpdated = new Texture2D(farmerBaseTexture.width, farmerBaseTexture.height);
        farmerBaseAdornmentsUpdated.filterMode = FilterMode.Point;

        SetTextureToTransparent(farmerBaseAdornmentsUpdated);
        ApplyAdornmentsTextureToBase();
    }

    private void AddAdornmentsToTexture(int AdornmentsStyleNo)
    {
        selectedAdornment = new Texture2D(adornmentsTextureWidth, adornmentsTextureHeight);
        selectedAdornment.filterMode = FilterMode.Point;

        int x = (AdornmentsStyleNo % adornmentsStylesNumberInSpriteWidth) * adornmentsTextureWidth;
        int y = (AdornmentsStyleNo / adornmentsStylesNumberInSpriteWidth) * adornmentsTextureHeight;
        Color[] adornmentsPixels = adornmentsBaseTexture.GetPixels(x, y, adornmentsTextureWidth, adornmentsTextureHeight);

        selectedAdornment.SetPixels(adornmentsPixels);
        selectedAdornment.Apply();
    }

    private void AddHatToTexture(int hatsStyleNo)
    {
        int x = (hatsStyleNo % hatsStylesNumberInSpriteWidth) * hatsTextureWidth;
        int y = (hatsStyleNo / hatsStylesNumberInSpriteWidth) * hatsTextureHeight;

        Color[] hatPixels = hatsBaseTexture.GetPixels(x, y, hatsTextureWidth, hatsTextureHeight);

        hatsCustomised.SetPixels(hatPixels);
        hatsCustomised.Apply();
    }

    private void MergeCustomisation()
    {
        Color[] farmerShirtPixels = farmerBaseShirtsUpdated.GetPixels(0, 0, bodyColumns * farmerSpriteWidth, farmerBaseTexture.height);
        Color[] farmerTrouserPixelsSelection = farmerBaseCustomised.GetPixels(288, 0, 96, farmerBaseTexture.height);
        Color[] farmerBodyPixels = farmerBaseCustomised.GetPixels(0, 0, bodyColumns * farmerSpriteWidth, farmerBaseTexture.height);
        Color[] farmerAdornmentsPixels = farmerBaseAdornmentsUpdated.GetPixels(0, 0, bodyColumns * farmerSpriteWidth, farmerBaseTexture.height);
        
        MergeColourArray(farmerBodyPixels, farmerTrouserPixelsSelection);
        MergeColourArray(farmerBodyPixels, farmerShirtPixels);
        MergeColourArray(farmerBodyPixels, farmerAdornmentsPixels);

        farmerBaseCustomised.SetPixels(0, 0, bodyColumns * farmerSpriteWidth, farmerBaseTexture.height, farmerBodyPixels);
        farmerBaseCustomised.Apply();
    }

    private void MergeColourArray(Color[] baseArray, Color[] mergeArray)
    {
        for(int i = 0; i < baseArray.Length; i++)
        {
            if (mergeArray[i].a > 0)
            {
                if (mergeArray[i].a >= 1)
                {
                    //完全代替原有颜色
                    baseArray[i] = mergeArray[i];
                }
                else
                {
                    float alpha = mergeArray[i].a;
                    //有透明度就做插值颜色
                    baseArray[i].r += (mergeArray[i].r - baseArray[i].r) * alpha;
                    baseArray[i].g += (mergeArray[i].g - baseArray[i].g) * alpha;
                    baseArray[i].b += (mergeArray[i].b - baseArray[i].b) * alpha;
                    baseArray[i].a += mergeArray[i].a;
                }
            }
        }
    }

    private void AddShirtToTexture(int inputShirtStyleNo)
    {
        selectedShirt = new Texture2D(shirtTextureWidth, shirtTextureHeight);
        selectedShirt.filterMode = FilterMode.Point;
        //计算shirt的坐标
        int x = (inputShirtStyleNo % shirtStylesNumberInSpriteWidth) * shirtTextureWidth;
        int y = (inputShirtStyleNo / shirtStylesNumberInSpriteWidth) * shirtTextureHeight;
        //Debug.Log(x + " " + y);
        Color[] shirtPixels = shirtsBaseTexture.GetPixels(x, y, shirtTextureWidth, shirtTextureHeight);

        selectedShirt.SetPixels(shirtPixels);
        selectedShirt.Apply();
    }

    private void AddHairtToTexture(int inputHairStyleNo)
    {
        int x = (inputHairStyleNo % hairStylesNumberInSpriteWidth) * hairTextureWidth;
        int y = (inputHairStyleNo / hairStylesNumberInSpriteWidth) * hairTextureHeight;

        Color[] hairPixels = hairBaseTexture.GetPixels(x,y, hairTextureWidth, hairTextureHeight);

        hairCustomised.SetPixels(hairPixels);
        hairCustomised.Apply();
    }

    private void ApplyShirtTextureToBase()
    {
        farmerBaseShirtsUpdated = new Texture2D(farmerBaseTexture.width, farmerBaseTexture.height);
        farmerBaseShirtsUpdated.filterMode = FilterMode.Point;
        //把要用的纹理设为透明的
        SetTextureToTransparent(farmerBaseShirtsUpdated);

        Color[] frontShirtPixels;
        Color[] backShirtPixels;
        Color[] rightShirtPixels;
        
        frontShirtPixels = selectedShirt.GetPixels(0, shirtSpriteHeight * 3, shirtSpriteWidth, shirtSpriteHeight);
        backShirtPixels = selectedShirt.GetPixels(0, shirtSpriteHeight * 0, shirtSpriteWidth, shirtSpriteHeight);
        rightShirtPixels = selectedShirt.GetPixels(0, shirtSpriteHeight * 2, shirtSpriteWidth, shirtSpriteHeight);
        
        for (int x = 0; x < bodyColumns; x++)
        {
            for(int y = 0; y < bodyRows; y++)
            {
                int pixelX = farmerSpriteWidth * x;
                int pixelY = farmerSpriteHeight * y;

                if (bodyShirtOffsetArray[x, y] != null)
                {
                    if (bodyShirtOffsetArray[x, y].x == 99 && bodyShirtOffsetArray[x, y].y == 99) continue;
                    pixelX += bodyShirtOffsetArray[x, y].x;
                    pixelY += bodyShirtOffsetArray[x, y].y;
                }
                //根据身体方向填充纹理
                switch (bodyFacingArray[x, y])
                {
                    case Facing.none:
                        break;
                    case Facing.front:
                        farmerBaseShirtsUpdated.SetPixels(pixelX, pixelY, shirtSpriteWidth, shirtSpriteHeight, frontShirtPixels);
                        break;
                    case Facing.back:
                        farmerBaseShirtsUpdated.SetPixels(pixelX, pixelY, shirtSpriteWidth, shirtSpriteHeight, backShirtPixels);
                        break;
                    case Facing.right:
                        farmerBaseShirtsUpdated.SetPixels(pixelX, pixelY, shirtSpriteWidth, shirtSpriteHeight, rightShirtPixels);
                        break;
                    default:
                        break;
                }
            }
        }
        farmerBaseShirtsUpdated.Apply();
    }

    private void ApplyAdornmentsTextureToBase()
    {
        Color[] frontAdornmentsPixels;
        Color[] rightAdornmentsPixels;

        frontAdornmentsPixels = selectedAdornment.GetPixels(0, adornmentsSpriteHeight * 1, adornmentsSpriteWidth, adornmentsSpriteHeight);
        rightAdornmentsPixels = selectedAdornment.GetPixels(0, adornmentsSpriteHeight * 0, adornmentsSpriteWidth, adornmentsSpriteHeight);

        for(int x = 0; x < bodyColumns; x++)
        {
            for(int y = 0; y < bodyRows; y++)
            {
                int pixelX = farmerSpriteWidth * x;
                int pixelY = farmerSpriteHeight * y;

                if (bodyAdornmentsOffsetArray[x, y] != null)
                {
                    pixelX += bodyAdornmentsOffsetArray[x, y].x;
                    pixelY += bodyAdornmentsOffsetArray[x, y].y;
                }

                switch (bodyFacingArray[x, y])
                {
                    case Facing.none:
                        break;
                    case Facing.front:
                        farmerBaseAdornmentsUpdated.SetPixels(pixelX, pixelY, adornmentsSpriteWidth, adornmentsSpriteHeight, frontAdornmentsPixels);
                        break;

                    case Facing.right:
                        farmerBaseAdornmentsUpdated.SetPixels(pixelX, pixelY, adornmentsSpriteWidth, adornmentsSpriteHeight, rightAdornmentsPixels);
                        break;
                    default:
                        break;
                }
            }
        }
        farmerBaseAdornmentsUpdated.Apply();
    }
    /// <summary>
    /// 把纹理设为透明的
    /// </summary>
    /// <param name="texture2D"></param>
    private void SetTextureToTransparent(Texture2D texture2D)
    {
        Color[] fill = new Color[texture2D.height * texture2D.width];
        for(int i = 0; i < fill.Length; i++)
        {
            fill[i] = Color.clear;
        }
        texture2D.SetPixels(fill);
    }

    private void PopulateBodyFacingArray()
    {
        //Sprite中左下角开始，前10行&前6列都是none的
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                bodyFacingArray[i, j] = Facing.none;
            }
        }
        bodyFacingArray[0, 10] = Facing.back;
        bodyFacingArray[1, 10] = Facing.back;
        bodyFacingArray[2, 10] = Facing.right;
        bodyFacingArray[3, 10] = Facing.right;
        bodyFacingArray[4, 10] = Facing.right;
        bodyFacingArray[5, 10] = Facing.right;

        bodyFacingArray[0, 11] = Facing.front;
        bodyFacingArray[1, 11] = Facing.front;
        bodyFacingArray[2, 11] = Facing.front;
        bodyFacingArray[3, 11] = Facing.front;
        bodyFacingArray[4, 11] = Facing.back;
        bodyFacingArray[5, 11] = Facing.back;

        bodyFacingArray[0, 12] = Facing.back;
        bodyFacingArray[1, 12] = Facing.back;
        bodyFacingArray[2, 12] = Facing.right;
        bodyFacingArray[3, 12] = Facing.right;
        bodyFacingArray[4, 12] = Facing.right;
        bodyFacingArray[5, 12] = Facing.right;

        bodyFacingArray[0, 13] = Facing.front;
        bodyFacingArray[1, 13] = Facing.front;
        bodyFacingArray[2, 13] = Facing.front;
        bodyFacingArray[3, 13] = Facing.front;
        bodyFacingArray[4, 13] = Facing.back;
        bodyFacingArray[5, 13] = Facing.back;

        bodyFacingArray[0, 14] = Facing.back;
        bodyFacingArray[1, 14] = Facing.back;
        bodyFacingArray[2, 14] = Facing.right;
        bodyFacingArray[3, 14] = Facing.right;
        bodyFacingArray[4, 14] = Facing.right;
        bodyFacingArray[5, 14] = Facing.right;

        bodyFacingArray[0, 15] = Facing.front;
        bodyFacingArray[1, 15] = Facing.front;
        bodyFacingArray[2, 15] = Facing.front;
        bodyFacingArray[3, 15] = Facing.front;
        bodyFacingArray[4, 15] = Facing.back;
        bodyFacingArray[5, 15] = Facing.back;

        bodyFacingArray[0, 16] = Facing.back;
        bodyFacingArray[1, 16] = Facing.back;
        bodyFacingArray[2, 16] = Facing.right;
        bodyFacingArray[3, 16] = Facing.right;
        bodyFacingArray[4, 16] = Facing.right;
        bodyFacingArray[5, 16] = Facing.right;

        bodyFacingArray[0, 17] = Facing.front;
        bodyFacingArray[1, 17] = Facing.front;
        bodyFacingArray[2, 17] = Facing.front;
        bodyFacingArray[3, 17] = Facing.front;
        bodyFacingArray[4, 17] = Facing.back;
        bodyFacingArray[5, 17] = Facing.back;

        bodyFacingArray[0, 18] = Facing.back;
        bodyFacingArray[1, 18] = Facing.back;
        bodyFacingArray[2, 18] = Facing.back;
        bodyFacingArray[3, 18] = Facing.right;
        bodyFacingArray[4, 18] = Facing.right;
        bodyFacingArray[5, 18] = Facing.right;

        bodyFacingArray[0, 19] = Facing.right;
        bodyFacingArray[1, 19] = Facing.right;
        bodyFacingArray[2, 19] = Facing.right;
        bodyFacingArray[3, 19] = Facing.front;
        bodyFacingArray[4, 19] = Facing.front;
        bodyFacingArray[5, 19] = Facing.front;

        bodyFacingArray[0, 20] = Facing.front;
        bodyFacingArray[1, 20] = Facing.front;
        bodyFacingArray[2, 20] = Facing.front;
        bodyFacingArray[3, 20] = Facing.back;
        bodyFacingArray[4, 20] = Facing.back;
        bodyFacingArray[5, 20] = Facing.back;
    }

    private void PopulateBodyShirtOffsetArray()
    {
        //Sprite中左下角开始，前10行&前6列都是(99,99)的
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                bodyShirtOffsetArray[i, j] = new Vector2Int(99, 99);
            }
        }
        bodyShirtOffsetArray[0, 10] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[1, 10] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[2, 10] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[3, 10] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[4, 10] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[5, 10] = new Vector2Int(4, 10);

        bodyShirtOffsetArray[0, 11] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[1, 11] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[2, 11] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[3, 11] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 11] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[5, 11] = new Vector2Int(4, 12);

        bodyShirtOffsetArray[0, 12] = new Vector2Int(3, 9);
        bodyShirtOffsetArray[1, 12] = new Vector2Int(3, 9);
        bodyShirtOffsetArray[2, 12] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[3, 12] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[4, 12] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 12] = new Vector2Int(4, 9);

        bodyShirtOffsetArray[0, 13] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[1, 13] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 13] = new Vector2Int(5, 9);
        bodyShirtOffsetArray[3, 13] = new Vector2Int(5, 9);
        bodyShirtOffsetArray[4, 13] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[5, 13] = new Vector2Int(4, 9);

        bodyShirtOffsetArray[0, 14] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[1, 14] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[2, 14] = new Vector2Int(4, 7);
        bodyShirtOffsetArray[3, 14] = new Vector2Int(4, 5);
        bodyShirtOffsetArray[4, 14] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 14] = new Vector2Int(4, 12);

        bodyShirtOffsetArray[0, 15] = new Vector2Int(4, 8);
        bodyShirtOffsetArray[1, 15] = new Vector2Int(4, 5);
        bodyShirtOffsetArray[2, 15] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 15] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[4, 15] = new Vector2Int(4, 8);
        bodyShirtOffsetArray[5, 15] = new Vector2Int(4, 5);

        bodyShirtOffsetArray[0, 16] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[1, 16] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[2, 16] = new Vector2Int(4, 7);
        bodyShirtOffsetArray[3, 16] = new Vector2Int(4, 8);
        bodyShirtOffsetArray[4, 16] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 16] = new Vector2Int(4, 10);

        bodyShirtOffsetArray[0, 17] = new Vector2Int(4, 7);
        bodyShirtOffsetArray[1, 17] = new Vector2Int(4, 8);
        bodyShirtOffsetArray[2, 17] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 17] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 17] = new Vector2Int(4, 7);
        bodyShirtOffsetArray[5, 17] = new Vector2Int(4, 8);

        bodyShirtOffsetArray[0, 18] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[1, 18] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 18] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 18] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 18] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 18] = new Vector2Int(4, 9);

        bodyShirtOffsetArray[0, 19] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[1, 19] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 19] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 19] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 19] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 19] = new Vector2Int(4, 9);

        bodyShirtOffsetArray[0, 20] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[1, 20] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 20] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 20] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 20] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 20] = new Vector2Int(4, 9);
    }

    private void PopulateBodyAdornmentsOffsetArray()
    {
        //Sprite中左下角开始，前10行&前6列都是(99,99)的
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                bodyAdornmentsOffsetArray[i, j] = new Vector2Int(99, 99);
            }
        }
        bodyAdornmentsOffsetArray[0, 10] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 10] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 10] = new Vector2Int(0, 1 + 16);
        bodyAdornmentsOffsetArray[3, 10] = new Vector2Int(0, 2 + 16);
        bodyAdornmentsOffsetArray[4, 10] = new Vector2Int(0, 1 + 16);
        bodyAdornmentsOffsetArray[5, 10] = new Vector2Int(0, 0 + 16);

        bodyAdornmentsOffsetArray[0, 11] = new Vector2Int(0, 1 + 16);
        bodyAdornmentsOffsetArray[1, 11] = new Vector2Int(0, 2 + 16);
        bodyAdornmentsOffsetArray[2, 11] = new Vector2Int(0, 1 + 16);
        bodyAdornmentsOffsetArray[3, 11] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[4, 11] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 11] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 12] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 12] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 12] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[3, 12] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[4, 12] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[5, 12] = new Vector2Int(0, -1 + 16);

        bodyAdornmentsOffsetArray[0, 13] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[1, 13] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[2, 13] = new Vector2Int(1, -1 + 16);
        bodyAdornmentsOffsetArray[3, 13] = new Vector2Int(1, -1 + 16);
        bodyAdornmentsOffsetArray[4, 13] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 13] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 14] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 14] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 14] = new Vector2Int(0, -3 + 16);
        bodyAdornmentsOffsetArray[3, 14] = new Vector2Int(0, -5 + 16);
        bodyAdornmentsOffsetArray[4, 14] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[5, 14] = new Vector2Int(0, 1 + 16);

        bodyAdornmentsOffsetArray[0, 15] = new Vector2Int(0, -2 + 16);
        bodyAdornmentsOffsetArray[1, 15] = new Vector2Int(0, -5 + 16);
        bodyAdornmentsOffsetArray[2, 15] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[3, 15] = new Vector2Int(0, 2 + 16);
        bodyAdornmentsOffsetArray[4, 15] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 15] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 16] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 16] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 16] = new Vector2Int(0, -3 + 16);
        bodyAdornmentsOffsetArray[3, 16] = new Vector2Int(0, -2 + 16);
        bodyAdornmentsOffsetArray[4, 16] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[5, 16] = new Vector2Int(0, 0 + 16);

        bodyAdornmentsOffsetArray[0, 17] = new Vector2Int(0, -3 + 16);
        bodyAdornmentsOffsetArray[1, 17] = new Vector2Int(0, -2 + 16);
        bodyAdornmentsOffsetArray[2, 17] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[3, 17] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[4, 17] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 17] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 18] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 18] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 18] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 18] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[4, 18] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[5, 18] = new Vector2Int(0, -1 + 16);

        bodyAdornmentsOffsetArray[0, 19] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[1, 19] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[2, 19] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[3, 19] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[4, 19] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[5, 19] = new Vector2Int(0, -1 + 16);

        bodyAdornmentsOffsetArray[0, 20] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[1, 20] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[2, 20] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[3, 20] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 20] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 20] = new Vector2Int(99, 99);
    }

    public  void ChangeHair(int hairStyle)
    {
        inputHairStyleNo = hairStyle;
        ProcessHair();
    }

    public void ChangeHair(Color hairColor)
    {
        inputHairColor = hairColor;
        ProcessHair();
    }

    public void ChangeSkinStyle(int skinStyle)
    {
        inputSkinStyleNo = skinStyle;
        ProcessCustomisation();
    }

    public void ChangeShirtStyle(int shirtStyle)
    {
        inputShirtStyleNo = shirtStyle;
        ProcessCustomisation();
    }

    public void ChangeTrouser(Color trouserColor)
    {
        inputTrouserColor = trouserColor;
        ProcessCustomisation();
    }

    public void ChangeHat(int hat)
    {
        inputHatsStyleNo = hat;
        ProcessCustomisation();
    }

    public void ChangeAdornments(int adornment)
    {
        inputAdornmentsStyleNo = adornment;
        ProcessCustomisation();
    }
}
