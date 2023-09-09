using UnityEngine;
using Cinemachine;

public class SwitchConfinerBoundingShape : MonoBehaviour
{
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SwitchBoundingShape;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SwitchBoundingShape;
    }

    //定义场景边缘，让虚拟相机视角不跳出屏幕边界
    private void SwitchBoundingShape()
    {
        //获得在不同scene里定义的多边形碰撞器
        PolygonCollider2D polygonCollider2D = GameObject.FindGameObjectWithTag(Tags.BoundsConfiner).GetComponent<PolygonCollider2D>();
        CinemachineConfiner cinemachineConfiner = GetComponent<CinemachineConfiner>();
        cinemachineConfiner.m_BoundingShape2D = polygonCollider2D;
        //边界形状改变时，调用该方法清除边界路径缓存，重新加载
        cinemachineConfiner.InvalidatePathCache();
    }
}
