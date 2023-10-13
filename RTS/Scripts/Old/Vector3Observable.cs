using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Monitor the listeners change,when the value does change,back the info to the listener
[CreateAssetMenu(menuName ="RTS/Observables/Vector3")]
public class Vector3Observable : ScriptableObject
{
    [SerializeField]
    private Vector3 DefaultValue = Vector3.zero;
    [SerializeField]
    private Vector3 CurrentValue = Vector3.zero;

    private Action<Vector3> Listeners = new Action<Vector3>((x) => { });

    private void OnEnable() 
    {
        this.CurrentValue = this.DefaultValue;
    }
    private void OnDisable()
    {
        this.CurrentValue= this.DefaultValue;
    }
    private void OnValidate()
    {
        this.Listeners(this.CurrentValue);
    }
    public void AddListener(Action<Vector3> listener)
    {
        this.Listeners += listener;
    }
    public void RemoveListener(Action<Vector3> listener)
    {
        this.Listeners -= listener;
    }
    public void Set(Vector3 value)
    {
        this.Value = value;
    }
    public Vector3 Value
    {
        get { return this.CurrentValue; }
        set
        {
            if (this.CurrentValue.x == value.x && this.CurrentValue.y == value.y && this.CurrentValue.z == value.z) return;
            this.CurrentValue+= value;
            this.Listeners(value);
        }
    }
}
