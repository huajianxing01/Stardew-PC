using System;
using System.Collections.Generic;

//声明一个Movement委托类型
public delegate void MovementDelegate(float inputX, float inputY, bool isWalking,bool isRunning, bool isIdle, bool isCarrying,
    ToolEffect toolEffect,
    bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown,
    bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown,
    bool isPickingRight, bool isPickingLeft, bool isPickingUp, bool isPickingDown,
    bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown,
    bool idleRight, bool idleLeft, bool idleUp, bool idleDown);

public static class EventHandler
{
    public static event Action DropSelectedItemEvent;
    public static void CallDropSelectedItemEvent()
    {
        if(DropSelectedItemEvent != null)
        {
            DropSelectedItemEvent();
        }
    }

    //声明一个Action委托事件，一个库存位置参数，一个库存列表参数
    public static event Action<InventoryLocation, List<InventoryItem>> InventoryUpdatedEvent;
    //Publishers调用event的方法
    public static void CallInventoryUpdatedEvent(InventoryLocation inventoryLocation, List<InventoryItem> inventoryItemsList)
    {
        if (InventoryUpdatedEvent != null)
        {
            InventoryUpdatedEvent(inventoryLocation, inventoryItemsList);
        }
    }

    //声明一个Action委托事件，处理Time Event-Minute
    public static event Action<int, Season, int, Week, int, int, int> AdvanceGameMinuteEvent;
    public static void CallAdvanceGameMinuteEvent(int gameYear, Season gameSeason, int gameDay,
        Week gameDayOfWeek, int gamehour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameMinuteEvent != null)
        {
            AdvanceGameMinuteEvent(gameYear, gameSeason, gameDay,
                gameDayOfWeek, gamehour, gameMinute, gameSecond);
        }
    }

    //声明一个Action委托事件，处理Time Event-Hour
    public static event Action<int, Season, int, Week, int, int, int> AdvanceGameHourEvent;
    public static void CallAdvanceGameHourEvent(int gameYear, Season gameSeason, int gameDay,
        Week gameDayOfWeek, int gamehour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameHourEvent != null)
        {
            AdvanceGameHourEvent(gameYear, gameSeason, gameDay,
                gameDayOfWeek, gamehour, gameMinute, gameSecond);
        }
    }

    //声明一个Action委托事件，处理Time Event-Day
    public static event Action<int, Season, int, Week, int, int, int> AdvanceGameDayEvent;
    public static void CallAdvanceGameDayEvent(int gameYear, Season gameSeason, int gameDay,
        Week gameDayOfWeek, int gamehour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameDayEvent != null)
        {
            AdvanceGameDayEvent(gameYear, gameSeason, gameDay,
                gameDayOfWeek, gamehour, gameMinute, gameSecond);
        }
    }

    //声明一个Action委托事件，处理Time Event-Season
    public static event Action<int, Season, int, Week, int, int, int> AdvanceGameSeasonEvent;
    public static void CallAdvanceGameSeasonEvent(int gameYear, Season gameSeason, int gameDay,
        Week gameDayOfWeek, int gamehour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameSeasonEvent != null)
        {
            AdvanceGameSeasonEvent(gameYear, gameSeason, gameDay,
                gameDayOfWeek, gamehour, gameMinute, gameSecond);
        }
    }

    //声明一个Action委托事件，处理Time Event-Year
    public static event Action<int, Season, int, Week, int, int, int> AdvanceGameYearEvent;
    public static void CallAdvanceGameYearEvent(int gameYear, Season gameSeason, int gameDay,
        Week gameDayOfWeek, int gamehour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameYearEvent != null)
        {
            AdvanceGameYearEvent(gameYear, gameSeason, gameDay,
                gameDayOfWeek, gamehour, gameMinute, gameSecond);
        }
    }

    //基于委托类型声明的Movement事件，看作一个委托字段，public才能被其他class在上面注册事件处理程序
    //事件是类/结构中的一个成员，必须声明在类/结构中，自动隐式初始化为null，只能有+=和-=两种操作
    //事件隐式声明，在编译时候，会生成一个和事件名相同的私有委托，触发事件也是对同名私有委托进行调用
    public static event MovementDelegate MovementEvent;
    //Publishers调用event的方法
    public static void CallMovementEvent(float inputX, float inputY, bool isWalking, bool isRunning, bool isIdle, bool isCarrying,
    ToolEffect toolEffect,
    bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown,
    bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown,
    bool isPickingRight, bool isPickingLeft, bool isPickingUp, bool isPickingDown,
    bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown,
    bool idleRight, bool idleLeft, bool idleUp, bool idleDown)
    {
        if (MovementEvent != null)
        {
            //触发Movement委托事件，和调用方法一样，事件名(参数列表)
            MovementEvent(inputX, inputY, isWalking, isRunning, isIdle, isCarrying,
                toolEffect,
                isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
                isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
                isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
                isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
                idleRight, idleLeft, idleUp, idleDown);
        }
    }

    public static event Action BeforeSceneUnloadFadeOutEvent;
    //前一场景退出-淡出效果事件
    public static void CallBeforeSceneUnloadFadeOutEvent()
    {
        if( BeforeSceneUnloadFadeOutEvent != null )
        {
            BeforeSceneUnloadFadeOutEvent();
        }
    }

    public static event Action BeforeSceneUnloadEvent;
    //前一场景退出事件
    public static void CallBeforeSceneUnloadEvent()
    {
        if (BeforeSceneUnloadEvent != null)
        {
            BeforeSceneUnloadEvent();
        }
    }

    public static event Action AfterSceneLoadEvent;
    //后一场景加载事件
    public static void CallAfterSceneLoadEvent()
    {
        if (AfterSceneLoadEvent != null)
        {
            AfterSceneLoadEvent();
        }
    }

    public static event Action AfterSceneLoadFadeInEvent;
    //后一场景加载-淡入效果事件
    public static void CallAfterSceneLoadFadeInEvent()
    {
        if (AfterSceneLoadFadeInEvent != null)
        {
            AfterSceneLoadFadeInEvent();
        }
    }
}