[System.Serializable]
public class Vector3Serializable
{
    //unity��vector3��quternion�������л�����Ҫ�Զ�����ת��
    //���л���ָ��GameObeject���ݶ���ת���ɶ��������Ĺ��̣��Ա�־û���洢�������ļ�
    //�����л����Ǵӱ����ļ���ȡ��������ת��Ϊ���ݶ���GameObeject
    public float x, y, z;

    public Vector3Serializable(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3Serializable()
    {
    }
}
