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
        //获得唯一的ID和对应name
        int poolKey = prefab.GetInstanceID();
        string prefabName = prefab.name;
        //创建一个父对象，以便实例化子对象
        GameObject parentGameObject = new GameObject(prefabName + "Anchor");
        parentGameObject.transform.SetParent(objectPoolTransform);

        if (!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary.Add(poolKey, new Queue<GameObject>());
            for(int i = 0; i < poolSize; i++)
            {
                //Instantiate实例化出的对象是Object类型,需要强制转换成GameObject
                //as运算符在转化不成功时候用null代替，不会抛出异常
                GameObject newObject = Instantiate(prefab, parentGameObject.transform) as GameObject;
                newObject.SetActive(false);
                //把实例化的object添加到队列(先进先出)的结尾处
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
        //把对象从队列第一个拿出，塞到队列最后一个
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
        //重置对象的本身缩放比例，lossyScale则是物体的世界缩放比例
        reuseObject.transform.localScale = prefab.transform.localScale;
    }
}
