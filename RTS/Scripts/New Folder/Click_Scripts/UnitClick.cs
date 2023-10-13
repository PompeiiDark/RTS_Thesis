using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitClick : MonoBehaviour
{
    private new Camera camera;

    public LayerMask Clickable;
    public LayerMask Ground;
    // Start is called before the first frame update
    void Start()
    {
        camera= Camera.main;
    }

    //Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, Clickable))
            {
                //hit a clickable object
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    //shift click
                    GameControllor.instance.OnShiftClickSelect(hit.collider.gameObject);
                }
                else
                {
                    //single mouse click
                    GameControllor.instance.OnLeftMouseClickSelect(hit.collider.gameObject);
                }
            }
            else
            {
                GameControllor.instance.DeselectAll();
                //if (!Input.GetKey(KeyCode.LeftShift))
                //{
                //    GameControllor.instance.DeselectAll();
                //}
            }
        }
    }
}
