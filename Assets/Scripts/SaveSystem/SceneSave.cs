using System.Collections.Generic;

[System.Serializable]
public class SceneSave
{
    //�洢scene��inventory�ϵ�item
    public List<SceneItem> listSceneItem;
    public List<InventoryItem>[] listInventoryItemArray;
    //�洢��������
    public Dictionary<string, int[]> intArrayDictionary;
    //�洢scene��ÿ��grid����ϸ��Ϣ
    public Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary;
    //�洢scene�ϵ�boolֵ
    public Dictionary<string, bool> boolDictionary;
    //�洢���﷽�򡢳�����λ����Ϣ
    public Dictionary<string, string> stringDictionary;
    public Dictionary<string, Vector3Serializable> vector3Dictionary;
    //�洢��Ϸʱ��
    public Dictionary<string, int> intDictionary;
}
