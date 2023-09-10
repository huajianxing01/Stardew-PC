using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SaveLoadManager:SingletonMonobehaviour<SaveLoadManager>
{
    //ʹ��interface�������ض��������ͣ��Ӷ����治ͬ���͵Ķ���
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
