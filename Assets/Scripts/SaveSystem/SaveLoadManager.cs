using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SaveLoadManager:SingletonMonobehaviour<SaveLoadManager>
{
    //使用interface而不是特定对象类型，从而保存不同类型的对象
    public List<ISaveable> iSaveableObjectList;

    protected override void Awake()
    {
        base.Awake();
        iSaveableObjectList = new List<ISaveable>();
    }

    public void StoreCurrentSceneData()
    {
        foreach (var item in iSaveableObjectList)
        {
            item.ISaveableStoreScene(SceneManager.GetActiveScene().name);
        }
    }

    public void RestoreCurrentSceneData()
    {
        foreach(var item in iSaveableObjectList)
        {
            item.ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }

}
