using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Drone : UnitBehavior
{
    public GameObject currentResource;

    public ResourceType resourceTypeToCollect;

    public float ToBuildRange = 5;
    public float ToBuildingRange = 7;
    public float collectionRange = 2; // 采集范围
    public float collectionRate = 3; // 单次采集所需时间

    [SerializeField]
    private float collectionTimer = 0f;// 已过采集时间
    [SerializeField]
    private int collectedResources = 0; // 已经采集的资源数量
    public int collectedNumEach = 0;

    public bool isCollecting = false;

    float dis;
    float disToBase;
    float disToBuild;

    public GameObject place;
    public bool Canbuild = false;
    public bool IsBuilding = false;

    public float buildProgress = 0f;
    public float buildTime = 0f;
    public Vector3 buildPos = Vector3.zero;

    public bool hasBuildClickRun = false;


    public GameObject nearestBaseBuilding;
    void Start()
    {
        
    }

    void Update()
    {
        GetAgent();
        Collection();
        if (Input.GetKey(KeyCode.B) && IsSelected() && !hasBuildClickRun)
        {
            OnBaseBuildClick();
            hasBuildClickRun =true;
        }
        if (Input.GetKey(KeyCode.C) && IsSelected() && !hasBuildClickRun)
        {
            OnMCBuildClick();
            hasBuildClickRun = true;
        }
        // 采集资源逻辑
        if (isCollecting && currentResource != null)
        {
            // 如果已经采集满了，返回基地
            if (collectedResources >= 10)
            {
                if (nearestBaseBuilding == null && gameObject.GetComponent<UnitBehavior>().IsUnit())//only person
                {
                    FindNearestBaseBuilding();
                }
                if (nearestBaseBuilding != null)
                {
                    disToBase = Vector3.Distance(transform.position, nearestBaseBuilding.transform.position);
                    if (disToBase > ToBuildingRange && agent != null && nearestBaseBuilding.transform.position != null)
                    {
                        agent.SetDestination(nearestBaseBuilding.transform.position);
                    }
                    else
                    {
                        if (agent != null && gameObject.transform.position != null)
                        {
                            agent.SetDestination(gameObject.transform.position);
                            if (currentResource.CompareTag("Crystal"))
                            { 
                                nearestBaseBuilding.GetComponent<Base>().StoreResources(ResourceType.Crystal, collectedResources); 
                            }
                            else if(currentResource.CompareTag("Gold"))
                            { 
                                nearestBaseBuilding.GetComponent<Base>().StoreResources(ResourceType.Gold, collectedResources); 
                            }
                            collectedResources = 0;
                            collectionTimer = 0f;
                        }
                    }
                }
            }
            else
            {
                dis = Vector3.Distance(transform.position, currentResource.transform.position);
                if (dis > collectionRange)
                {
                    // 移动到资源位置
                    if (agent != null && currentResource.transform.position != null)
                    {
                        agent.SetDestination(currentResource.transform.position);
                    }
                }
                else
                {
                    if (agent != null && gameObject.transform.position != null)
                    {
                        agent.SetDestination(gameObject.transform.position);
                    }
                    collectionTimer += Time.deltaTime;
                    if (collectionTimer >= collectionRate)
                    {
                        if (CollectCrystal() || CollectGold())
                        {
                            FinishCollecting();
                        }
                    }
                }
            }
        }
        //建造建筑逻辑
        if (Canbuild && IsBuilding)//at here,left mouse button is already clicked and the postion of placeholder will not change,all parameters which build needed are localed
        {
            if (gameObject.GetComponent<UnitBehavior>().IsUnit())
            { 
                getBuildInfo(); 
            }
            if (gameObject.GetComponent<UnitBehavior>().IsUnit() && (Input.GetMouseButtonDown(1)|| Input.GetKey(KeyCode.S)))//May has Problem,//Need check the selected unit
            {
                if (GameControllor.instance.currenSelection_Unit.Count == 1)
                {
                    foreach (GameObject unit in GameControllor.instance.currenSelection_Unit)
                    {
                        if (unit == this.gameObject)//should not change BuildingManager,change itself
                        {
                            SetBuildOff();
                            Destroy(place);
                            return;
                        }
                    }
                }
            }
            disToBuild = Vector3.Distance(gameObject.transform.position, buildPos);//BuildingManager.instance._placeholder.transform.position
            if (disToBuild > ToBuildRange)
            {
                agent.SetDestination(buildPos);//BuildingManager.instance._placeholder.transform.position
            }
            else
            {
                agent.SetDestination(gameObject.transform.position);
                buildProgress += Time.deltaTime;
                if (buildProgress >= buildTime)
                {
                    if (place.name == "Base_Placeholder(Clone)" || place.name == "Base_Placeholder")
                    {
                        GameObject building = Instantiate(BuildingManager.instance.BaseBuilding, buildPos, Quaternion.identity);
                        GameObject effect = Instantiate(building.GetComponent<Building>().BuildedEffect, building.transform.position, building.transform.rotation);
                        Destroy(effect, 1.5f);
                        building.GetComponent<Building>().owner = gameObject.GetComponent<UnitBehavior>().owner;
                        if (gameObject.GetComponent<UnitBehavior>().IsUnit())
                        {
                            BuildingManager.instance.BaseBuilding.GetComponent<Building>().DeductResources();
                        }
                    }
                    else if (place.name == "MC_Placeholder(Clone)" || place.name == "MC_Placeholder")
                    {
                        GameObject building = Instantiate(BuildingManager.instance.SecondBuilding, buildPos, Quaternion.identity);
                        GameObject effect = Instantiate(building.GetComponent<Building>().BuildedEffect, building.transform.position, building.transform.rotation);
                        Destroy(effect, 1.5f);
                        building.GetComponent<Building>().owner = gameObject.GetComponent<UnitBehavior>().owner;
                        if (gameObject.GetComponent<UnitBehavior>().IsUnit())
                        { 
                            BuildingManager.instance.SecondBuilding.GetComponent<Building>().DeductResources();
                        }
                    }
                    if (gameObject.GetComponent<UnitBehavior>().IsUnit())
                    { 
                        Destroy(place); 
                    }
                    SetBuildOff();
                }
            }
        }
    }
    public void Collection()
    {
        if (IsSelected())
        {
            if (Input.GetMouseButtonDown(1) || Input.GetKey(KeyCode.M))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.CompareTag("Crystal"))
                    {
                        currentResource = hit.collider.gameObject;
                        StartCollecting();
                    }
                    else if (hit.collider.CompareTag("Gold"))
                    {
                        currentResource = hit.collider.gameObject;
                        StartCollecting();
                    }
                    else
                    {
                        StopCollecting();
                    }
                }
            }
        }
    }
    void GetAgent()
    {
        if (gameObject.GetComponent<UnitBehavior>().agent != null && agent != gameObject.GetComponent<UnitBehavior>().agent)
        {
            agent = gameObject.GetComponent<UnitBehavior>().agent;
        }
    }
    public bool IsSelected()
    {
        
        if (gameObject.GetComponent<UnitBehavior>().IsUnit() && gameObject.transform.GetChild(0).gameObject.activeSelf)
        {
            return true;
        }
        return false;
    }
    private bool CollectCrystal()
    {
        if (currentResource == null)
        {
            return false;
        }

        Crystal crystalComponent = currentResource.GetComponent<Crystal>();
        if (crystalComponent != null && crystalComponent.CollectResource())
        {
            collectedNumEach = crystalComponent.collectionValue;
            // success collection
            return true;
        }

        return false;
    }
    private bool CollectGold()
    {
        if (currentResource == null)
        {
            return false;
        }

        Gold goldComponent = currentResource.GetComponent<Gold>();
        if (goldComponent != null && goldComponent.CollectResource())
        {
            collectedNumEach = goldComponent.collectionValue;
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
        collectedResources += collectedNumEach;
    }

    private void StopCollecting()
    {
        // 停止采集，清空采集状态
        currentResource = null;
        isCollecting = false;
        collectionTimer = 0f;
    }
    private void FindNearestBaseBuilding()
    {
        float nearestDistance=100000;
        if (BuildingManager.instance.buildings != null)
        {
            bool Isbase;
            foreach (var building in BuildingManager.instance.buildings)
            {
                if (Isbase = building.GetComponent<Base>())
                {
                    float dis = Vector3.Distance(gameObject.transform.position, building.transform.position);
                    if (dis < nearestDistance)
                    {
                        nearestDistance = dis;
                        nearestBaseBuilding = building;
                    }
                }
            }
        }
    }

    void getBuildInfo()
    {
        if (BuildingManager.instance._placeholder != null && BuildingManager.instance.building != null && buildPos == Vector3.zero && buildTime == 0)//only work when all is initial
        {
            buildPos = BuildingManager.instance._placeholder.transform.position;
            buildTime = BuildingManager.instance.building.buildTime;
            buildProgress = 0f;
            BuildingManager.instance.SetBuildOff();
        }
    }
    void SetBuildOff()
    {
        buildProgress = 0;
        buildPos = Vector3.zero;
        buildTime = 0;
        IsBuilding = false;
        Canbuild = false;
    }

    public void OnBaseBuildClick()
    {
        place = Instantiate(BuildingManager.instance.BaseBuildingPlaceholder);

        BuildingManager.instance._placeholder = place;
        BuildingManager.instance.SetPlaceholder();
        IsBuilding = true;

    }
    public void OnMCBuildClick()
    {
        place = Instantiate(BuildingManager.instance.SecondBuildingPlaceholder);

        BuildingManager.instance._placeholder = place;
        BuildingManager.instance.SetPlaceholder();
        IsBuilding = true;

    }
    
}
