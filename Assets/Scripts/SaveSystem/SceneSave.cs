using System.Collections.Generic;

[System.Serializable]
public class SceneSave
{
    //存储scene上的item
    public List<SceneItem> listSceneItem;
    //存储scene上每个grid的详细信息
    public Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary;
    //存储scene上的bool值
    public Dictionary<string, bool> boolDictionary;
}
