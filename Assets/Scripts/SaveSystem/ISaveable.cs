using UnityEngine;

//interface�ӿڶ���Э���д����������ʵ��״̬��ʹ�ýӿڱ�����ʾʵ�ֽӿڵĳ�Ա
public interface ISaveable
{
    string ISaveableUniqueID { get; set; }
    GameObjectSave GameObjectSave { get; set; }
    void ISaveableRegister();
    void ISaveableDeregister();
    void ISaveableStoreScene(string sceneName);
    void ISaveableRestoreScene(string sceneName);
}
