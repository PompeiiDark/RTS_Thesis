using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class Ai_Behavior : MonoBehaviour
{
    public GameObject BaseBuildingPlaceholder;
    public GameObject SecondBuildingPlaceholder;
    public GameObject BaseBuilding;
    public GameObject SecondBuilding;

    public List<GameObject> Units = new List<GameObject>();
    public List<GameObject> Buildings = new List<GameObject>();
    public List<GameObject> _Resources = new List<GameObject>();

    public int warningRange = 30;//警戒范围
    public int miningRange = 30;//最远采矿范围 -> 建造新基地
    public float buildRange = 35.0f;

    public int StartingGold = 1000;
    public int StartingCrystal = 1000;
    public int currentGold;
    public int currentCrystal;

    public int diffLevel = 5;
    [SerializeField]
    bool IsSetDiff = false;

    int maxAttempts = 10;  // 最大尝试次数，以避免无限循环
    int attempts = 0;

    int McNum = 0;
    int MaxMcNum = 5;

    Vector3 mapMin = new Vector3(-95f, 0f, -75f); // 最小坐标
    Vector3 mapMax = new Vector3(55f, 0f, 75f); // 最大坐标
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("start lvl: " + diffLevel);
        currentCrystal = StartingCrystal;
        currentGold = StartingGold;

        // 查找并存储所有Resource
        GameObject[] crystals = GameObject.FindGameObjectsWithTag("Crystal");
        _Resources.AddRange(crystals);
        GameObject[] gold = GameObject.FindGameObjectsWithTag("Gold");
        _Resources.AddRange(gold);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsSetDiff && gameObject.GetComponent<Players>().IsEnemy(GameObject.Find("PlayerObject").GetComponent<Players>()))
        {
            Debug.Log("in !IsSetDiff,lvl: " + diffLevel);
            if (diffLevel != 5)
            {
                SetDiff(diffLevel);
            }
        }
        foreach (var unit in Units)
        {
            if (unit != null)//Mining,building,running
            {
                Drone drone = unit.GetComponent<Drone>();
                if (drone != null)
                {
                    CheckEnemyAround_Drone(unit);
                    BuildMc(drone);
                    FindNearestResourseAndMining(drone);
                    BuildBase(drone);
                }
            }
            else if (unit == null)
            {
                Units = Units.FindAll(x => x != null);
            }
        }
    }
    private void FindNearestResourseAndMining(Drone unit)//drone unit
    {
        float nearestDistance_R = 100000;
        float nearestDistance_B = 100000;
        Drone drone = unit.GetComponent<Drone>();
        if (_Resources.Count > 0 && drone.currentResource == null && drone.IsBuilding == false)
        {
            foreach (var resource in _Resources)
            {
                if (resource != null)
                {
                    float dis = Vector3.Distance(drone.gameObject.transform.position, resource.transform.position);//find nearest resource
                    if (dis < nearestDistance_R)
                    {
                        nearestDistance_R = dis;
                        drone.currentResource = resource;
                    }
                }
                else if (resource == null)
                {
                    _Resources = _Resources.FindAll(x => x != null);
                }
            }
        }
        if (drone.currentResource != null && drone.IsBuilding == false)
        {
            foreach (var building in Buildings)
            {
                if (building != null)
                {
                    bool Isbase = false;
                    if (Isbase = building.GetComponent<Base>())
                    {
                        float dis = Vector3.Distance(drone.gameObject.transform.position, building.transform.position);//find nearest base
                        if (dis < nearestDistance_B)
                        {
                            nearestDistance_B = dis;
                            drone.nearestBaseBuilding = building;
                        }
                    }
                }
                else if (building == null)
                {
                    Buildings = Buildings.FindAll(x => x != null);
                }
            }
            drone.isCollecting = true;
        }
    }
    public void AddResource(ResourceType type, int amount)
    {
        if (type == ResourceType.Crystal)
        {
            currentCrystal += amount;
        }
        else if (type == ResourceType.Gold)
        {
            currentGold += amount;
        }
    }


    public void CheckEnemyAround_Drone(GameObject unit)
    {
        int num = 0;
        Collider[] colliders = Physics.OverlapSphere(unit.gameObject.transform.position,unit.gameObject.GetComponent<UnitBehavior>().FindRange, unit.gameObject.GetComponent<UnitBehavior>().unit);
        List<Collider> enemyColliders = new List<Collider>();
        if (colliders.Length > 0)//have enemy around,call help and run away
        {
            foreach (var c in colliders)
            {
                if (c != null && unit.GetComponent<UnitBehavior>().owner.IsEnemy(c.gameObject.GetComponent<UnitBehavior>().owner))
                {
                    num++;
                    enemyColliders.Add(c);
                }
            }
            if (enemyColliders.Count > 0)
            {
                Collider[] cl = enemyColliders.ToArray();
                Vector3 pos = unit.transform.position;
                CallHelp(pos, num);
                if (unit.GetComponent<Drone>())
                { 
                    RunAway(cl, unit.GetComponent<Drone>()); 
                }
            }
        }
    }
    public void CallHelp(Vector3 pos ,int enemyNum)
    {
        List<GameObject> combatUnits = new List<GameObject>();
        foreach (var unit in Units)
        {
            if (unit != null && !unit.GetComponent<Drone>())//fight units
            {
                combatUnits.Add(unit);
            }
            else if (unit == null)
            {
                Units = Units.FindAll(x => x != null);
            }
        }
        // 计算每个战斗单位到目标位置的距离并且根据单位到目标点的距离排序
        combatUnits.Sort((a, b) => Vector3.Distance(a.transform.position, pos).CompareTo(Vector3.Distance(b.transform.position, pos)));
        for (int i = 0; i < Mathf.Min(combatUnits.Count, enemyNum + 1); i++)//all of the most closest fight units would go to help but may fight on the way
        {
            GameObject unit = combatUnits[i]; 
            UnitBehavior ub = unit.GetComponent<UnitBehavior>();
            if (ub != null && ub.IsAttacking != true)
            {
                ub.agent.SetDestination(pos);
                ub.IsFindingEnemy = true; 
            }
        }
    }
    public void RunAway(Collider[] colliders , Drone unit)
    {
        Vector3 escapeDirection = Vector3.zero;
        foreach (var collider in colliders)
        {
            if (collider != null)
            {
                Vector3 enemyPosition = collider.gameObject.transform.position;
                escapeDirection += (unit.gameObject.transform.position - enemyPosition).normalized;
            }
        }
        escapeDirection.Normalize();//stay away from enemies
        float escapeDistance = 25.0f; 
        Vector3 escapePosition = unit.gameObject.transform.position + escapeDirection * escapeDistance;
        if (unit != null)
        { unit.gameObject.GetComponent<UnitBehavior>().agent.SetDestination(escapePosition); }
    }



    public void BuildMc(Drone unit)
    {
        Building building = SecondBuilding.GetComponent<Building>();
        if (currentCrystal >= building.crystalCost && currentGold >= building.goldCost)//build MC
        {
            foreach (var b in Buildings)
            {
                if (b != null && b.GetComponent<MilitaryCamp>())
                {
                    McNum++;
                }
                else if (b == null)
                {
                    Buildings = Buildings.FindAll(x => x != null);
                }
            }
            if (McNum <= MaxMcNum)
            {
                Vector3 buildingPos = GetRandomVaildBuildPos(unit.transform.position, buildRange, SecondBuildingPlaceholder);
                //Debug.Log("buildingPos:" + buildingPos);
                if (buildingPos != Vector3.zero)
                {
                    currentCrystal -= building.crystalCost;
                    currentGold -= building.goldCost;
                    if (currentCrystal < 0 || currentGold < 0)//since concurrent,check resources values again
                    {
                        currentCrystal += building.crystalCost;
                        currentGold += building.goldCost;
                        return;
                    }
                    SetDroneBuildProperty(unit, buildingPos, SecondBuilding,SecondBuildingPlaceholder); 
                }
            }
        }
    }
    public void BuildBase(Drone unit)
    {
        Building building = BaseBuilding.GetComponent<Building>();
        if (currentCrystal >= building.crystalCost && currentGold >= building.goldCost)//build BS
        {
            int num = 0;
            foreach (var b in Buildings)
            {
                if (b != null && b.GetComponent<Base>())
                {
                    num++;
                }
                else if (b == null)
                {
                    Buildings = Buildings.FindAll(x => x != null);
                }
            }
            if (num < 1)
            {
                Vector3 buildingPos = GetRandomVaildBuildPos(unit.transform.position, buildRange, BaseBuildingPlaceholder);
                if (buildingPos != Vector3.zero)
                {
                    currentCrystal -= building.crystalCost;
                    currentGold -= building.goldCost;
                    if (currentCrystal < 0 || currentGold < 0)
                    {
                        currentCrystal += building.crystalCost;
                        currentGold += building.goldCost;
                        return;
                    }
                    SetDroneBuildProperty(unit, buildingPos, BaseBuilding, BaseBuildingPlaceholder);
                }
            }
        }
    }
    Vector3 GetRandomVaildBuildPos(Vector3 unitPos, float searchRadius, GameObject placeholder)
    {
        BoxCollider bc = placeholder.GetComponent<BoxCollider>();
        Debug.Log("GetRandomVaildBuildPos is working");
        if (attempts <= maxAttempts)
        {
            // 随机生成一个位置
            Vector3 randomPosition = unitPos + Random.insideUnitSphere * searchRadius;
            // 设置位置的高度与建筑位置相同，以防止建筑悬浮
            randomPosition.y = 0f;

            Collider[] collidersX = Physics.OverlapSphere(randomPosition, bc.size.x);
            Collider[] collidersZ = Physics.OverlapSphere(randomPosition, bc.size.z);
            if (collidersX.Length <= 1 && collidersZ.Length <= 1 
                && randomPosition.x - bc.size.x / 2 >= mapMin.x 
                && randomPosition.x + bc.size.x / 2 <= mapMax.x 
                && randomPosition.z - bc.size.z / 2 >= mapMin.z 
                && randomPosition.z + bc.size.z / 2 <= mapMax.z)
            {
                attempts = 0;
                return randomPosition;
            }

            attempts++;
        }
        attempts = 0;
        return Vector3.zero;
    }
    public void SetDroneBuildProperty(Drone unit,Vector3 pos,GameObject building,GameObject placeholder)
    {
        Drone drone = unit.gameObject.GetComponent<Drone>();
        if (drone.buildPos == Vector3.zero && drone.IsBuilding == false && drone.Canbuild == false)
        {
            drone.buildPos = pos;
            drone.buildTime = building.GetComponent<Building>().buildTime;
            drone.buildProgress = 0f;
            drone.place = placeholder;
            drone.isCollecting = false;
            drone.IsBuilding = true;
            drone.Canbuild = true;
        }
    }

    public void SetDiff(int lvl)
    {
        Debug.Log("lvl in: " + lvl);
        foreach (var b in Buildings)
        {
            Building bd = b.GetComponent<Building>();
            if (lvl == 0)
            {
                bd.buildTime += 5;
                bd.crystalCost += 50;
                bd.goldCost += 100;
            }
            else if (lvl == 2)
            {
                bd.buildTime -= 5;
                bd.crystalCost -= 50;
                bd.goldCost -= 100;
            }
        }
        foreach (var u in Units)
        {
            UnitBehavior ub = u.GetComponent<UnitBehavior>();
            if (lvl == 0)
            {
                ub.AttackPower -= 4;
                ub.Hp -= 30;
                ub.currentHp -= 30;
                ub.AttackRate += 0.2f;
                if (u.GetComponent<Drone>())
                {
                    u.GetComponent<Drone>().collectionRate += 1f;
                }
            }
            else if (lvl == 2)
            {
                ub.AttackPower += 5;
                ub.Hp += 50;
                ub.currentHp += 50;
                ub.AttackRate -= 0.2f;
                if (u.GetComponent<Drone>())
                {
                    u.GetComponent<Drone>().collectionRate -= 1f;
                }
            }
        }
        IsSetDiff = true;
    }

}
