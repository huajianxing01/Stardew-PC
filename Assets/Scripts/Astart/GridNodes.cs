using UnityEngine;

public class GridNodes
{
    private int width;
    private int height;
    private Node[,] gridNodes;

    public GridNodes(int width, int height)
    {
        this.width = width;
        this.height = height;
        gridNodes = new Node[width, height];
        //����ͼ����grid node��ʼ��
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                gridNodes[i, j] = new Node(new Vector2Int(i, j));
            }
        }
    }
    /// <summary>
    /// ����(x,y)��ȡGrid�϶�Ӧ��Nodeֵ
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Node GetGridNode(int x, int y)
    {
        if (x < width && y < height)
        {
            return gridNodes[x, y];
        }
        else
        {
            Debug.Log("������Grid Node������֪��ͼ��Χ");
            return null;
        }
    }
}