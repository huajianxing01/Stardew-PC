using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(NPCPath))]
public class NPCSchedule : MonoBehaviour
{
    [SerializeField] private SO_NPCScheduleEventList so_NPCScheduleEventList = null;
    private SortedSet<NPCScheduleEvent> npcScheduleEventSet;//排序的集合，自动去重
    private NPCPath npcPath;

    private void Awake()
    {
        //指定比较器NPCScheduleEventSort并初始化Set
        npcScheduleEventSet = new SortedSet<NPCScheduleEvent>(new NPCScheduleEventSort());
        foreach(var npcScheduleEvent in so_NPCScheduleEventList.npcScheduleEventList)
        {
            npcScheduleEventSet.Add(npcScheduleEvent);
        }

        npcPath = GetComponent<NPCPath>();
    }

    private void OnEnable()
    {
        EventHandler.AdvanceGameMinuteEvent += GameTimeSystem_AdvanceMinute;
    }

    private void OnDisable()
    {
        EventHandler.AdvanceGameMinuteEvent -= GameTimeSystem_AdvanceMinute;
    }

    private void GameTimeSystem_AdvanceMinute(int year, Season season, int day, Week week, int hour, int minute, int second)
    {
        int time = hour * 100 + minute;
        NPCScheduleEvent matchingNPCScheduleEvent = null;
        
        foreach(var npcScheduleEvent in npcScheduleEventSet)
        {
            if(npcScheduleEvent.Time == time)
            {
                if (npcScheduleEvent.day != 0 && npcScheduleEvent.day != day)
                    continue;
                if (npcScheduleEvent.season != Season.none && npcScheduleEvent.season != season)
                    continue;
                if (npcScheduleEvent.weather != Weather.none && npcScheduleEvent.weather != GameManager.Instance.currentWeather)
                    continue;
                //找到匹配的调度事件，跳出循环
                matchingNPCScheduleEvent = npcScheduleEvent;
                break;
            }
            else if(npcScheduleEvent.Time > time)
            {
                //跳出循环，再往后的调度事件是未来的
                break;
            }
        }
        //如果当前调度事件不是空的，向npcPath发起调用
        if(matchingNPCScheduleEvent != null)
        {
            npcPath.BuildPath(matchingNPCScheduleEvent);
        }
    }
}
