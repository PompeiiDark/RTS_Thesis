using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class AddUnitsToPlayer : MonoBehaviour
{
    public Infomation info;
    private void Start()
    {
        info.player.Units.Add(info);
    }
}
