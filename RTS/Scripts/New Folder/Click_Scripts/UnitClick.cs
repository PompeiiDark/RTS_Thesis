using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitClick : MonoBehaviour
{
    private new Camera camera;

    public LayerMask Unit;
    public LayerMask Building;
    public LayerMask Ground;
    // Start is called before the first frame update
    void Start()
    {
        camera= Camera.main;
    }

    //Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))//May has problem
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (!EventSystem.current.IsPointerOverGameObject())//mouse is not at UI layer
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, Unit))
                {
                    //hit a clickable object
                    if (Input.GetKey(KeyCode.LeftShift) && hit.collider.gameObject != null)
                    {
                        //shift click
                        GameControllor.instance.OnShiftClickSelect(hit.collider.gameObject);
                    }
                    else if(hit.collider.gameObject != null)
                    {
                        //single mouse click
                        GameControllor.instance.OnLeftMouseClickSelect(hit.collider.gameObject);
                    }
                }
                else if (Physics.Raycast(ray, out hit, Mathf.Infinity, Building))
                {
                    //hit a clickable object
                    if (Input.GetKey(KeyCode.LeftShift) && hit.collider.gameObject != null)
                    {
                        //shift click
                        BuildingManager.instance.OnShiftClickSelect(hit.collider.gameObject);
                    }
                    else if(hit.collider.gameObject != null)
                    {
                        //single mouse click
                        BuildingManager.instance.OnLeftMouseClickSelect(hit.collider.gameObject);
                    }
                }
                //else
                //{
                //    GameControllor.instance.DeselectAll();
                //    BuildingManager.instance.DeselectAll();
                //    //if (!Input.GetKey(KeyCode.LeftShift))
                //    //{
                //    //    GameControllor.instance.DeselectAll();
                //    //}
                //}
            }
        }
    }
}
