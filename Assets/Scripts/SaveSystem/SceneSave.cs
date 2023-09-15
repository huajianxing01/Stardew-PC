using System.Collections.Generic;

[System.Serializable]
public class SceneSave
{
    //存储scene和inventory上的item
    public List<SceneItem> listSceneItem;
    public List<InventoryItem>[] listInventoryItemArray;
    //存储库存的容量
    public Dictionary<string, int[]> intArrayDictionary;
    //存储scene上每个grid的详细信息
    public Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary;
    //存储scene上的bool值
    public Dictionary<string, bool> boolDictionary;
    //存储人物方向、场景、位置信息
    public Dictionary<string, string> stringDictionary;
    public Dictionary<string, Vector3Serializable> vector3Dictionary;
    //存储游戏时间
    public Dictionary<string, int> intDictionary;
}
