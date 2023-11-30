using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CheckClickPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject obj = checkFirstPick(Input.mousePosition);
            Debug.Log("obj is:" + obj);
        }
    }
    public GameObject checkFirstPick(Vector2 pos)
    {
        EventSystem eventSystem = EventSystem.current;
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = pos;
        List<RaycastResult> uiRaycast = new List<RaycastResult>();
        eventSystem.RaycastAll(pointerEventData, uiRaycast);
        if(uiRaycast.Count >0)
            return uiRaycast[0].gameObject;
        return null;
    }
}
