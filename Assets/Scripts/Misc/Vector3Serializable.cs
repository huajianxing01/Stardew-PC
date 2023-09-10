[System.Serializable]
public class Vector3Serializable
{
    //unity中vector3和quternion不可序列化，需要自定义类转化
    //序列化是指把GameObeject数据对象转化成二进制流的过程，以便持久化或存储到本地文件
    //反序列化则是从本地文件读取二进制流转化为数据对象GameObeject
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
