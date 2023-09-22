using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : SingletonMonobehaviour<TimeManager>, ISaveable
{
    private int gameYear = 1;
    private Season gameSeason = Season.Spring;
    private int gameDay = 1;
    private int gameHour = 6;
    private int gameMinute = 30;
    private int gameSecond = 0;
    private Week gameDayOfWeek = Week.Mon;

    private bool gameClockPaused = false;
    private float gameTick = 0;

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
        EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, 
            gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    private void Update()
    {
        if(!gameClockPaused)
        {
            GameTick();
        }
    }

    private void OnEnable()
    {
        ISaveableRegister();
        EventHandler.BeforeSceneUnloadEvent += BeforeSceneUnloadFadeOut;
        EventHandler.AfterSceneLoadEvent += AfterSceneLoadFadeIn;
    }

    private void OnDisable()
    {
        ISaveableDeregister();
        EventHandler.BeforeSceneUnloadEvent -= BeforeSceneUnloadFadeOut;
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoadFadeIn;
    }

    private void AfterSceneLoadFadeIn()
    {
        gameClockPaused = false;
    }

    private void BeforeSceneUnloadFadeOut()
    {
        gameClockPaused = true;
    }

    private void GameTick()
    {
        gameTick += Time.deltaTime;
        if(gameTick > Settings.secondsPerGameSecond)
        {
            gameTick -= Settings.secondsPerGameSecond;
            UpdateGameSecond();
        }
    }

    private void UpdateGameSecond()
    {
        gameSecond++;
        if (gameSecond > 59)
        {
            gameSecond = 0;
            gameMinute++;
            if(gameMinute > 59)
            {
                gameMinute = 0;
                gameHour++;
                if(gameHour > 23)
                {
                    gameHour = 0;
                    gameDay++;
                    if (gameDay > 30)
                    {
                        gameDay = 1;
                        int gs = (int)gameSeason;
                        gs++;
                        gameSeason = (Season)gs;
                        if (gs > 3)
                        {
                            gs = 0;
                            gameSeason = (Season)gs;
                            gameYear++;
                            //如果年份达到上限，重置回第一年，以防溢出
                            if(gameYear>9999) gameYear = 1;

                            EventHandler.CallAdvanceGameYearEvent(gameYear,gameSeason,gameDay,
                                gameDayOfWeek,gameHour,gameMinute,gameSecond);
                        }

                        EventHandler.CallAdvanceGameSeasonEvent(gameYear, gameSeason, gameDay,
                                gameDayOfWeek, gameHour, gameMinute, gameSecond);
                    }

                    gameDayOfWeek = GetDayOfWeek();
                    EventHandler.CallAdvanceGameDayEvent(gameYear, gameSeason, gameDay,
                                gameDayOfWeek, gameHour, gameMinute, gameSecond);
                }

                EventHandler.CallAdvanceGameHourEvent(gameYear, gameSeason, gameDay,
                                gameDayOfWeek, gameHour, gameMinute, gameSecond);
            }

            EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay,
                                gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }

    private Week GetDayOfWeek()
    {
        int allDay = ((int)gameSeason * 30) + gameDay;
        int dayOfWeek = allDay % 7;
        switch (dayOfWeek)
        {
            case 1:
                return Week.Mon;
            case 2:
                return Week.Tues;
            case 3: 
                return Week.Wed;
            case 4:
                return Week.Thur;
            case 5:
                return Week.Fri;
            case 6:
                return Week.Sat;
            case 0:
                return Week.Sun;
            default:
                return Week.none;
        }
    }

    public TimeSpan GetGameTime()
    {
        //timespan结构表示一个时间间隔
        TimeSpan gameTime = new TimeSpan(gameHour, gameMinute, gameSecond);
        return gameTime;
    }

    public string GetSeasonChineseWord(Season season)
    {
        string seasonTmp;
        switch (season)
        {
            case Season.Spring:
                seasonTmp = Settings.spring;
                break;
            case Season.Summer:
                seasonTmp = Settings.summer;
                break;
            case Season.Autumn:
                seasonTmp = Settings.autumn;
                break;
            case Season.Winter:
                seasonTmp = Settings.winter;
                break;
            default:
                seasonTmp = season.ToString();
                break;
        }
        return seasonTmp;
    }

    public string GetWeekChineseWord(Week week)
    {
        string weekTmp;
        switch (week)
        {
            case Week.Mon:
                weekTmp = Settings.monday;
                break;
            case Week.Tues:
                weekTmp = Settings.tuesday;
                break;
            case Week.Wed:
                weekTmp = Settings.wednesday;
                break;
            case Week.Thur:
                weekTmp = Settings.thursday;
                break;
            case Week.Fri:
                weekTmp = Settings.friday;
                break;
            case Week.Sat:
                weekTmp = Settings.saturday;
                break;
            case Week.Sun:
                weekTmp = Settings.sunday;
                break;
            default:
                weekTmp = week.ToString();
                break;
        }
        return weekTmp;
    }

    public void TestAdvanceGameMinute()
    {
        for(int i = 0; i < 60*10; i++)
        {
            UpdateGameSecond();
        }
    }

    public void TestAdvanceGameDay()
    {
        for (int i = 0; i < 60*60*24; i++)
        {
            UpdateGameSecond();
        }
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public void ISaveableStoreScene(string sceneName)
    {
        //无
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        //无
    }

    public GameObjectSave ISaveableSave()
    {
        GameObjectSave.sceneData.Remove(Settings.persistentScene);

        SceneSave sceneSave = new SceneSave();
        sceneSave.intDictionary = new Dictionary<string, int>();
        sceneSave.stringDictionary = new Dictionary<string, string>();

        sceneSave.intDictionary.Add("gameYear", gameYear);
        sceneSave.intDictionary.Add("gameDay", gameDay);
        sceneSave.intDictionary.Add("gameHour", gameHour);
        sceneSave.intDictionary.Add("gameMinute", gameMinute);
        sceneSave.intDictionary.Add("gameSecond", gameSecond);
        sceneSave.stringDictionary.Add("gameWeek", gameDayOfWeek.ToString());
        sceneSave.stringDictionary.Add("gameSeason", gameSeason.ToString());
        GameObjectSave.sceneData.Add(Settings.persistentScene, sceneSave);

        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave.GameData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;
            if (GameObjectSave.sceneData.TryGetValue(Settings.persistentScene, out SceneSave sceneSave))
            {
                if (sceneSave.intDictionary != null && sceneSave.stringDictionary != null)
                {
                    if (sceneSave.intDictionary.TryGetValue("gameYear", out int savedGameYear)) gameYear = savedGameYear;
                    if (sceneSave.intDictionary.TryGetValue("gameDay", out int savedGameDay)) gameDay = savedGameDay;
                    if (sceneSave.intDictionary.TryGetValue("gameHour", out int savedGameHour)) gameHour = savedGameHour;
                    if (sceneSave.intDictionary.TryGetValue("gameMinute", out int savedGameMinute)) gameMinute = savedGameMinute;
                    if (sceneSave.intDictionary.TryGetValue("gameSecond", out int savedGameSecond)) gameSecond = savedGameSecond;
                    if (sceneSave.stringDictionary.TryGetValue("gameWeek", out string savedGameWeek))
                    {
                        if(Enum.TryParse<Week>(savedGameWeek, out Week gameofWeek))
                        gameDayOfWeek = gameofWeek;
                    }
                    if (sceneSave.stringDictionary.TryGetValue("gameSeason", out string savedGameSeason))
                    {
                        if (Enum.TryParse<Season>(savedGameSeason, out Season gameofSeason))
                            gameSeason = gameofSeason;
                    }
                    gameTick = 0;

                    EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                }
            }
        }
    }
}
