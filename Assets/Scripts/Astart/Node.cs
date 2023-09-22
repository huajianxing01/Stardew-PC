using System;
using UnityEngine;

public class Node : IComparable<Node>
{
    public Vector2Int gridPosition;
    public int gCost = 0;//��starting node�ľ���
    public int hCost = 0;//��finishing node�ľ���
    public bool isObstacle = false;//�ж��Ƿ��ϰ�node
    public int movementPenalty;//�ƶ��ͷ�����A*�㷨���ո�����path����
    public Node parentNode;

    public int FCost { get => gCost + hCost; }

    public Node(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;
        parentNode = null;
    }

    public int CompareTo(Node nodeToCampare)
    {
        //����˽ڵ��FCostС��nodeToCampare��FCost����compareС��0
        //����˽ڵ��FCost����nodeToCampare��FCost����compare����0
        //����˽ڵ��FCost����nodeToCampare��FCost����compare����0
        int compare = FCost.CompareTo(nodeToCampare.FCost);
        if(compare == 0) compare = hCost.CompareTo(nodeToCampare.hCost);
        
        return compare;
    }
}
