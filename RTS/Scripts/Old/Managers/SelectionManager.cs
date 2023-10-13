using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public float DoubleClickRadius = 10f;
    private List<Infomation> SelectedInfo = new List<Infomation>();
    // Start is called before the first frame update
    void Start()
    {
        InteractionManager.Instance.AddMouseListener(
            source: InteractionSource.MouseButton1,
            interaction: InteractionType.MouseClick,
            eventHandler: OnSingleClick);
        InteractionManager.Instance.AddMouseListener(
            source: InteractionSource.MouseButton1,
            interaction: InteractionType.MouseDoubleClick,
            eventHandler: OnDoubleClick);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnSingleClick(Vector2 MousePosition)
    {
        if (UiCheck.IsMouseOverUI())
        {
            //Mouse is over UI
            return;
        }
        Ray ray = Camera.main.ScreenPointToRay(MousePosition);
        RaycastHit hit;
        var shift = Input.GetKey(KeyCode.LeftShift);
        if (Physics.Raycast(ray, out hit))
        {
            var info = hit.transform.GetComponentInChildren<Infomation>();
            if (!shift && (info == null || info.selectable == null))
            {
                UnselectAll();
                return;
            }
            if (info == null)
            {
                return;
            }
            if (shift)
            {
                //add the selected one to the collection
                SelectedInfo.Add(info);
                info.selectable.Select();
                return;
            }
            else
            {
                //only select the chose one
                this.UnselectAll();
                this.SelectedInfo.Add(info);
                info.selectable.Select();
            }
        }
        else
        {
            if(!shift) UnselectAll();
        }
    }
    private void OnDoubleClick(Vector2 MousePosition)
    {
        if (UiCheck.IsMouseOverUI())
        {
            //Mouse is over UI
            return;
        }
        Ray ray = Camera.main.ScreenPointToRay(MousePosition);
        RaycastHit hit;
        var shift = Input.GetKey(KeyCode.LeftShift);
        if (Physics.Raycast(ray, out hit))
        {
            var info = hit.transform.GetComponentInChildren<Infomation>();
            if (!shift && (info == null || info.selectable == null))
            {
                UnselectAll();
                return;
            }
            if (info == null)
            {
                return;
            }
            if (!shift)
            {
                UnselectAll();
                SelectedInfo.Add(info);
                info.selectable.Select();
            }
            foreach (var info2 in info.player.Units)
            {
                if (info2.unit.name == info.unit.name)
                {
                    var distance = Vector3.Distance(
                    info.transform.position,
                    info2.transform.position);
                    if (distance > DoubleClickRadius) continue;

                    this.SelectedInfo.Add(info2);
                    info2.selectable.Select();
                }

            }
        }
        else
        {
            if (!shift) UnselectAll();
        }
    }
    private void UnselectAll()
    {
        Infomation IgnoreInfo = null;
        foreach(var info in this.SelectedInfo)
        {
            if (info == IgnoreInfo) continue;
            info.selectable.Unselect();
        }
        this.SelectedInfo.Clear();
    }
}
