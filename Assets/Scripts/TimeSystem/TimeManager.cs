using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : SingletonMonobehaviour<TimeManager>
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
}
