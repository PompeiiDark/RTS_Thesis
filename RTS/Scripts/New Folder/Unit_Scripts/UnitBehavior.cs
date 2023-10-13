using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class UnitBehavior : MonoBehaviour
{
    protected NavMeshAgent agent;

    //public GameObject player;
    [SerializeField]
    LayerMask ground;
    [SerializeField]
    LayerMask enemy;

    public int Hp = 50;
    public int AttackPower = 5;
    public int Armor = 1;

    [SerializeField]
    int currentHp;

    public float AttackRange = 2;
    public float AttackRate = 1;
    private float AttackTimer = 0f;

    float distance;

    public Transform cameraPos;

    bool IsAttacking = false;
    //new SphereCollider collider;
    [SerializeField]
    GameObject selectedEnemy;
    // Start is called before the first frame update
    void Start()
    {
        //if build from player building
        if (IsUnit())
        {
            GameControllor.instance.existingunit.Add(gameObject);
            enemy = LayerMask.GetMask("Enemy");
        }
        else
        {
            GameControllor.instance.enemysObject.Add(gameObject);
            enemy = LayerMask.GetMask("Unit");
        }
        ground = LayerMask.GetMask("Ground");
        agent = GetComponent<NavMeshAgent>();
        //collider= GetComponent<SphereCollider>();
        //player=GameObject.Find("PlayerObject");
        currentHp = Hp;
        //Debug.Log(this.gameObject.layer);
    }
    private void OnDestroy()
    {
        GameControllor.instance.existingunit.Remove(gameObject);
    }
    private void OnMouseDown()
    {
        GameControllor.instance.SetCameraPos(cameraPos);
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        AttackTargetSelected();
        if (IsAttacking == true && selectedEnemy != null)//must directly inside Update for loop
        {
            //calculate the distance between selcected enemy and current unit every frame
            distance = Vector3.Distance(transform.position, selectedEnemy.transform.position);
            if (distance > AttackRange)
            {
                //move to the target
                agent.SetDestination(selectedEnemy.transform.position);
                AttackTimer = 0f;
                Debug.Log("Unit move to enemy");
            }
            else
            {
                //stop move
                agent.SetDestination(gameObject.transform.position);
                if (AttackTimer < AttackRate)
                {
                    AttackTimer += Time.deltaTime;
                    if (AttackTimer == 1)
                    { Debug.Log("time running:" + AttackTimer); }
                }
                else
                {
                    selectedEnemy.GetComponent<UnitBehavior>().GetDamage(AttackPower);
                    AttackTimer = 0f;
                }
            }
        }
    }
    void Movement()
    {
        if (Input.GetMouseButtonDown(1) && IsUnit())
        {
            DeselectEnemy();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                DeselectEnemy();
                //Movement
                agent.SetDestination(hit.point);
                //stop if multiple units at(around) same target position
            }
        }
        else if (Input.GetKey(KeyCode.S) && IsUnit())
        {
            DeselectEnemy();
            agent.SetDestination(gameObject.transform.position);
        }
    }
    void AttackTargetSelected()
    {
        if (Input.GetMouseButtonDown(1) && IsUnit())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, enemy))
            {
                //Attack
                if (!Equals(selectedEnemy, hit.collider.gameObject))
                {
                    OnRightClickAttack(hit.collider.gameObject);
                }
                IsAttacking = true;//Will only change to true at here!
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

    public void GetDamage(int damage)
    {
        if (currentHp <= 0)
        {
            return;
        }
        currentHp -=  (damage / Armor);
        Debug.Log("The current HP is: " + this.currentHp);
        if (currentHp <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        //died effection

        //Destory
        Destroy(gameObject);
    }

    public bool IsUnit()
    {
        if (this.gameObject.layer.Equals(6))
        {
            return true;
        }
        return false;

    }
}
