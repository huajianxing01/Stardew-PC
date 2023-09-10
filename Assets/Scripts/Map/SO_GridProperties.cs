using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="so_GridProperties",menuName ="Scriptable Objects/Grid Properties")]
public class SO_GridProperties : ScriptableObject
{
    public SceneName SceneName;
    //width和height获取的是tilemap的最大边界
    public int gridWidth;
    public int gridHeight;
    //获取tilemap原点，总是在左下角(0,0)
    public int originX;
    public int originY;

    [SerializeField] public List<GridProperty> gridPropertyList;
}