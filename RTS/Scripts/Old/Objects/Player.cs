using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RTS/Player")]
public class Player : ScriptableObject
{
    public string Name;
    public Color color;
    public List<Infomation> Units;
    public void OnEnable()
    {
        Units = new List<Infomation>();
    }
}
