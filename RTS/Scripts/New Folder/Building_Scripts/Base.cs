using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Base : Building
{
    public GameObject Drone;

    [SerializeField]
    LayerMask ground;
    [SerializeField]
    public int productionQuene = 0;
    public float productionTime = 6;
    float productionProgress = 0;
    [SerializeField]
    Vector3 rallyPos;
    Vector3 InstanPos;
    float distanceFromBaseZ = 7.0f;
    float distanceFromBaseX = 5.0f;

    public int UpCrystalCost = 150;
    public int UpGoldCost = 150;

    public bool IsAtkUp = false;
    public bool IsAmrUp = false;
    public float UpTime = 15;
    float UpProgress = 0;

    public Text AtkLv;
    public Text AmrLv;

    private bool hasClick = false;
    private float timer = 0;
    private float minResponseTime = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        if (AtkLv != null && AmrLv != null && gameObject.GetComponent<Building>().owner.upgradeManager != null)
        {
            AtkLv.text = gameObject.GetComponent<Building>().owner.upgradeManager.Attack_Time.ToString();
            AmrLv.text = gameObject.GetComponent<Building>().owner.upgradeManager.Armor_Time.ToString();
        }
        Vector3 baseBuildingPosition = gameObject.transform.position;
        InstanPos = baseBuildingPosition + new Vector3(distanceFromBaseX, 0, distanceFromBaseZ);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D) && gameObject.transform.GetChild(0).gameObject.activeSelf && !hasClick)
        {
            OnProductButtonClick();
            hasClick = true;
        }
        if (Input.GetKeyDown(KeyCode.Z) && gameObject.transform.GetChild(0).gameObject.activeSelf && !hasClick)
        {
            OnAttackUpButtonClick();
            hasClick = true;
        }
        if (Input.GetKeyDown(KeyCode.X) && gameObject.transform.GetChild(0).gameObject.activeSelf && !hasClick)
        {
            OnArmorUp_ButtonClick();
            hasClick = true;
        }

        if (hasClick)
        {
            timer += Time.deltaTime;
            if (timer >= minResponseTime)
            {
                timer = 0;
                hasClick = false;
            }
        }

        if (Input.GetMouseButtonDown(1) && gameObject.transform.GetChild(0).gameObject.activeSelf)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                rallyPos = hit.point;
            }
        }
        if (productionQuene > 0)
        {
            productionProgress += Time.deltaTime;
            if (productionProgress >= productionTime)
            {
                GameObject unitObject = Instantiate(Drone, InstanPos, Quaternion.identity); 
                unitObject.GetComponent<UnitBehavior>().owner = gameObject.GetComponent<Building>().owner;//give the player.Id to the new unit //fix
                if (rallyPos != Vector3.zero)
                {
                    unitObject.GetComponent<UnitBehavior>().agent.SetDestination(rallyPos);
                }
                else
                { unitObject.GetComponent<UnitBehavior>().agent.SetDestination(unitObject.transform.position + new Vector3(1, 0, 1)); }
                productionProgress = 0;
                productionQuene -= 1;
            }
        }
        if (IsAtkUp == true)
        {
            UpProgress += Time.deltaTime;
            if (UpProgress >= UpTime)
            {
                gameObject.GetComponent<Building>().owner.upgradeManager.SetAtkUp(gameObject.GetComponent<Building>().owner);
                UpProgress = 0;
                IsAtkUp = false;
            }
        }
        if (IsAmrUp == true)
        {
            UpProgress += Time.deltaTime;
            if (UpProgress >= UpTime)
            {
                gameObject.GetComponent<Building>().owner.upgradeManager.SetAmrUp(gameObject.GetComponent<Building>().owner);
                UpProgress = 0;
                IsAmrUp = false;
            }
        }
    }
    public void StoreResources(ResourceType resourceType, int amount)
    {
        if (gameObject.GetComponent<Building>().IsUnit())
        {
            ResourceManager.instance.AddResource(resourceType, amount);
        }
        else
        {
            Players ai = gameObject.GetComponent<Building>().owner;
            if (ai != null && ai.gameObject.GetComponent<Ai_Behavior>() != null)
            {
                ai.gameObject.GetComponent<Ai_Behavior>().AddResource(resourceType, amount);
            }
            else
            {
                Debug.LogError("ai/Ai_Behavior is null!!" + (ai==null) + "/" + (ai.gameObject.GetComponent<Ai_Behavior>() == null));
            }
        }
    }
    public void OnProductButtonClick()
    {
        if (productionQuene < 6 && Drone.GetComponent<UnitBehavior>().CanAfford() && gameObject.GetComponent<Building>().IsUnit())
        {
            productionQuene += 1;
            Drone.GetComponent<UnitBehavior>().DeductResources();
        }
    }
    public void OnAttackUpButtonClick()
    {
        UpgradeManager manager = gameObject.GetComponent<Building>().owner.upgradeManager;
        if (gameObject.GetComponent<Building>().IsUnit())
        {
            int currentCrystal = ResourceManager.instance.GetCurrentCrystal();
            int currentGold = ResourceManager.instance.GetCurrentGold();
            if (currentCrystal >= UpCrystalCost && currentGold >= UpGoldCost)
            {
                if (manager.Attack_Time < manager.MaxAttack)
                {
                    IsAtkUp = true;
                    ResourceManager.instance.SpendResources(goldCost, crystalCost);
                }
            }
        }
    }
    public void OnArmorUp_ButtonClick()
    {
        UpgradeManager manager = gameObject.GetComponent<Building>().owner.upgradeManager;
        if (gameObject.GetComponent<Building>().IsUnit())
        {
            int currentCrystal = ResourceManager.instance.GetCurrentCrystal();
            int currentGold = ResourceManager.instance.GetCurrentGold();
            if (currentCrystal >= UpCrystalCost && currentGold >= UpGoldCost)
            {
                if (manager.Armor_Time < manager.MaxArmor)
                {
                    IsAmrUp = true;
                    ResourceManager.instance.SpendResources(goldCost, crystalCost);
                }
            }
        }
    }
}
