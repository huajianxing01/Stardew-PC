using System;
using UnityEngine;

public class Node : IComparable<Node>
{
    public Vector2Int gridPosition;
    public int gCost = 0;//与starting node的距离
    public int hCost = 0;//与finishing node的距离
    public bool isObstacle = false;//判断是否障碍node
    public int movementPenalty;//移动惩罚，让A*算法按照给定的path行走
    public Node parentNode;

    public int FCost { get => gCost + hCost; }

    public Node(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;
        parentNode = null;
    }

    public int CompareTo(Node nodeToCampare)
    {
        //如果此节点的FCost小于nodeToCampare的FCost，则compare小于0
        //如果此节点的FCost大于nodeToCampare的FCost，则compare大于0
        //如果此节点的FCost等于nodeToCampare的FCost，则compare等于0
        int compare = FCost.CompareTo(nodeToCampare.FCost);
        if(compare == 0) compare = hCost.CompareTo(nodeToCampare.hCost);
        
        return compare;
    }
}
