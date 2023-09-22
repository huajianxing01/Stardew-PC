using UnityEngine;

[System.Serializable]
public class NPCScheduleEvent
{
    public int minute;
    public int hour;
    public int day;
    public Weather weather;
    public Season season;
    public int priority;//���ȼ�

    public SceneName toSceneName;
    public GridCoordinate toGridCoordinate;
    public Direction npcFacingDirectionAtDestination = Direction.none;
    public AnimationClip animationAtDestination;//�ִ�Ŀ�ĵ�ʱ�Ķ���

    public int Time { get => hour * 100 + minute; }//����ʱ�����˻��㣬Ҫ�ȽϵĻ���Ҫ����ͬ����

    public NPCScheduleEvent()
    {
        //�յģ���Ҫʵ���������
    }

    public NPCScheduleEvent(int hour, int minute, int priority, int day, Weather weather,
        Season season, SceneName sceneName, GridCoordinate gridCoordinate, AnimationClip animationClip)
    {
        this.hour = hour;
        this.minute = minute;
        this.priority = priority;
        this.day = day;
        this.weather = weather;
        this.season = season;
        this.toSceneName = sceneName;
        this.toGridCoordinate = gridCoordinate;
        this.animationAtDestination = animationClip;
    }

    //Debugʱ����ʾ
    public override string ToString()
    {
        return $"Time: {Time}, Priority: {priority}, Day: {day}, Weather: {weather}, Season: {season}.";
    }
}
