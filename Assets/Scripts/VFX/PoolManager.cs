using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PoolManager : SingletonMonobehaviour<PoolManager>
{
    [Serializable]
    public struct Pool
    {
        public int poolSize;
        public GameObject prefab;
    }

    private Dictionary<int,Queue<GameObject>> poolDictionary = new Dictionary<int,Queue<GameObject>>();
    [SerializeField] private Pool[] pool = null;
    [SerializeField] private Transform objectPoolTransform = null;

    private void Start()
    {
        for(int i = 0; i < pool.Length; i++)
        {
            CreatePool(pool[i].prefab, pool[i].poolSize);
        }
    }

    private void CreatePool(GameObject prefab, int poolSize)
    {
        //���Ψһ��ID�Ͷ�Ӧname
        int poolKey = prefab.GetInstanceID();
        string prefabName = prefab.name;
        //����һ���������Ա�ʵ�����Ӷ���
        GameObject parentGameObject = new GameObject(prefabName + "Anchor");
        parentGameObject.transform.SetParent(objectPoolTransform);

        if (!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary.Add(poolKey, new Queue<GameObject>());
            for(int i = 0; i < poolSize; i++)
            {
                //Instantiateʵ�������Ķ�����Object����,��Ҫǿ��ת����GameObject
                //as�������ת�����ɹ�ʱ����null���棬�����׳��쳣
                GameObject newObject = Instantiate(prefab, parentGameObject.transform) as GameObject;
                newObject.SetActive(false);
                //��ʵ������object��ӵ�����(�Ƚ��ȳ�)�Ľ�β��
                poolDictionary[poolKey].Enqueue(newObject);
            }
        }
    }

    public GameObject ReuseObject(GameObject prefab,Vector3 position,Quaternion rotation)
    {
        int poolKey = prefab.GetInstanceID();

        if (poolDictionary.ContainsKey(poolKey))
        {
            GameObject reuseObject = GetObjectFromPool(poolKey);

            ResetObject(position, rotation, reuseObject, prefab);

            return reuseObject;
        }
        else
        {
            Debug.Log("No object pool for" + prefab.name);
            return null;
        }
    }

    private GameObject GetObjectFromPool(int poolKey)
    {
        //�Ѷ���Ӷ��е�һ���ó��������������һ��
        GameObject gameObject = poolDictionary[poolKey].Dequeue();
        poolDictionary[poolKey].Enqueue(gameObject);

        if(gameObject.activeSelf == true)
        {
            gameObject.SetActive(false);
        }
        return gameObject;
    }

    private void ResetObject(Vector3 position, Quaternion rotation, GameObject reuseObject, GameObject prefab)
    {
        reuseObject.transform.position = position;
        reuseObject.transform.rotation = rotation;
        //���ö���ı������ű�����lossyScale����������������ű���
        reuseObject.transform.localScale = prefab.transform.localScale;
    }
}
