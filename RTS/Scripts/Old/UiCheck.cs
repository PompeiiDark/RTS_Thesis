using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiCheck : MonoBehaviour
{
    public static bool IsMouseOverUI()
    {
        PointerEventData eventData_CurrentPosition = new PointerEventData(EventSystem.current);
        eventData_CurrentPosition.position = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
        List<RaycastResult> results= new List<RaycastResult>();//it null list
        EventSystem.current?.RaycastAll(eventData_CurrentPosition, results);
        return results.Count > 0;//something is truly in the mouse point(UI),return true
    }
}
