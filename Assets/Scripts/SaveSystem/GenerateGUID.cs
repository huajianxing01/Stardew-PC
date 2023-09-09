using UnityEngine;

//ʹ�ű���ʵ���ڲ���ģʽ�ڼ��Լ��༭ʱʼ��ִ��
[ExecuteAlways]
public class GenerateGUID : MonoBehaviour
{
    [SerializeField] private string _gUID = string.Empty;

    public string GUID { get => _gUID; set => _gUID = value; }

    private void Awake()
    {
        //�жϽű��Լ�����Ϸ�����Ƿ�����Ϸ�����һ���֣�����ֻ�����ڱ༭��ģʽ
        //unity�б༭������������ģʽ���༭��ģʽ������ģʽ��Ԥ����ģʽ
        if (!Application.IsPlaying(gameObject))
        {
            if(_gUID == string.Empty)
            {
                //ϵͳ��gUID����һ�������Ψһ��16λ�ַ���
                _gUID = System.Guid.NewGuid().ToString();
            }
        }
    }
}
