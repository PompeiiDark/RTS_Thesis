using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StoreResources(ResourceType resourceType, int amount)
    {
        // ������ʵ�ֽ��ɼ�������Դ�洢�����ص��߼�
        // ����ʹ�ô���������������ʽ��������Դ�Ĵ洢
        // Ȼ����Դ��ӵ� ResourceManager ��
        ResourceManager.instance.AddResource(resourceType, amount);
    }
}
