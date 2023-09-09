using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="so_GridProperties",menuName ="Scriptable Objects/Grid Properties")]
public class SO_GridProperties : ScriptableObject
{
    public SceneName SceneName;
    //width��height��ȡ����tilemap�����߽�
    public int gridWidth;
    public int gridHeight;
    //��ȡtilemapԭ�㣬���������½�(0,0)
    public int originX;
    public int originY;

    [SerializeField] public List<GridProperty> gridPropertyList;
}