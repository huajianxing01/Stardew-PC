using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(GenerateGUID))]
public class GridPropertiesManager : SingletonMonobehaviour<GridPropertiesManager>, ISaveable
{
    private Tilemap groundDecoration1;
    private Tilemap groundDecoration2;

    private Grid grid;
    //key��grid������
    private Dictionary<string, GridPropertyDetails> gridPropertyDictionary;
    [SerializeField] private SO_GridProperties[] so_GridPropertiesArray = null;
    [SerializeField] private Tile[] dugGround = null;
    [SerializeField] private Tile[] waterGround = null;

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get => _iSaveableUniqueID; set => _iSaveableUniqueID = value; }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get => _gameObjectSave; set => _gameObjectSave = value; }

    protected override void Awake()
    {
        base.Awake();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void Start()
    {
        InitialiseGridProperties();
    }

    private void InitialiseGridProperties()
    {
        foreach (var properties in so_GridPropertiesArray)
        {
            Dictionary<string, GridPropertyDetails> gridPropertyDictionary = new Dictionary<string, GridPropertyDetails>();

            foreach(var gridProperty in properties.gridPropertyList)
            {
                GridPropertyDetails gridPropertyDetails;
                gridPropertyDetails = GetGridPropertyDetails(gridProperty.gridCoordinate.x,
                    gridProperty.gridCoordinate.y, gridPropertyDictionary);
                //�ж�dictionary��(x,y)��Ӧ��grid detail��Ϣ�Ƿ�Ϊnull
                if(gridPropertyDetails == null)
                {
                    gridPropertyDetails = new GridPropertyDetails();
                }

                switch(gridProperty.gridBoolProperty)
                {
                    case GridBoolProperty.diggable:
                        gridPropertyDetails.isDiggable = gridProperty.gridBoolValue;
                        break;
                    case GridBoolProperty.canDropItem:
                        gridPropertyDetails.canDropItem = gridProperty.gridBoolValue;
                        break;
                    case GridBoolProperty.canPlaceFurniture:
                        gridPropertyDetails.canPlaceFurniture = gridProperty.gridBoolValue;
                        break;
                    case GridBoolProperty.isPath:
                        gridPropertyDetails.isPath = gridProperty.gridBoolValue;
                        break;
                    case GridBoolProperty.isNPCObstacle:
                        gridPropertyDetails.isNPCObstacle = gridProperty.gridBoolValue;
                        break;
                    default:
                        break;
                }
                //����grid������Ϣ��ѭ������ܻ�����е�boolֵ
                SetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDetails, gridPropertyDictionary);
            }
            //��grid��dictionary���浽scenesave��
            SceneSave sceneSave = new SceneSave();
            sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;

            if(properties.SceneName.ToString() == SceneControllerManager.Instance.startingSceneName.ToString())
            {
                this.gridPropertyDictionary = gridPropertyDictionary;
            }
            //��scenesave���浽gameobjectsave��
            GameObjectSave.sceneData.Add(properties.SceneName.ToString(), sceneSave);
        }
    }

    private void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        string key = "x" + gridX + "y" + gridY;
        gridPropertyDetails.gridX = gridX;
        gridPropertyDetails.gridY = gridY;
        gridPropertyDictionary[key] = gridPropertyDetails;
    }

    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails)
    {
        SetGridPropertyDetails(gridX, gridY, gridPropertyDetails, gridPropertyDictionary);
    }

    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        //����dictionary��string key
        string key = "x" + gridX + "y" + gridY;
        GridPropertyDetails gridPropertyDetails;

        if(!gridPropertyDictionary.TryGetValue(key,out gridPropertyDetails))
        {
            return null;
        }
        else
        {
            return gridPropertyDetails;
        }
    }

    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY)
    {
        return GetGridPropertyDetails(gridX, gridY, gridPropertyDictionary);
    }

    private void OnEnable()
    {
        ISaveableRegister();

        EventHandler.AfterSceneLoadEvent += AfterSceneLoaded;
        EventHandler.AdvanceGameDayEvent += AdvanceDay;
    }

    private void OnDisable()
    {
        ISaveableDeregister();

        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded;
        EventHandler.AdvanceGameDayEvent -= AdvanceDay;
    }

    private void AdvanceDay(int year, Season season, int day, Week week, int hour, int minute, int second)
    {
        ClearDisplayGridPropertyDetails();

        //waterÿ��һ��գ���Ҫ��������scene��grid��Ϣ����������ǰscene��
        foreach(var gridProperties in so_GridPropertiesArray)
        {
            if (GameObjectSave.sceneData.TryGetValue(gridProperties.SceneName.ToString(), out SceneSave sceneSave))
            {
                if(sceneSave.gridPropertyDetailsDictionary != null)
                {
                    //�������dictionary���޸�key��Ӧ��valueֵ
                    for (int i = sceneSave.gridPropertyDetailsDictionary.Count - 1; i >= 0; i--)
                    {
                        KeyValuePair<string, GridPropertyDetails> item = sceneSave.gridPropertyDetailsDictionary.ElementAt(i);
                        GridPropertyDetails gridPropertyDetails = item.Value;

                        if (gridPropertyDetails.daysSinceWatered > -1)
                        {
                            gridPropertyDetails.daysSinceWatered = -1;
                        }

                        SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY,
                            gridPropertyDetails, sceneSave.gridPropertyDetailsDictionary);
                    }
                }
            }
        }

        DisplayGridPropertyDetails();
    }

    private void AfterSceneLoaded()
    {
        grid = GameObject.FindObjectOfType<Grid>();
        groundDecoration1 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration1).GetComponent<Tilemap>();
        groundDecoration2 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration2).GetComponent<Tilemap>();
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        if(GameObjectSave.sceneData.TryGetValue(sceneName,out SceneSave sceneSave))
        {
            if(sceneSave.gridPropertyDetailsDictionary != null)
            {
                gridPropertyDictionary = sceneSave.gridPropertyDetailsDictionary;
            }

            if (gridPropertyDictionary.Count > 0)
            {
                ClearDisplayGridPropertyDetails();
                DisplayGridPropertyDetails();
            }
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {
        GameObjectSave.sceneData.Remove(sceneName);

        SceneSave sceneSave = new SceneSave();
        sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;

        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }

    private void ClearDisplayGroundDecoration()
    {
        groundDecoration1.ClearAllTiles();
        groundDecoration2.ClearAllTiles();
    }

    private void ClearDisplayGridPropertyDetails()
    {
        ClearDisplayGroundDecoration();
    }

    public void DisplayDugGround(GridPropertyDetails gridPropertyDetails)
    {
        if(gridPropertyDetails.daysSinceDug > -1)
        {
            ConnectDugGround(gridPropertyDetails);
        }
    }

    private void ConnectDugGround(GridPropertyDetails gridPropertyDetails)
    {
        Tile dugTile0 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), dugTile0);

        //set 4 tiles of center tile dugtile0,
        GridPropertyDetails adjacentGridPropertyDetails;
        //up
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
        if(adjacentGridPropertyDetails != null &&adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile1 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0), dugTile1);
        }
        //down
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile2 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1, 0), dugTile2);
        }
        //left
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile3 = SetDugTile(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY, 0), dugTile3);
        }
        //right
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile4 = SetDugTile(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY, 0), dugTile4);
        }
    }

    private Tile SetDugTile(int gridX, int gridY)
    {
        //get 4 tile around center tile
        bool upDug = IsGridSquareDug(gridX, gridY + 1);
        bool downDug = IsGridSquareDug(gridX, gridY - 1);
        bool leftDug = IsGridSquareDug(gridX - 1, gridY);
        bool rightDug = IsGridSquareDug(gridX + 1, gridY);

        #region ��õ�ǰgrid��Ӧ��Dug��ͼ
        if(!upDug && !downDug && !leftDug && !rightDug)
        {
            return dugGround[0];
        }
        else if(!upDug && downDug && !leftDug && rightDug)
        {
            return dugGround[1];
        }
        else if (!upDug && downDug && leftDug && rightDug)
        {
            return dugGround[2];
        }
        else if (!upDug && downDug && leftDug && !rightDug)
        {
            return dugGround[3];
        }
        else if (!upDug && downDug && !leftDug && !rightDug)
        {
            return dugGround[4];
        }
        else if (upDug && downDug && !leftDug && rightDug)
        {
            return dugGround[5];
        }
        else if (upDug && downDug && leftDug && rightDug)
        {
            return dugGround[6];
        }
        else if (upDug && downDug && leftDug && !rightDug)
        {
            return dugGround[7];
        }
        else if (upDug && downDug && !leftDug && !rightDug)
        {
            return dugGround[8];
        }
        else if (upDug && !downDug && !leftDug && rightDug)
        {
            return dugGround[9];
        }
        else if (upDug && !downDug && leftDug && rightDug)
        {
            return dugGround[10];
        }
        else if (upDug && !downDug && leftDug && !rightDug)
        {
            return dugGround[11];
        }
        else if (upDug && !downDug && !leftDug && !rightDug)
        {
            return dugGround[12];
        }
        else if (!upDug && !downDug && !leftDug && rightDug)
        {
            return dugGround[13];
        }
        else if (!upDug && !downDug && leftDug && rightDug)
        {
            return dugGround[14];
        }
        else if (!upDug && !downDug && leftDug && !rightDug)
        {
            return dugGround[15];
        }
        else 
        {
            return null;
        }

        #endregion
    }

    public void DisplayWaterGround(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.daysSinceWatered > -1)
        {
            ConnectWaterGround(gridPropertyDetails);
        }
    }

    private void ConnectWaterGround(GridPropertyDetails gridPropertyDetails)
    {
        Tile waterTile0 = SetWaterTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), waterTile0);

        //set 4 tiles of center tile waterTile0,
        GridPropertyDetails adjacentGridPropertyDetails;
        //up
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile waterTile1 = SetWaterTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0), waterTile1);
        }
        //down
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile waterTile2 = SetWaterTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1, 0), waterTile2);
        }
        //left
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile waterTile3 = SetWaterTile(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY, 0), waterTile3);
        }
        //right
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile waterTile4 = SetWaterTile(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY, 0), waterTile4);
        }
    }

    private Tile SetWaterTile(int gridX, int gridY)
    {
        //get 4 tile around center tile
        bool upWater = IsGridSquareWater(gridX, gridY + 1);
        bool downWater = IsGridSquareWater(gridX, gridY - 1);
        bool leftWater = IsGridSquareWater(gridX - 1, gridY);
        bool rightWater = IsGridSquareWater(gridX + 1, gridY);

        #region ��õ�ǰgrid��Ӧ��Water��ͼ
        if (!upWater && !downWater && !leftWater && !rightWater)
        {
            return waterGround[0];
        }
        else if (!upWater && downWater && !leftWater && rightWater)
        {
            return waterGround[1];
        }
        else if (!upWater && downWater && leftWater && rightWater)
        {
            return waterGround[2];
        }
        else if (!upWater && downWater && leftWater && !rightWater)
        {
            return waterGround[3];
        }
        else if (!upWater && downWater && !leftWater && !rightWater)
        {
            return waterGround[4];
        }
        else if (upWater && downWater && !leftWater && rightWater)
        {
            return waterGround[5];
        }
        else if (upWater && downWater && leftWater && rightWater)
        {
            return waterGround[6];
        }
        else if (upWater && downWater && leftWater && !rightWater)
        {
            return waterGround[7];
        }
        else if (upWater && downWater && !leftWater && !rightWater)
        {
            return waterGround[8];
        }
        else if (upWater && !downWater && !leftWater && rightWater)
        {
            return waterGround[9];
        }
        else if (upWater && !downWater && leftWater && rightWater)
        {
            return waterGround[10];
        }
        else if (upWater && !downWater && leftWater && !rightWater)
        {
            return waterGround[11];
        }
        else if (upWater && !downWater && !leftWater && !rightWater)
        {
            return waterGround[12];
        }
        else if (!upWater && !downWater && !leftWater && rightWater)
        {
            return waterGround[13];
        }
        else if (!upWater && !downWater && leftWater && rightWater)
        {
            return waterGround[14];
        }
        else if (!upWater && !downWater && leftWater && !rightWater)
        {
            return waterGround[15];
        }
        else
        {
            return null;
        }

        #endregion
    }

    //�ж�grid�Ƿ񿪿���
    private bool IsGridSquareDug(int gridX, int gridY)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(gridX, gridY);
        if(gridPropertyDetails == null)
        {
            return false;
        }
        else if (gridPropertyDetails.daysSinceDug > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }

    //�ж�grid�Ƿ���ˮ��
    private bool IsGridSquareWater(int gridX, int gridY)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(gridX, gridY);
        if (gridPropertyDetails == null)
        {
            return false;
        }
        else if (gridPropertyDetails.daysSinceWatered > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void DisplayGridPropertyDetails()
    {
        foreach(KeyValuePair<string,GridPropertyDetails> item in gridPropertyDictionary)
        {
            GridPropertyDetails propertyDetails = item.Value;
            DisplayDugGround(propertyDetails);
            DisplayWaterGround(propertyDetails);
        }
    }
}