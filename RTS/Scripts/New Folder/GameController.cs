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
        if (_instance != null & _instance != this)
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
        currenSelection_Unit.Add(unit);
        unit.transform.GetChild(0).gameObject.SetActive(true);//Active the selected circle
        unit.transform.GetChild(1).gameObject.SetActive(true);//UI Panel active
        unit.GetComponent<UnitBehavior>().enabled=true;
        //ShowCurrentPanel();
    }
    public void OnShiftClickSelect(GameObject unit)
    {
        //if not contain
        if (!currenSelection_Unit.Contains(unit))
        {
            currenSelection_Unit.Add(unit);
            unit.transform.GetChild(0).gameObject.SetActive(true);
            unit.GetComponent<UnitBehavior>().enabled = true;
        }
        else
        {
            unit.GetComponent<UnitBehavior>().enabled = false;
            unit.transform.GetChild(0).gameObject.SetActive(false);
            currenSelection_Unit.Remove(unit);
        }
    }
    public void DragSelect(GameObject unit)
    {
        DeselectAll();
        currenSelection_Unit.Add(unit);
        unit.transform.GetChild(0).gameObject.SetActive(true);
        unit.GetComponent<UnitBehavior>().enabled = true;
    }
    public void DeselectAll()
    {
        foreach (var unit in currenSelection_Unit)
        {
            unit.GetComponent<UnitBehavior>().enabled = false;
            unit.transform.GetChild(0).gameObject.SetActive(false);
            unit.transform.GetChild(1).gameObject.SetActive(false);
        }
        currenSelection_Unit.Clear();
    }
    //public void OnRightClickAttack(GameObject enemy)
    //{
    //    DeselectEnemy();
    //    enemyObject=enemy;
    //}
    //public void DeselectEnemy()
    //{
    //    enemyObject=null;
    //}
    public void SetCameraPos(Transform cameraPos)
    {
        avatar_cameraTrans.SetParent(cameraPos);
        avatar_cameraTrans.localPosition = Vector3.zero;
        avatar_cameraTrans.localRotation = Quaternion.identity;
    }
    //public void ShowCurrentPanel()
    //{
    //    for (int i = 0; i < Panels.Length; i++)
    //    {
    //        Panels[i].SetActive(false);
    //    }
    //    if (currenSelection_Unit[0] != null)
    //    {
    //        if (currenSelection_Unit[0].name.Equals("Drone"))
    //        {
    //            Panels[1].SetActive(true);
    //        }
    //        else if (currenSelection_Unit[0].name.Equals("Grav"))
    //        {
    //            Panels[2].SetActive(true);
    //        }
    //        else if (currenSelection_Unit[0].name.Equals("Tank"))
    //        {
    //            Panels[3].SetActive(true);
    //        }
    //    }
    //    else
    //    {
    //        Panels[0].SetActive(true);
    //    }
    //}
}
