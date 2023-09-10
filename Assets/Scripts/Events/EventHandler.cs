using System;
using System.Collections.Generic;

//����һ��Movementί������
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

    //����һ��Actionί���¼���һ�����λ�ò�����һ������б����
    public static event Action<InventoryLocation, List<InventoryItem>> InventoryUpdatedEvent;
    //Publishers����event�ķ���
    public static void CallInventoryUpdatedEvent(InventoryLocation inventoryLocation, List<InventoryItem> inventoryItemsList)
    {
        if (InventoryUpdatedEvent != null)
        {
            InventoryUpdatedEvent(inventoryLocation, inventoryItemsList);
        }
    }

    //����һ��Actionί���¼�������Time Event-Minute
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

    //����һ��Actionί���¼�������Time Event-Hour
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

    //����һ��Actionί���¼�������Time Event-Day
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

    //����һ��Actionί���¼�������Time Event-Season
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

    //����һ��Actionί���¼�������Time Event-Year
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

    //����ί������������Movement�¼�������һ��ί���ֶΣ�public���ܱ�����class������ע���¼��������
    //�¼�����/�ṹ�е�һ����Ա��������������/�ṹ�У��Զ���ʽ��ʼ��Ϊnull��ֻ����+=��-=���ֲ���
    //�¼���ʽ�������ڱ���ʱ�򣬻�����һ�����¼�����ͬ��˽��ί�У������¼�Ҳ�Ƕ�ͬ��˽��ί�н��е���
    public static event MovementDelegate MovementEvent;
    //Publishers����event�ķ���
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
            //����Movementί���¼����͵��÷���һ�����¼���(�����б�)
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
    //ǰһ�����˳�-����Ч���¼�
    public static void CallBeforeSceneUnloadFadeOutEvent()
    {
        if( BeforeSceneUnloadFadeOutEvent != null )
        {
            BeforeSceneUnloadFadeOutEvent();
        }
    }

    public static event Action BeforeSceneUnloadEvent;
    //ǰһ�����˳��¼�
    public static void CallBeforeSceneUnloadEvent()
    {
        if (BeforeSceneUnloadEvent != null)
        {
            BeforeSceneUnloadEvent();
        }
    }

    public static event Action AfterSceneLoadEvent;
    //��һ���������¼�
    public static void CallAfterSceneLoadEvent()
    {
        if (AfterSceneLoadEvent != null)
        {
            AfterSceneLoadEvent();
        }
    }

    public static event Action AfterSceneLoadFadeInEvent;
    //��һ��������-����Ч���¼�
    public static void CallAfterSceneLoadFadeInEvent()
    {
        if (AfterSceneLoadFadeInEvent != null)
        {
            AfterSceneLoadFadeInEvent();
        }
    }
}