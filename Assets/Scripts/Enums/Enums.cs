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
    Seed,//种子
    Commodity,//商品
    Watering_tool,//浇水工具
    Hoeing_tool,//锄地工具
    Chopping_tool,//劈砍工具
    Breaking_tool,//破碎工具
    Reaping_tool,//收割工具
    Collecting_tool,//收集工具
    Reapable_scenary,//可收割的
    Funityre,//家具
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
    swingToolDown,//摆动工具
    swingToolUp,
    swingToolLeft,
    swingToolRight,
    liftToolDown,//举起工具
    liftToolUp,
    liftToolLeft,
    liftToolRight,
    holdToolDown,//握住工具
    holdToolUp,
    holdToolRight,
    holdToolLeft,
    pickDown,//挖掘
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