using System.Collections.Generic;

[System.Serializable]
public class SceneSave
{
    //List�洢ÿ��scene��item
    public List<SceneItem> listSceneItem;
    //Dictionary�洢scene��ÿ��grid����ϸ��Ϣ
    public Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary;
}
