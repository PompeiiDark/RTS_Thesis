using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//Contains direct references to other objects
//Can be passed as a reference to a specific unit by add new objects
public class Infomation : MonoBehaviour
{
    public Unit unit;
    public Selectable selectable;
    public Player player;
}
