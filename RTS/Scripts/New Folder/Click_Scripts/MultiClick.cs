using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiClick : MonoBehaviour
{
    new Camera camera;
    //graphic
    [SerializeField]
    RectTransform BoxVisual;

    //logical
    Rect selectionBox;//Use Xmax/min ; Ymax/min

    Vector2 StartPos;
    Vector2 EndPos;



    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        StartPos = Vector2.zero; 
        EndPos = Vector2.zero;
        DrawVisual();
    }

    // Update is called once per frame
    void Update()
    {
        //when clicked
        if (Input.GetMouseButtonDown(0))
        {
            StartPos = Input.mousePosition;
            selectionBox = new Rect();
        }
        //when holding click(drag)
        if (Input.GetMouseButton(0))
        {
            EndPos= Input.mousePosition;
            DrawVisual();
            DrawSelection();
        }
        //when release
        if (Input.GetMouseButtonUp(0))
        {
            SelectUnits();
            StartPos = Vector2.zero;
            EndPos = Vector2.zero;
            DrawVisual();
        }
    }

    //draw a visual box when drag mouse
    void DrawVisual()
    {
        Vector2 BoxStart = StartPos;
        Vector2 BoxEnd = EndPos;
        Vector2 BoxCenter = (BoxStart + BoxEnd) / 2;
        BoxVisual.position = BoxCenter;

        Vector2 BoxSize = new Vector2(MathF.Abs(BoxStart.x - BoxEnd.x), MathF.Abs(BoxStart.y - BoxEnd.y));
        BoxVisual.sizeDelta= BoxSize;
    }

    void DrawSelection()
    {
        //X calculation
        if (Input.mousePosition.x < StartPos.x)
        {
            //Draging to left
            selectionBox.xMin = Input.mousePosition.x;
            selectionBox.xMax = StartPos.x;
        }
        else
        {
            //Draging to right
            selectionBox.xMax = Input.mousePosition.x;
            selectionBox.xMin = StartPos.x;
        }
        //Y calculation
        if (Input.mousePosition.y < StartPos.y)
        {
            //Draging to down
            selectionBox.yMin = Input.mousePosition.y;
            selectionBox.yMax = StartPos.y;
        }
        else
        { 
            //Draging to up
            selectionBox.yMax = Input.mousePosition.y;
            selectionBox.yMin = StartPos.y;
        }
    }

    void SelectUnits()
    {
        if (selectionBox.width >= 1 && selectionBox.height >= 1)//Distinguish between click and drag
        {//loop thus all the units
            GameControllor.instance.DeselectAll();
            foreach (var unit in GameControllor.instance.existingunit)
            {
                //if unit is at inside of the selection box,add it
                if (unit != null && selectionBox.Contains(camera.WorldToScreenPoint(unit.transform.position)))
                {
                    GameControllor.instance.DragSelect(unit);
                }
            }
        }
    }
}
