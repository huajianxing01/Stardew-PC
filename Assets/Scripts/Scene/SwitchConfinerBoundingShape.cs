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

    //���峡����Ե������������ӽǲ�������Ļ�߽�
    private void SwitchBoundingShape()
    {
        //����ڲ�ͬscene�ﶨ��Ķ������ײ��
        PolygonCollider2D polygonCollider2D = GameObject.FindGameObjectWithTag(Tags.BoundsConfiner).GetComponent<PolygonCollider2D>();
        CinemachineConfiner cinemachineConfiner = GetComponent<CinemachineConfiner>();
        cinemachineConfiner.m_BoundingShape2D = polygonCollider2D;
        //�߽���״�ı�ʱ�����ø÷�������߽�·�����棬���¼���
        cinemachineConfiner.InvalidatePathCache();
    }
}
