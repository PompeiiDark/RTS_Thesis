using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

public class UnitBehavior : MonoBehaviour
{
    protected internal NavMeshAgent agent;

    public GameObject FireEffect;
    public GameObject DamagedEffect;
    public Transform FirePosition;
    public Transform head;


    public Players owner;//player's palyers.Id have to be 0 always!

    //public GameObject player;
    public LayerMask ground;
    public LayerMask unit;
    public LayerMask building;

    public int Hp = 50;
    public int AttackPower = 5;
    public int Armor = 1;
    public int crystalCost = 30;
    public int goldCost = 20;
    public int currentHp;

    public float FindRange = 20;
    public float AttackRange = 2;
    public float AttackRate = 1;
    private float AttackTimer = 0f;

    public Text HpTxt;
    public Text AtkTxt;
    public Text AmrTxt;
    public Slider slider;

    float distance;

    public Transform cameraPos;

    public bool IsAttacking = false;
    public bool IsFindingEnemy = false;

    //Collider[] colliders = new Collider[10];

    [SerializeField]
    GameObject selectedEnemy;
    // Start is called before the first frame update
    void Start()
    {
        //if build from player building
        if (IsUnit())
        {
            GameControllor.instance.existingunit.Add(gameObject);
        }
        else if (owner.IsEnemy(GameObject.Find("PlayerObject").GetComponent<Players>()))
        {
            GameControllor.instance.enemysObject.Add(gameObject);
            owner.gameObject.GetComponent<Ai_Behavior>().Units.Add(gameObject);
        }
        ground = LayerMask.GetMask("Ground");
        unit = LayerMask.GetMask("Unit");
        building = LayerMask.GetMask("Build");
        if (owner.upgradeManager != null)
        {
            if (owner.upgradeManager.Attack_Time != 0)
            {
                AttackPower += (owner.upgradeManager.Attack_Time * owner.upgradeManager.AddAtkNum);
            }

            if (owner.upgradeManager.Armor_Time != 0)
            {
                Armor += (owner.upgradeManager.Armor_Time * owner.upgradeManager.AddArmNum);
            }
        }
        if (HpTxt != null && AtkTxt != null && AmrTxt != null)
        {
            HpTxt.text = currentHp.ToString();
            AtkTxt.text = AttackPower.ToString();
            AmrTxt.text = Armor.ToString();
        }
        if (owner.gameObject.GetComponent<Ai_Behavior>())
        {
            Ai_Behavior ub = owner.gameObject.GetComponent<Ai_Behavior>();
            if (ub.diffLevel == 0)
            {
                AttackPower -= 4;
                Hp -= 30;
                currentHp -= 30;
                AttackRate += 0.2f;
                if (gameObject.GetComponent<Drone>())
                {
                    gameObject.GetComponent<Drone>().collectionRate += 1f;
                }
            }
            else if (ub.diffLevel == 2)
            {
                AttackPower += 5;
                Hp += 50;
                currentHp += 50;
                AttackRate -= 0.2f;
                if (gameObject.GetComponent<Drone>())
                {
                    gameObject.GetComponent<Drone>().collectionRate -= 1f;
                }
            }
            if (owner.IsEnemy(GameObject.Find("PlayerObject").GetComponent<Players>()))
            {
                transform.GetChild(2).GetComponent<MeshRenderer>().material.color = Color.red;
            }
        }
        currentHp = Hp;
    }
    private void Awake()
    {   
        agent = GetComponent<NavMeshAgent>();
    }
    //private void OnMouseDown()
    //{
    //    GameControllor.instance.SetCameraPos(cameraPos);
    //}

    // Update is called once per frame
    void Update()
    {
        Movement();
        AttackTargetSelected();
        AttackByA_ClickGround();
        if (IsAttacking == true && selectedEnemy != null)//must directly inside Update for loop
        {
            IsFindingEnemy = false;
            //calculate the distance between selcected enemy and current unit every frame
            distance = Vector3.Distance(transform.position, selectedEnemy.transform.position);
            if (distance > AttackRange)
            {
                //move to the target
                agent.SetDestination(selectedEnemy.transform.position);
                AttackTimer = 0f;

            }
            else
            {
                //stop move
                agent.SetDestination(gameObject.transform.position);
                if (AttackTimer < AttackRate)
                {
                    AttackTimer += Time.deltaTime;
                }
                else
                {
                    if (selectedEnemy.GetComponent<UnitBehavior>())
                    {
                        SetAttackEffect();
                        selectedEnemy.GetComponent<UnitBehavior>().GetDamage(AttackPower); 
                    }
                    else if (selectedEnemy.GetComponent<Building>())
                    {
                        SetAttackEffect();
                        selectedEnemy.GetComponent<Building>().GetDamage(AttackPower); 
                    }
                    AttackTimer = 0f;
                }
            }
        }
        else if (IsAttacking == true && selectedEnemy == null)
        {
            IsAttacking = false;
            selectedEnemy = null;
            IsFindingEnemy = true;
        }

        if (IsFindingEnemy == true && IsAttacking == false)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, FindRange, unit);
            Collider[] BuildingColliders = Physics.OverlapSphere(transform.position, FindRange, building);
            if (colliders.Length > 0)
            {
                foreach (var c in colliders)
                {
                    if (owner.IsEnemy(c.gameObject.GetComponent<UnitBehavior>().owner))
                    {
                        OnRightClickAttack(c.gameObject);
                        IsAttacking = true;
                        IsFindingEnemy = false;
                        break;
                    }
                }
            }
            if (BuildingColliders.Length > 0 && IsAttacking == false)
            {
                foreach (var c in BuildingColliders)
                {
                    if (owner.IsEnemy(c.gameObject.GetComponent<Building>().owner))
                    {
                        OnRightClickAttack(c.gameObject);
                        IsAttacking = true;
                        IsFindingEnemy = false;
                        break;
                    }
                }
            }
        }
    }
    void Movement()
    {
        if (Input.GetMouseButtonDown(1) && IsUnit() && gameObject.transform.GetChild(0).gameObject.activeSelf)
        {
            DeselectEnemy();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                //Movement
                agent.SetDestination(hit.point);
                //stop if multiple units at(around) same target position
            }
        }
        else if (Input.GetKey(KeyCode.S) && IsUnit() && gameObject.transform.GetChild(0).gameObject.activeSelf)
        {
            DeselectEnemy();
            agent.SetDestination(gameObject.transform.position);
        }
    }
    void AttackTargetSelected()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetKey(KeyCode.A))//manual attack
        {
            if (IsUnit() && gameObject.transform.GetChild(0).gameObject.activeSelf)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, unit) && owner.IsEnemy(hit.collider.gameObject.GetComponent<UnitBehavior>().owner))
                {
                    //Attack
                    if (!Equals(selectedEnemy, hit.collider.gameObject))
                    {
                        OnRightClickAttack(hit.collider.gameObject);
                    }
                    IsAttacking = true;//Will only change to true at here!
                }
                else if (Physics.Raycast(ray, out hit, Mathf.Infinity, building) && owner.IsEnemy(hit.collider.gameObject.GetComponent<Building>().owner))
                {
                    if (!Equals(selectedEnemy, hit.collider.gameObject))
                    {
                        OnRightClickAttack(hit.collider.gameObject);
                    }
                    IsAttacking = true;
                }
            }
        }
    }
    void AttackByA_ClickGround()
    {
        if (Input.GetKey(KeyCode.A) && IsUnit() && gameObject.transform.GetChild(0).gameObject.activeSelf)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                agent.SetDestination(hit.point);
                IsFindingEnemy = true;
            }
        }
    }
    public void OnRightClickAttack(GameObject enemy)
    {
        DeselectEnemy();
        selectedEnemy = enemy;
    }

    public void DeselectEnemy()
    {
        selectedEnemy = null;
        IsAttacking = false;
        AttackTimer = 0f;
    }
    public void AutoAttackBack()
    {
        if (selectedEnemy == null)
        {
            IsAttacking = false;
            IsFindingEnemy = true;
        }
    }
    public void CallHelp()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position,FindRange,unit);
        if (colliders.Length > 0)
        {
            foreach (var c in colliders)
            {
                if (c != null)
                {
                    UnitBehavior ub = c.GetComponent<UnitBehavior>();
                    if (ub != null && owner.Id == ub.owner.Id && ub.IsAttacking == false && !ub.gameObject.GetComponent<Drone>())
                    {
                        ub.agent.SetDestination(transform.position);
                        ub.IsFindingEnemy = true;
                    }
                }
            }
        }
    }
    public void SetAttackEffect()
    {
        Vector3 targetPos = selectedEnemy.transform.position;
        targetPos.y = head.position.y;
        head.LookAt(targetPos);
        GameObject effect = Instantiate(FireEffect, FirePosition.position, FirePosition.rotation);
        Destroy(effect, 1.5f);
    }
    public void GetDamage(int damage)
    {
        if (currentHp <= 0)
        {
            return;
        }
        GameObject effect = Instantiate(DamagedEffect, transform.position, transform.rotation);
        Destroy(effect, 1.5f);
        currentHp -=  (damage / Armor);
        HpTxt.text = currentHp.ToString();
        slider.value = (float)currentHp / Hp;
        if (currentHp <= 0)
        {
            Die();
        }
        AutoAttackBack();
        CallHelp();
    }
    public void Die()
    {
        //died effection

        //Destory
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        if (GameControllor.instance.avatar_cameraTrans.IsChildOf(cameraPos))
        {
            GameControllor.instance.avatar_cameraTrans.SetParent(null);
        }
        GameObject effect = Instantiate(DamagedEffect, transform.position, transform.rotation);
        Destroy(effect, 1.5f);
        Destroy(gameObject);
        GameObject.Find("Manager").transform.GetChild(5).GetComponent<InGameMenu>().CheckLose();
        GameObject.Find("Manager").transform.GetChild(5).GetComponent<InGameMenu>().CheckWin();
    }

    public bool IsUnit()
    {
        if (owner.Id == 0)
        {
            return true;
        }
        return false;

    }

    public bool CanAfford()
    {
        int currentCrystal = ResourceManager.instance.GetCurrentCrystal();
        int currentGold = ResourceManager.instance.GetCurrentGold();
        if (currentGold >= goldCost && currentCrystal >= crystalCost)
        {
            return true;
        }
        return false;
    }
    public void DeductResources()
    {
        ResourceManager.instance.SpendResources(goldCost, crystalCost);
    }
}
