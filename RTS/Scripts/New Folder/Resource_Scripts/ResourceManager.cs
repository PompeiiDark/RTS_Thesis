using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.UI;

public enum ResourceType
{
    Gold,
    Crystal,// 新添加的水晶资源类型
    // 其他资源类型
}
public class ResourceManager : MonoBehaviour
{
    private static ResourceManager _instance;
    public static ResourceManager instance { get { return _instance; } }
    public int startingCrystal = 100;
    public int startingGold = 100;

    [SerializeField]
    private int currentCrystal;
    [SerializeField]
    private int currentGold;

    public Text Gold;
    public Text Crystal;
    // 在这里定义其他资源

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentCrystal = startingCrystal;
        currentGold = startingGold;
        if (Gold != null && Crystal != null)
        {
            Gold.text = currentGold.ToString();
            Crystal.text = currentCrystal.ToString();
        }
    }
    public void AddResource(ResourceType type, int amount)
    {
        if (type == ResourceType.Crystal)
        {
            currentCrystal += amount;
            Crystal.text = currentCrystal.ToString();
            //update UI
        }
        else if (type == ResourceType.Gold)
        {
            currentGold += amount;
            Gold.text = currentGold.ToString();
            //update UI
        }
    }

    public bool SpendResources(int gold, int crystal)
    {
        if (currentGold >= gold && currentCrystal >= crystal)
        {
            currentCrystal -= crystal;
            currentGold -= gold;
            Gold.text = currentGold.ToString();
            Crystal.text = currentCrystal.ToString();
            return true;
        }
        return false;
    }
    // 在这里添加其他资源的处理方法

    // 获取当前木材数量
    public int GetCurrentCrystal()
    {
        return currentCrystal;
    }

    // 获取当前金币数量
    public int GetCurrentGold()
    {
        return currentGold;
    }

}
