using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(NPCPath))]
public class NPCSchedule : MonoBehaviour
{
    [SerializeField] private SO_NPCScheduleEventList so_NPCScheduleEventList = null;
    private SortedSet<NPCScheduleEvent> npcScheduleEventSet;//����ļ��ϣ��Զ�ȥ��
    private NPCPath npcPath;

    private void Awake()
    {
        //ָ���Ƚ���NPCScheduleEventSort����ʼ��Set
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
                //�ҵ�ƥ��ĵ����¼�������ѭ��
                matchingNPCScheduleEvent = npcScheduleEvent;
                break;
            }
            else if(npcScheduleEvent.Time > time)
            {
                //����ѭ����������ĵ����¼���δ����
                break;
            }
        }
        //�����ǰ�����¼����ǿյģ���npcPath�������
        if(matchingNPCScheduleEvent != null)
        {
            npcPath.BuildPath(matchingNPCScheduleEvent);
        }
    }
}
