using System;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    [Header("Tiles & Tilemap 引用")]
    [Header("A*算法选项")]
    [SerializeField] private bool observeMovementPenalties = true;//观察移动惩罚
    [Range(0f, 20f)][SerializeField]private int pathMovementPenalty = 0;//路径移动惩罚
    [Range(0f, 20f)][SerializeField] private int defaultMovementPenalty = 0;//默认移动惩罚

    private GridNodes gridNodes;
    private Node startNode;
    private Node targetNode;
    private int gridWidth;
    private int gridHeight;
    private int gridOriginX;
    private int gridOriginY;

    private List<Node> openNodeList;
    private List<Node> closeNodeList;
    
    private bool pathFound = false;

    public bool BuildPath(SceneName sceneName,Vector2Int startGridPosition,Vector2Int endGridPosition, Stack<NPCMovementStep> npcMovementStepStack)
    {
        pathFound = false;
        if (PopulateGridNodeFromGridPropertiesDictionary(sceneName, startGridPosition, endGridPosition))
        {
            if (FindShortesPath())
            {
                UpdatePathOnNPCMovementStepStack(sceneName, npcMovementStepStack);
                return true;
            }
        }
        return false;
    }

    private void UpdatePathOnNPCMovementStepStack(SceneName sceneName, Stack<NPCMovementStep> npcMovementStepStack)
    {
        Node nextNode = targetNode;

        while (nextNode != null)
        {
            NPCMovementStep npcMovementStep = new NPCMovementStep();
            npcMovementStep.SceneName = sceneName;
            npcMovementStep.gridCoordinate = new Vector2Int(nextNode.gridPosition.x + gridOriginX, nextNode.gridPosition.y + gridOriginY);
            //从终点到起点，push到栈里，先入后出
            npcMovementStepStack.Push(npcMovementStep);
            nextNode = nextNode.parentNode;//到起点后没有父节点，退出循环
        }
    }

    /// <summary>
    /// A*算法找出最短路径
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private bool FindShortesPath()
    {
        openNodeList.Add(startNode);

        while(openNodeList.Count > 0)
        {
            openNodeList.Sort();
            //第一个是openNodeList中FCost最小的
            Node currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);
            closeNodeList.Add(currentNode);

            if(currentNode == targetNode)
            {
                pathFound = true;
                break;
            }
            //计算当前Node周围8个Node
            EvaluateCurrentNodeNeighbours(currentNode);
        }

        if (pathFound)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void EvaluateCurrentNodeNeighbours(Node currentNode)
    {
        Vector2Int currentGridPosition = currentNode.gridPosition;
        Node validNeighbourNode;//有效邻居node

        for (int i = -1; i <= 1; i++) 
        {
            for(int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }

                validNeighbourNode = GetValidNeighbour(currentGridPosition.x + i, currentGridPosition.y + j);

                if(validNeighbourNode != null)
                {
                    int newCostToNeighbour;//计算新的gCost
                    if(observeMovementPenalties)
                    {
                        newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode) + validNeighbourNode.movementPenalty;
                    }
                    else
                    {
                        //正常情况下，计算(start node到当前node的)gCost+当前node到邻居node的距离
                        newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode);
                    }

                    bool isValidNeighbourNodeInOpenList = openNodeList.Contains(validNeighbourNode);
                    if (newCostToNeighbour < validNeighbourNode.gCost || !isValidNeighbourNodeInOpenList) 
                    {
                        validNeighbourNode.gCost = newCostToNeighbour;
                        validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
                        validNeighbourNode.parentNode = currentNode;

                        if (!isValidNeighbourNodeInOpenList)
                        {
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        if (dstX > dstY) return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    private Node GetValidNeighbour(int positionX, int positionY)
    {
        //邻居node在地图外，无效
        if (positionX >= gridWidth || positionX < 0 || positionY >= gridHeight || positionY < 0)
        {
            return null;
        }

        Node neighbourNode = gridNodes.GetGridNode(positionX, positionY);
        //邻居node是障碍物或已在close列表中，无效
        if (neighbourNode.isObstacle || closeNodeList.Contains(neighbourNode))
        {
            return null;
        }
        else
        {
            return neighbourNode;
        }
    }

    /// <summary>
    /// 从场景Grid属性字典里获取数据并填充grid node网格
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="startGridPosition"></param>
    /// <param name="endGridPosition"></param>
    /// <returns></returns>
    private bool PopulateGridNodeFromGridPropertiesDictionary(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition)
    {
        SceneSave sceneSave;

        if (GridPropertiesManager.Instance.GameObjectSave.sceneData.TryGetValue(sceneName.ToString(), out sceneSave))
        {
            if(sceneSave.gridPropertyDetailsDictionary != null)
            {
                //获取scene上的grid数据，并实例化两个列表
                if (GridPropertiesManager.Instance.GetGridDimensions(sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin)) 
                {
                    gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);
                    gridWidth = gridDimensions.x;
                    gridHeight = gridDimensions.y;
                    gridOriginX = gridOrigin.x;
                    gridOriginY = gridOrigin.y;
                    openNodeList = new List<Node>();
                    closeNodeList = new List<Node>();
                }
                else
                {
                    return false;
                }
                //tilemap的原点在地图中间，不一定是（0，0），需要调整要用的网格原点为左下角（0，0）
                startNode = gridNodes.GetGridNode(startGridPosition.x - gridOrigin.x, startGridPosition.y - gridOrigin.y);
                targetNode = gridNodes.GetGridNode(endGridPosition.x - gridOrigin.x, endGridPosition.y - gridOrigin.y);

                for(int x = 0; x < gridDimensions.x; x++)
                {
                    for(int y = 0; y < gridDimensions.y; y++)
                    {
                        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(x + gridOrigin.x,
                            y + gridOrigin.y, sceneSave.gridPropertyDetailsDictionary);
                        if (gridPropertyDetails != null)
                        {
                            if (gridPropertyDetails.isNPCObstacle)
                            {
                                Node node = gridNodes.GetGridNode(x, y);
                                node.isObstacle = true;
                            }
                            else if (gridPropertyDetails.isPath)
                            {
                                Node node = gridNodes.GetGridNode(x, y);
                                node.movementPenalty = pathMovementPenalty;
                            }
                            else
                            {
                                Node node = gridNodes.GetGridNode(x, y);
                                node.movementPenalty = defaultMovementPenalty;
                            }
                        }
                    }
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
        return true;
    }
}
