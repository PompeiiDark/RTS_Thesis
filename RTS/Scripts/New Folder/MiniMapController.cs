using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MiniMapController : MonoBehaviour
{

    [SerializeField]
    Camera cam; //for raycast of minimapCamera
    [SerializeField]
    GameObject camToMove; // the gameobject the camera is attached to:CamerSystem

    RaycastHit hit;
    Ray ray;
    [SerializeField]
    LayerMask mask;// ground layer 

    Vector3 movePoint;
    //float YPos;

    void Start()
    {
        if (cam == null)
        {
            cam = GetComponent<Camera>();
        }
    }
    void Update()
    {
        //to move camera :
        if (IspointerOverUiObject())
        {
            if (Input.GetMouseButton(0))
            {
                ray = cam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
                {
                    //YPos = camToMove.transform.position.y;
                    movePoint = new Vector3(hit.point.x, 0f, hit.point.z );
                    camToMove.transform.position = movePoint;
                }
            }
        }

        if (IspointerOverUiObject())
        {
            if (Input.GetMouseButton(1) && GameControllor.instance.currenSelection_Unit.Count > 0)
            {
                ray = cam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
                {
                    foreach (var u in GameControllor.instance.currenSelection_Unit)
                    {
                        u.GetComponent<UnitBehavior>().agent.SetDestination(new Vector3(hit.point.x, u.transform.position.y, hit.point.z));
                    }
                }
            }
        }
    }
    //this function dectects clicks on ui objects
    private static bool IspointerOverUiObject()
    {
        PointerEventData EventDataCurrentPosition = new PointerEventData(EventSystem.current);
        EventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> result = new List<RaycastResult>();
        EventSystem.current.RaycastAll(EventDataCurrentPosition, result);
        return result.Count > 0;

    }
}
