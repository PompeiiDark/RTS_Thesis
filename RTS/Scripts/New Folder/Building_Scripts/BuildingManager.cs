using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public GameObject _placeholder;

    public GameObject BaseBuildingPlaceholder;
    public GameObject SecondBuildingPlaceholder;
    public GameObject BaseBuilding;
    public GameObject SecondBuilding;

    private Vector3 _mousePosition;
    private float _previousX;
    private float _previousZ;

    public PlaceholderBuilding _buildingScript;
    public Building building;

    public bool CanBuild = false;
    private bool IsMouseDown = false;

    public List<GameObject>buildings = new List<GameObject>();
    public List<GameObject>selectedBuilding = new List<GameObject>();
    public List<GameObject> enemysObject = new List<GameObject>();

    public Transform avatar_cameraTrans;

    private static BuildingManager _instance;
    public static BuildingManager instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null & _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //building
        if (_placeholder != null)
        {
            _mousePosition = Input.mousePosition;

            Ray ray = Camera.main.ScreenPointToRay(_mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                float positionX = hit.point.x;
                float positionZ = hit.point.z;

                if (_previousX != positionX || _previousZ != positionZ)
                {
                    _previousX = positionX;
                    _previousZ = positionZ;
                    if (!IsMouseDown)
                        _placeholder.transform.position = new Vector3(positionX, 0f, positionZ);//placeholder always at mouse pos
                }

                if (Input.GetMouseButtonDown(0))
                {
                    IsMouseDown = true;
                }
                else if (Input.GetMouseButtonDown(1) || Input.GetKey(KeyCode.S))
                {
                    foreach (GameObject unit in GameControllor.instance.currenSelection_Unit)
                    {
                        if (unit.GetComponent<Drone>().place != null)
                        {
                            Destroy(unit.GetComponent<Drone>().place);
                            SetBuildOff();
                            unit.GetComponent<Drone>().hasBuildClickRun = false;
                        }
                    }
                    //SetBuildOff();
                }

                if (IsMouseDown)
                {
                    if (_buildingScript.isBuilding && building.CanAfford())
                    {
                        CanBuild = true;
                        foreach (GameObject unit in GameControllor.instance.currenSelection_Unit)
                        {
                            if (unit.GetComponent<Drone>().place != null && unit.transform.GetChild(1).gameObject.activeSelf)
                            {
                                unit.GetComponent<Drone>().Canbuild = true;
                                unit.GetComponent<Drone>().hasBuildClickRun = false;
                            }
                        }
                    }
                }

            }
        }
    }
    public void SetPlaceholder()
    {
        if (_placeholder != null)
        {
            _buildingScript = _placeholder.GetComponent<PlaceholderBuilding>();
            building = BaseBuilding.GetComponent<Building>();
        }
    }
    public void SetBuildOff()//Drone接受完数据后立刻清理数据,为其它drone让位
    {
        _placeholder = null;
        building = null;
        _buildingScript = null;
        IsMouseDown = false;
        CanBuild = false;
    }





    public void OnLeftMouseClickSelect(GameObject unit)
    {
        DeselectAll();
        GameControllor.instance.DeselectAll();
        if (unit.GetComponent<Building>().IsUnit())
        {
            selectedBuilding.Add(unit);
            unit.transform.GetChild(0).gameObject.SetActive(true);//Active the selected circle
        }
        unit.transform.GetChild(1).gameObject.SetActive(true);//UI Panel active
        SetCameraPos(unit.GetComponent<Building>().cameraPos);
        //ShowCurrentPanel();
    }
    public void OnShiftClickSelect(GameObject unit)
    {
        GameControllor.instance.DeselectAll();
        //if not contain
        if (unit.GetComponent<Building>().IsUnit())
        {
            if (!selectedBuilding.Contains(unit))
            {
                selectedBuilding.Add(unit);
                unit.transform.GetChild(0).gameObject.SetActive(true);
                SetCameraPos(unit.GetComponent<Building>().cameraPos);
            }
            else
            {
                unit.transform.GetChild(0).gameObject.SetActive(false);
                selectedBuilding.Remove(unit);
            }
        }
    }
    public void DeselectAll()
    {
        if (selectedBuilding != null)
        {
            foreach (var unit in selectedBuilding)
            {
                if (unit != null)
                {
                    unit.transform.GetChild(0).gameObject.SetActive(false);
                    unit.transform.GetChild(1).gameObject.SetActive(false);
                }
            }
            selectedBuilding.Clear();
        }
    }
    public void SetCameraPos(Transform cameraPos)
    {
        avatar_cameraTrans.SetParent(cameraPos);
        avatar_cameraTrans.localPosition = Vector3.zero;
        avatar_cameraTrans.localRotation = Quaternion.identity;
    }
}