using TMPro;
using UnityEngine;

public class GameClock : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText = null;
    [SerializeField] private TextMeshProUGUI dayText = null;
    [SerializeField] private TextMeshProUGUI seasonText = null;
    [SerializeField] private TextMeshProUGUI yearText = null;

    private void OnEnable()
    {
        EventHandler.AdvanceGameMinuteEvent += UpdateGameTime;
    }

    private void OnDisable()
    {
        EventHandler.AdvanceGameMinuteEvent -= UpdateGameTime;
    }

    private void UpdateGameTime(int gameYear, Season gameSeason, int gameDay,
        Week gameDayOfWeek, int gamehour, int gameMinute, int gameSecond)
    {
        //获得10的倍数的分钟
        gameMinute = gameMinute - (gameMinute % 10);

        string ampm = (gamehour >= 12) ? "PM" : "AM";
        if (gamehour >= 13)
        {
            gamehour -= 12;
        }

        string minute = (gameMinute < 10) ? ("0" + gameMinute.ToString()) : gameMinute.ToString();
        string time = gamehour.ToString() + ":" + minute + " " + ampm;

        timeText.SetText(time);
        dayText.SetText(TimeManager.Instance.GetWeekChineseWord(gameDayOfWeek) + "." + gameDay.ToString());
        seasonText.SetText(TimeManager.Instance.GetSeasonChineseWord(gameSeason));
        yearText.SetText("第" + gameYear + "年");
    }
}
