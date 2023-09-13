using UnityEngine;

public static class Settings
{
    //Obscuring Item Fading
    public const float fadeInSeconds = 0.25f;
    public const float fadeOutSeconds = 0.35f;
    public const float targetAlpha = 0.45f;

    //tilemap
    public const float gridCellSize = 1f;
    public static Vector2 cursorSize = Vector2.one;

    //player
    public static float playerCenterOffset = 0.875f;

    //player movement
    public const float runningSpeed = 8.0f;
    public const float walkingSpeed = 5.333f;
    public static float useToolAnimationPause = 0.25f;
    public static float afterUseToolAnimationPause = 0.2f;
    public static float liftToolAnimationPause = 0.4f;
    public static float afterLiftToolAnimationPause = 0.4f;
    public static float pickAnimationPause = 1f;
    public static float afterPickAnimationPause = 0.2f;

    //Time system
    public const float secondsPerGameSecond = 0.012f;
    public const string spring = "春天";
    public const string summer = "夏天";
    public const string autumn = "秋天";
    public const string winter = "冬天";
    public const string monday = "周一";
    public const string tuesday = "周二";
    public const string wednesday = "周三";
    public const string thursday = "周四";
    public const string friday = "周五";
    public const string saturday = "周六";
    public const string sunday = "周日";


    //玩家仓库容量（背包）
    public static int playerInitialInventoryCapacity = 24;
    public static int playerMaxInventoryCapacity = 48;

    //存储Player的Animation的参数ID名称，设为int是因为ID是整数
    public static int xInput;
    public static int yInput;
    public static int isWalking;
    public static int isRunning;
    public static int toolEffect;

    public static int isUsingToolRight;
    public static int isUsingToolLeft;
    public static int isUsingToolUp;
    public static int isUsingToolDown;

    public static int isLiftingToolRight;
    public static int isLiftingToolLeft;
    public static int isLiftingToolUp;
    public static int isLiftingToolDown;

    public static int isPickingRight;
    public static int isPickingLeft;
    public static int isPickingUp;
    public static int isPickingDown;

    public static int isSwingingToolRight;
    public static int isSwingingToolLeft;
    public static int isSwingingToolUp;
    public static int isSwingingToolDown;

    //共享的Animation的参数ID名
    public static int idleRight;
    public static int idleLeft;
    public static int idleUp;
    public static int idleDown;

    //工具称呼
    public const string HoeingTool = "锄地工具";
    public const string ChoppingTool = "劈砍工具";
    public const string BreakingTool = "破碎工具";
    public const string ReapingTool = "收割工具";
    public const string WateringTool = "浇水工具";
    public const string CollectingTool = "收集工具";

    //其他物品称呼
    public const string Seed = "种子";
    public const string Commodity = "商品";
    public const string Funityre = "家具";
    public const string OtherItem = "杂物";

    //收割限制
    public const int maxCollidersToTestPerReapSwing = 15;
    public const int maxTargetComponentsToDestroyPerReapSwing = 2;

    //构造函数
    static Settings()
    {
        //存储Player的Animation的参数ID名称，设为int是因为ID是整数
        xInput = Animator.StringToHash("xInput");
        yInput = Animator.StringToHash("yInput");
        isWalking = Animator.StringToHash("isWalking");
        isRunning = Animator.StringToHash("isRunning");
        toolEffect = Animator.StringToHash("toolEffect");

        isUsingToolRight = Animator.StringToHash("isUsingToolRight");
        isUsingToolLeft = Animator.StringToHash("isUsingToolLeft");
        isUsingToolUp = Animator.StringToHash("isUsingToolUp");
        isUsingToolDown = Animator.StringToHash("isUsingToolDown");

        isLiftingToolRight = Animator.StringToHash("isLiftingToolRight");
        isLiftingToolLeft = Animator.StringToHash("isLiftingToolLeft");
        isLiftingToolUp = Animator.StringToHash("isLiftingToolUp");
        isLiftingToolDown = Animator.StringToHash("isLiftingToolDown");

        isPickingRight = Animator.StringToHash("isPickingRight");
        isPickingLeft = Animator.StringToHash("isPickingLeft");
        isPickingUp = Animator.StringToHash("isPickingUp");
        isPickingDown = Animator.StringToHash("isPickingDown");

        isSwingingToolRight = Animator.StringToHash("isSwingingToolRight");
        isSwingingToolLeft = Animator.StringToHash("isSwingingToolLeft");
        isSwingingToolUp = Animator.StringToHash("isSwingingToolUp");
        isSwingingToolDown = Animator.StringToHash("isSwingingToolDown");
        
        //共享的Animation的参数ID名
        idleRight = Animator.StringToHash("idleRight");
        idleLeft = Animator.StringToHash("idleLeft");
        idleUp = Animator.StringToHash("idleUp");
        idleDown = Animator.StringToHash("idleDown");
    }
}
