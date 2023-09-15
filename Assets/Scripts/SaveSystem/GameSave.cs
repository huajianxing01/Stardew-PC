using System.Collections.Generic;

[System.Serializable]
public class GameSave
{
    public Dictionary<string, GameObjectSave> GameData;

    public GameSave()
    {
        GameData = new Dictionary<string, GameObjectSave>();
    }

}
