using System.Collections.Generic;

[System.Serializable]
public class SceneSave
{
    //�洢scene�ϵ�item
    public List<SceneItem> listSceneItem;
    //�洢scene��ÿ��grid����ϸ��Ϣ
    public Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary;
    //�洢scene�ϵ�boolֵ
    public Dictionary<string, bool> boolDictionary;
}
