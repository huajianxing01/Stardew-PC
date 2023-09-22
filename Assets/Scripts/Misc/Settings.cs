using UnityEngine;

public static class Settings
{
    //scene
    public const string persistentScene = "PersistentScene";

    //Obscuring Item Fading
    public const float fadeInSeconds = 0.25f;
    public const float fadeOutSeconds = 0.35f;
    public const float targetAlpha = 0.45f;

    //tilemap
    public const float gridCellSize = 1f;
    public static Vector2 cursorSize = Vector2.one;
    public static float gridCellDiagonalSize = 1.4f;//unity cell�жԽǾ�����1.4
    public static int maxGridWidth = 999999;
    public static int maxGridHeight = 999999;

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

    //play�ֿ�������������
    public static int playerInitialInventoryCapacity = 24;
    public static int playerMaxInventoryCapacity = 48;

    //�洢Player��Animation�Ĳ���ID���ƣ���Ϊint����ΪID������
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

    //�����Animation�Ĳ���ID��
    public static int idleRight;
    public static int idleLeft;
    public static int idleUp;
    public static int idleDown;

    //NPC movement��NPC�ƶ��ľ���
    public static float pixelSize = 0.0625f;

    //�洢NPC��animation����
    public static int npcWalkUp;
    public static int npcWalkDown;
    public static int npcWalkLeft;
    public static int npcWalkRight;
    public static int npcEventAnimation;

    //Time system
    public const float secondsPerGameSecond = 0.012f;
    public const string spring = "����";
    public const string summer = "����";
    public const string autumn = "����";
    public const string winter = "����";
    public const string monday = "��һ";
    public const string tuesday = "�ܶ�";
    public const string wednesday = "����";
    public const string thursday = "����";
    public const string friday = "����";
    public const string saturday = "����";
    public const string sunday = "����";

    //���߳ƺ�
    public const string HoeingTool = "���ع���";
    public const string ChoppingTool = "��������";
    public const string BreakingTool = "���鹤��";
    public const string ReapingTool = "�ո��";
    public const string WateringTool = "��ˮ����";
    public const string CollectingTool = "�ռ�����";

    //������Ʒ�ƺ�
    public const string Seed = "����";
    public const string Commodity = "��Ʒ";
    public const string Funityre = "�Ҿ�";
    public const string OtherItem = "����";

    //�ո�����
    public const int maxCollidersToTestPerReapSwing = 15;
    public const int maxTargetComponentsToDestroyPerReapSwing = 2;

    //���캯��
    static Settings()
    {
        //�洢Player��Animation�Ĳ���ID���ƣ���Ϊint����ΪID������
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

        //npc
        npcWalkUp = Animator.StringToHash("npcWalkUp");
        npcWalkDown = Animator.StringToHash("npcWalkDown");
        npcWalkLeft = Animator.StringToHash("npcWalkLeft");
        npcWalkRight = Animator.StringToHash("npcWalkRight");
        npcEventAnimation = Animator.StringToHash("npcEventAnimation");

        //�����Animation�Ĳ���ID��
        idleRight = Animator.StringToHash("idleRight");
        idleLeft = Animator.StringToHash("idleLeft");
        idleUp = Animator.StringToHash("idleUp");
        idleDown = Animator.StringToHash("idleDown");
    }
}
