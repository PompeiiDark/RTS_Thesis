using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilitaryCamp : Building
{
    public GameObject Tank;
    public GameObject Grav;
    [SerializeField]
    LayerMask ground;
    public int productionQuene = 0;

    public float TankproductionTime = 6;
    public float GravproductionTime = 6;

    public float productionProgress = 0;

    [SerializeField]
    public Queue<int> queue = new Queue<int>();
    public int current;

    private bool hasClick = false;
    private float timer = 0;
    private float minResponseTime = 0.3f;

    [SerializeField]
    Vector3 rallyPos;
    Vector3 InstanPos;
    float distanceFromBaseZ = 7.0f;
    float distanceFromBaseX = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 baseBuildingPosition = gameObject.transform.position;
        InstanPos = baseBuildingPosition + new Vector3(distanceFromBaseX, 0, distanceFromBaseZ);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && gameObject.transform.GetChild(0).gameObject.activeSelf && !hasClick)
        {
            OnTankProductButtonClick();
        }
        if (Input.GetKeyDown(KeyCode.G) && gameObject.transform.GetChild(0).gameObject.activeSelf && !hasClick)
        {
            OnGravProductButtonClick();
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
            if (queue.Count > 0 && current == 0)
            { current = queue.Dequeue(); }
            if (current.Equals(1))
            {
                productionProgress += Time.deltaTime;
                if (productionProgress >= TankproductionTime)
                {
                    GameObject TankObject = Instantiate(Tank, InstanPos, Quaternion.identity);
                    TankObject.GetComponent<UnitBehavior>().owner = gameObject.GetComponent<Building>().owner;
                    if (rallyPos != Vector3.zero)
                    {
                        TankObject.GetComponent<UnitBehavior>().agent.SetDestination(rallyPos);
                    }
                    TankObject.GetComponent<UnitBehavior>().agent.SetDestination(TankObject.transform.position + new Vector3(3, 0, 3));
                    productionProgress = 0;
                    productionQuene -= 1;
                    current = 0;
                }
            }
            else if (current.Equals(2))
            {
                productionProgress += Time.deltaTime;
                if (productionProgress >= GravproductionTime)
                {
                    GameObject GravObject = Instantiate(Grav, InstanPos, Quaternion.identity);
                    GravObject.GetComponent<UnitBehavior>().owner = gameObject.GetComponent<Building>().owner;
                    if (rallyPos != Vector3.zero)
                    {
                        GravObject.GetComponent<UnitBehavior>().agent.SetDestination(rallyPos);
                    }
                    else
                    { GravObject.GetComponent<UnitBehavior>().agent.SetDestination(GravObject.transform.position + new Vector3(2, 0, 2)); }
                    productionProgress = 0;
                    productionQuene -= 1;
                    current = 0;
                }
            }
        }
    }
    public void OnTankProductButtonClick()
    {
        if (productionQuene < 6 && Tank.GetComponent<UnitBehavior>().CanAfford() && gameObject.GetComponent<Building>().IsUnit())
        {
            productionQuene += 1;
            queue.Enqueue(1);
            Tank.GetComponent<UnitBehavior>().DeductResources();
        }
    }
    public void OnGravProductButtonClick()
    {
        if (productionQuene < 6 && Grav.GetComponent<UnitBehavior>().CanAfford() && gameObject.GetComponent<Building>().IsUnit())
        {
            productionQuene += 1;
            queue.Enqueue(2);
            Grav.GetComponent<UnitBehavior>().DeductResources();
        }
    }
}
