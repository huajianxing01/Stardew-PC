using UnityEngine;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    protected override void Awake()
    {
        base.Awake();
        //切换屏幕分辨率为1920x1080的全屏窗口模式，RefreshRate默认值0为显示器最高刷新率
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
    }
}
