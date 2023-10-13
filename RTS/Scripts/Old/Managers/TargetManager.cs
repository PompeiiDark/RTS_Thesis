using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    public Vector3Observable RightClickTarget;
    // Start is called before the first frame update
    void Start()
    {
        InteractionManager.Instance.AddMouseListener(
            source:InteractionSource.MouseButton2,
            interaction: InteractionType.MouseClick,
            eventHandler: MouseRighrClickHandle);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void MouseRighrClickHandle(Vector2 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (UiCheck.IsMouseOverUI())
        {
            return;
        }    
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            this.RightClickTarget.Value = hit.point;
        }
    }
}
