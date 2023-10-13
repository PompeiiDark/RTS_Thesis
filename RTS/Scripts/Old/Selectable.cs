using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Selectable : MonoBehaviour
{
    public bool Selected = false;

    public UnityEvent OnSelect;
    public UnityEvent OnUnselect;

    public void Select()
    {
        if (!Selected) OnSelect.Invoke();
        Selected = true;
    }

    public void Unselect()
    {
        if (Selected) OnUnselect.Invoke();
        Selected = false;
    }
}

