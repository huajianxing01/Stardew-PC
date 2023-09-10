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
    Reapable_scenary,//���ո��
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