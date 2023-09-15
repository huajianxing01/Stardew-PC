using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager:SingletonMonobehaviour<SaveLoadManager>
{
    //使用interface而不是特定对象类型，从而保存不同类型的对象
    public List<ISaveable> iSaveableObjectList;
    public GameSave gameSave;

    protected override void Awake()
    {
        base.Awake();
        iSaveableObjectList = new List<ISaveable>();
    }
    /// <summary>
    /// 存储当前Scene数据
    /// </summary>
    public void StoreCurrentSceneData()
    {
        foreach (var item in iSaveableObjectList)
        {
            item.ISaveableStoreScene(SceneManager.GetActiveScene().name);
        }
    }
    /// <summary>
    /// 加载当前Scene数据
    /// </summary>
    public void RestoreCurrentSceneData()
    {
        foreach(var item in iSaveableObjectList)
        {
            item.ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }
    /// <summary>
    /// 使用二进制反序列化从File中加载游戏保存的所有数据
    /// </summary>
    public void LoadDataFromFile()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/WildHopeCreek.dat"))
        {
            gameSave = new GameSave();
            //打开对应File
            FileStream file = File.Open(Application.persistentDataPath + "/WildHopeCreek.dat", FileMode.Open);
            //使用二进制反序列化获得游戏保存的所有数据
            gameSave = (GameSave)binaryFormatter.Deserialize(file);

            for(int i = iSaveableObjectList.Count - 1; i > -1; i--)
            {
                if (gameSave.GameData.ContainsKey(iSaveableObjectList[i].ISaveableUniqueID))
                {
                    iSaveableObjectList[i].ISaveableLoad(gameSave);
                }
                else
                {
                    //如果UID对应的GameObjectSave不存在字典中，则销毁对应的游戏对象
                    Component component = (Component)iSaveableObjectList[i];
                    Destroy(component.gameObject);
                }
            }
            //关闭file
            file.Close();
        }
        //关闭Pause菜单，回到Scene中
        UIManager.Instance.DisablePauseMenu();
    }
    /// <summary>
    /// 使用二进制序列化保存游戏数据到File中
    /// </summary>
    public void SaveDataToFile()
    {
        gameSave = new GameSave();

        foreach (var item in iSaveableObjectList)
        {
            gameSave.GameData.Add(item.ISaveableUniqueID, item.ISaveableSave());
        }

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        //创建对应File，如果已有File就覆盖
        FileStream file = File.Open(Application.persistentDataPath + "/WildHopeCreek.dat", FileMode.Create);
        //使用二进制序列化保存游戏数据到File中
        binaryFormatter.Serialize(file, gameSave);
        file.Close();

        UIManager.Instance.DisablePauseMenu();
    }
}
