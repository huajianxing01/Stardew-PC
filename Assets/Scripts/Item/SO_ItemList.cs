using System.Collections.Generic;
using UnityEngine;

//��Creat�˵��д����Զ���˵�
[CreateAssetMenu(fileName ="so_ItemList",menuName ="Scriptable Objects/Item/Item List")]
public class SO_ItemList : ScriptableObject
{
    //���л�һ��item details���͵�list����unity�п��ӻ�&�ɴ洢
    [SerializeField] public List<ItemDetails> ItemDetails;
}
