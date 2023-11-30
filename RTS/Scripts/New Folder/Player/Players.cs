using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Players : MonoBehaviour
{
    public UpgradeManager upgradeManager;
    public int Id;
    public List<Players> enemys = new List<Players>();
    public List<Players> ally = new List<Players>();
    private void Start()
    {
    }
    public bool IsEnemy(Players player)
    {
        return enemys.Contains(player);
    }
}
