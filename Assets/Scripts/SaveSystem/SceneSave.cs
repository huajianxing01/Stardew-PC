using System.Collections.Generic;

[System.Serializable]
public class SceneSave
{
    //List存储每个scene的item
    public List<SceneItem> listSceneItem;
    //Dictionary存储scene上每个grid的详细信息
    public Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary;
}
