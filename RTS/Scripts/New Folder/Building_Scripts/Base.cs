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
        // 在这里实现将采集到的资源存储到基地的逻辑
        // 可以使用储存容器或其他方式来处理资源的存储
        // 然后将资源添加到 ResourceManager 中
        ResourceManager.instance.AddResource(resourceType, amount);
    }
}
