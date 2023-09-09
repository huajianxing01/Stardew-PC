using UnityEngine;

//interface接口定义协议的写法，不包含实例状态，使用接口必须显示实现接口的成员
public interface ISaveable
{
    string ISaveableUniqueID { get; set; }
    GameObjectSave GameObjectSave { get; set; }
    void ISaveableRegister();
    void ISaveableDeregister();
    void ISaveableStoreScene(string sceneName);
    void ISaveableRestoreScene(string sceneName);
}
