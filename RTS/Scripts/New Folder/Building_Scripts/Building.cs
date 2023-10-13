using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Building : MonoBehaviour
{
    public string buildingName = "Building";
    public int crystalCost = 100;
    public int goldCost = 200;
    public float buildTime = 10.0f;
    public GameObject buildingPrefab; // 这是建筑建造完成后的游戏对象

    public bool isBuilding = false;//whether it can be placed

    private int _collisionHit = 0;//Count how many objects are currently touching the placeholder
    private Renderer _placeholderChecker;

    private float buildProgress = 0.0f;

    private void FixedUpdate()
    {
        if (_collisionHit > 0)
        {
            isBuilding = false;
            _placeholderChecker.material.SetColor("_Color", Color.red);
        }
        else
        {
            isBuilding = true;
            _placeholderChecker.material.SetColor("_Color", Color.green);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Platform") _collisionHit++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Platform") _collisionHit--;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // 开始建造建筑
    public void StartBuilding()
    {
        if (CanAfford() && !isBuilding)
        {
            isBuilding = true;
            // 扣除资源
            DeductResources();
        }
    }

    // 判断是否有足够的资源来建造建筑
    private bool CanAfford()
    {
        // 根据游戏中的资源管理系统来判断
        // 这里假设有名为ResourceManager的管理器
        int currentWood = ResourceManager.instance.GetCurrentWood();
        int currentGold = ResourceManager.instance.GetCurrentGold();
        if (currentGold >= goldCost && currentWood >= crystalCost)
        {
            return true;
        } 
        return false;
    }

    // 扣除资源
    private void DeductResources()
    {
        // 根据游戏中的资源管理系统来扣除资源
        ResourceManager.instance.SpendResources(goldCost, crystalCost);
    }

    // 建筑建造完成
    private void CompleteBuilding()
    {
        isBuilding = false;
        buildProgress = 0.0f;

        // 实例化建筑预制件
        Instantiate(buildingPrefab, transform.position, Quaternion.identity);
        // 销毁正在建造的建筑
        Destroy(gameObject);
    }
}
