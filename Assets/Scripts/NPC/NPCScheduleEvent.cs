using UnityEngine;

[System.Serializable]
public class NPCScheduleEvent
{
    public int minute;
    public int hour;
    public int day;
    public Weather weather;
    public Season season;
    public int priority;//优先级

    public SceneName toSceneName;
    public GridCoordinate toGridCoordinate;
    public Direction npcFacingDirectionAtDestination = Direction.none;
    public AnimationClip animationAtDestination;//抵达目的地时的动画

    public int Time { get => hour * 100 + minute; }//这里时间做了换算，要比较的话需要做相同换算

    public NPCScheduleEvent()
    {
        //空的，需要实例化后填充
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

    //Debug时候显示
    public override string ToString()
    {
        return $"Time: {Time}, Priority: {priority}, Day: {day}, Weather: {weather}, Season: {season}.";
    }
}
