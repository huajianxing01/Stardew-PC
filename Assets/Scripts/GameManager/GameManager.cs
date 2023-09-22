using UnityEngine;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    public Weather currentWeather;

    protected override void Awake()
    {
        base.Awake();
        //�л���Ļ�ֱ���Ϊ1920x1080��ȫ������ģʽ��RefreshRateĬ��ֵ0Ϊ��ʾ�����ˢ����
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
    }
}
