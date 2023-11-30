using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameControllor : MonoBehaviour
{
    private static GameControllor _instance;
    public static GameControllor instance { get { return _instance; } }


    public List<GameObject> existingunit = new List<GameObject>();
    public List<GameObject> currenSelection_Unit = new List<GameObject>();
    public List<GameObject> enemysObject = new List<GameObject>();

    public Transform avatar_cameraTrans;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance= this;
        }
        //instance= this;
    }
    private void Update()
    {
        
    }

    public void OnLeftMouseClickSelect(GameObject unit)
    {
        DeselectAll();
        BuildingManager.instance.DeselectAll();
        if (unit.GetComponent<UnitBehavior>().IsUnit())
        {
            currenSelection_Unit.Add(unit);
            unit.transform.GetChild(0).gameObject.SetActive(true);//Active the selected circle
            unit.transform.GetChild(1).gameObject.SetActive(true);//UI Panel active
        }
        SetCameraPos(unit.GetComponent<UnitBehavior>().cameraPos);
    }
    public void OnShiftClickSelect(GameObject unit)
    {
        BuildingManager.instance.DeselectAll();
        //if not contain
        if (unit.GetComponent<UnitBehavior>().IsUnit())
        {
            if (!currenSelection_Unit.Contains(unit))
            {
                currenSelection_Unit.Add(unit);
                unit.transform.GetChild(0).gameObject.SetActive(true);
                SetCameraPos(unit.GetComponent<UnitBehavior>().cameraPos);
            }
            else
            {
                unit.transform.GetChild(0).gameObject.SetActive(false);
                currenSelection_Unit.Remove(unit);
            }
        }
    }
    public void DragSelect(GameObject unit)
    {
        //DeselectAll(); Run in foreach,will not work if deselectAll!!!
        BuildingManager.instance.DeselectAll();
        if (unit.GetComponent<UnitBehavior>().IsUnit())
        {
            currenSelection_Unit.Add(unit);
            unit.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
    public void DeselectAll()
    {
        if (currenSelection_Unit != null)
        {
            foreach (var unit in currenSelection_Unit)
            {
                if (unit != null)
                {
                    unit.transform.GetChild(0).gameObject.SetActive(false);
                    unit.transform.GetChild(1).gameObject.SetActive(false);
                }
            }
            currenSelection_Unit.Clear();
        }
    }
    public void SetCameraPos(Transform cameraPos)
    {
        if (cameraPos != null)
        {
            avatar_cameraTrans.SetParent(cameraPos);
            avatar_cameraTrans.localPosition = Vector3.zero;
            avatar_cameraTrans.localRotation = Quaternion.identity;
        }
    }
}
