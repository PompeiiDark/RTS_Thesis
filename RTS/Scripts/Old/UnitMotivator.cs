using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitMoviation : MonoBehaviour
{
    public NavMeshAgent Agent;
    public Infomation info;
    public Vector3Observable RightClickTarget;
    public void Awake()
    {
        this.RightClickTarget.AddListener(OnRightClick);
    }
    private void OnRightClick(Vector3 destination)
    {
        if (!this.info.selectable.Selected) return;

        Agent.destination = destination;
    }
}
