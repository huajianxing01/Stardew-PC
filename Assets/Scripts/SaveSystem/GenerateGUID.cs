using UnityEngine;

//使脚本的实例在播放模式期间以及编辑时始终执行
[ExecuteAlways]
public class GenerateGUID : MonoBehaviour
{
    [SerializeField] private string _gUID = string.Empty;

    public string GUID { get => _gUID; set => _gUID = value; }

    private void Awake()
    {
        //判断脚本自己的游戏对象是否是游戏世界的一部分，这里只运行在编辑器模式
        //unity中编辑器界面有三种模式，编辑器模式、运行模式、预制体模式
        if (!Application.IsPlaying(gameObject))
        {
            if(_gUID == string.Empty)
            {
                //系统给gUID分配一个随机、唯一的16位字符串
                _gUID = System.Guid.NewGuid().ToString();
            }
        }
    }
}
