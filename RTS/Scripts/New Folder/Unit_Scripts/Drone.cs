using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : UnitBehavior
{
    private GameObject currentCrystal;

    public ResourceType resourceTypeToCollect;
    public float collectionRange = 2f; // 采集范围
    public float collectionRate = 3f; // 单次采集所需时间

    private float collectionTimer = 0f;// 已过采集时间
    private int collectedResources = 0; // 已经采集的资源数量

    private bool isCollecting = false;

    float distance;

    private Base nearestBaseBuilding;

    void Start()
    {
        
    }

    void Update()
    {
        Collection();
        // 采集资源逻辑
        if (isCollecting && currentCrystal != null)
        {
            // 如果已经采集满了，返回基地
            if (collectedResources >= 10)
            {
                ReturnToBase();
                //return;
            }

            distance = Vector3.Distance(transform.position, currentCrystal.transform.position);
            if (distance <= collectionRange)
            {
                collectionTimer += Time.deltaTime;
                if (collectionTimer >= collectionRate)
                {
                    if (CollectCrystal())
                    {
                        FinishCollecting();
                    }
                }
            }
            else
            {
                // 移动到资源位置
                agent.SetDestination(currentCrystal.transform.position);
                //animator.SetBool("IsCollecting", true);
            }
        }
    }
    public void Collection()
    {
        if (IsSelected())
        {
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.CompareTag("Crystal"))
                    {
                        // 只有当右键点击水晶对象时才进行采集
                        currentCrystal = hit.collider.gameObject;
                        StartCollecting();
                        //agent.SetDestination(currentCrystal.transform.position);
                    }
                    else 
                    {
                        StopCollecting();
                    }
                }    
            }
        }
    }
    public bool IsSelected()
    {
        bool unitBehaviorEnabled = this.gameObject.GetComponent<UnitBehavior>().enabled;
        if (unitBehaviorEnabled && this.gameObject.GetComponent<UnitBehavior>().IsUnit()) 
        {
            return true;
        }
        return false;
    }
    private bool CollectCrystal()
    {
        if (currentCrystal == null)
        {
            return false;
        }

        Crystal crystalComponent = currentCrystal.GetComponent<Crystal>();
        if (crystalComponent != null && crystalComponent.CollectResource())
        {
            // 成功采集水晶资源
            return true;
        }

        return false;
    }
    private void StartCollecting()
    {
        // 开始采集，初始化计时器和采集状态
        collectionTimer = 0f;
        isCollecting = true;
    }

    private void FinishCollecting()
    {
        collectedResources += 10;
        collectionTimer = 0f;
    }

    private void StopCollecting()
    {
        // 停止采集，清空采集状态
        currentCrystal = null;
        isCollecting = false;
        collectionTimer = 0f;
    }
    private void ReturnToBase()
    {
        // 在这里添加返回基地的逻辑
        // 可以将采集到的资源传递给最近的基地
        // 然后将资源添加到 ResourceManager 中

        // 寻找最近的基地
        FindNearestBaseBuilding();
        if (nearestBaseBuilding != null)
        {
            // 在这里将资源传递给最近的基地
            nearestBaseBuilding.StoreResources(ResourceType.Crystal, collectedResources); // 假设传递10个水晶资源
        }
        collectedResources = 0;
    }
    private void FindNearestBaseBuilding()
    {
        // 在这里实现寻找最近的基地的逻辑
        // 可以使用 Vector3.Distance 等方法来计算距离并找到最近的基地
    }
}
