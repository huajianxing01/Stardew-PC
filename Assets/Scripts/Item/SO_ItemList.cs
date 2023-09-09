using System.Collections.Generic;
using UnityEngine;

//在Creat菜单中创建自定义菜单
[CreateAssetMenu(fileName ="so_ItemList",menuName ="Scriptable Objects/Item/Item List")]
public class SO_ItemList : ScriptableObject
{
    //序列化一个item details类型的list，在unity中可视化
    [SerializeField] public List<ItemDetails> ItemDetails;
}
