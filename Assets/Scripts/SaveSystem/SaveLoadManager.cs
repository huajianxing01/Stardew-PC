using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager:SingletonMonobehaviour<SaveLoadManager>
{
    //ʹ��interface�������ض��������ͣ��Ӷ����治ͬ���͵Ķ���
    public List<ISaveable> iSaveableObjectList;
    public GameSave gameSave;

    protected override void Awake()
    {
        base.Awake();
        iSaveableObjectList = new List<ISaveable>();
    }
    /// <summary>
    /// �洢��ǰScene����
    /// </summary>
    public void StoreCurrentSceneData()
    {
        foreach (var item in iSaveableObjectList)
        {
            item.ISaveableStoreScene(SceneManager.GetActiveScene().name);
        }
    }
    /// <summary>
    /// ���ص�ǰScene����
    /// </summary>
    public void RestoreCurrentSceneData()
    {
        foreach(var item in iSaveableObjectList)
        {
            item.ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }
    /// <summary>
    /// ʹ�ö����Ʒ����л���File�м�����Ϸ�������������
    /// </summary>
    public void LoadDataFromFile()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/WildHopeCreek.dat"))
        {
            gameSave = new GameSave();
            //�򿪶�ӦFile
            FileStream file = File.Open(Application.persistentDataPath + "/WildHopeCreek.dat", FileMode.Open);
            //ʹ�ö����Ʒ����л������Ϸ�������������
            gameSave = (GameSave)binaryFormatter.Deserialize(file);

            for(int i = iSaveableObjectList.Count - 1; i > -1; i--)
            {
                if (gameSave.GameData.ContainsKey(iSaveableObjectList[i].ISaveableUniqueID))
                {
                    iSaveableObjectList[i].ISaveableLoad(gameSave);
                }
                else
                {
                    //���UID��Ӧ��GameObjectSave�������ֵ��У������ٶ�Ӧ����Ϸ����
                    Component component = (Component)iSaveableObjectList[i];
                    Destroy(component.gameObject);
                }
            }
            //�ر�file
            file.Close();
        }
        //�ر�Pause�˵����ص�Scene��
        UIManager.Instance.DisablePauseMenu();
    }
    /// <summary>
    /// ʹ�ö��������л�������Ϸ���ݵ�File��
    /// </summary>
    public void SaveDataToFile()
    {
        gameSave = new GameSave();

        foreach (var item in iSaveableObjectList)
        {
            gameSave.GameData.Add(item.ISaveableUniqueID, item.ISaveableSave());
        }

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        //������ӦFile���������File�͸���
        FileStream file = File.Open(Application.persistentDataPath + "/WildHopeCreek.dat", FileMode.Create);
        //ʹ�ö��������л�������Ϸ���ݵ�File��
        binaryFormatter.Serialize(file, gameSave);
        file.Close();

        UIManager.Instance.DisablePauseMenu();
    }
}
