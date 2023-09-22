public enum ToolEffect
{
    none,
    watering
}

public enum Direction
{
    up,
    down,
    left,
    right,
    none
}

public enum ItemType
{
    Seed,//����
    Commodity,//��Ʒ
    Watering_tool,//��ˮ����
    Hoeing_tool,//���ع���
    Chopping_tool,//��������
    Breaking_tool,//���鹤��
    Reaping_tool,//�ո��
    Collecting_tool,//�ռ�����
    Reapable_scenery,//���ո��
    Funityre,//�Ҿ�
    none,
    count
}

public enum InventoryLocation
{
    player,
    chest,
    count
}

public enum AnimationName
{
    idleDown,
    idleUp,
    idleLeft,
    idleRight,
    walkDown,
    walkUp,
    walkLeft,
    walkRight,
    runDown,
    runUp,
    runLeft,
    runRight,
    useToopUp,
    useToopDown,
    useToopLeft,
    useToopRight,
    swingToolDown,//�ڶ�����
    swingToolUp,
    swingToolLeft,
    swingToolRight,
    liftToolDown,//���𹤾�
    liftToolUp,
    liftToolLeft,
    liftToolRight,
    holdToolDown,//��ס����
    holdToolUp,
    holdToolRight,
    holdToolLeft,
    pickDown,//�ھ�
    pickUp,
    pickLeft,
    pickRight,
    count
}

public enum CharacterPartAnimator
{
    body,
    arms,
    hair,
    hat,
    tool,
    count
}

public enum PartVariantColour
{
    none,
    count
}

public enum PartVariantType
{
    none,
    carry,
    hoe,
    pickaxe,
    axe,
    scythe,
    wateringCan,
    count
}

public enum Season
{
    Spring,
    Summer,
    Autumn,
    Winter,
    none,
    count
}

public enum Week
{
    Mon,
    Tues,
    Wed,
    Thur,
    Fri,
    Sat,
    Sun,
    none,
    count
}

public enum SceneName
{
    Scene1_Farm,
    Scene2_Field,
    Scene3_Cabin,
}

public enum GridBoolProperty
{
    diggable,
    canDropItem,
    canPlaceFurniture,
    isPath,
    isNPCObstacle
}

public enum HarvestActionEffect
{
    reaping,//�ո���Ч
    deciduousLeavesFalling,//��Ҷ����
    pineConesFalling,//�ɹ�����
    choppingTreeTrunk,//����׮
    breakingStone,//��ʯͷ
    none
}

public enum Facing
{
    none,
    front,
    back,
    right
}

public enum Weather
{
    dry,
    raining,
    snowing,
    none,
    count
}

public enum SoundName
{
    none = 0,
    effectFoodtstepSoftGround = 10,
    effectFoodtstepHardGround = 20,
    effectAxe = 30,
    effectPickaxe = 40,
    effectScythe = 50,
    effectHoe = 60,
    effectWateringCan = 70,
    effectBasket = 80,
    effectPickupSound = 90,
    effectRustle = 100,
    effectTreeFalling = 110,
    effectPlantingSound = 120,
    effectPluck = 130,//��ժ
    effectStoneShatter = 140,
    effectWoodSplinters = 150,
    ambientCountryside1 = 1000,//���-������
    ambientCountryside2 = 1010,
    ambientIndoors1 = 1020,
    musicCalm3 = 2000,//��ƽ�˾�
    musicCalm1 = 2010
}