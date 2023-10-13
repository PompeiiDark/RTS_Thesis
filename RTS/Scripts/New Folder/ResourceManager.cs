using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
public enum ResourceType
{
    Gold,
    Crystal,// ����ӵ�ˮ����Դ����
    // ������Դ����
}
public class ResourceManager : MonoBehaviour
{
    private static ResourceManager _instance;
    public static ResourceManager instance { get { return _instance; } }
    public int startingCrystal = 100;
    public int startingGold = 100;

    private int currentCrystal;
    private int currentGold;

    // �����ﶨ��������Դ

    private void Awake()
    {
        if (_instance != null & _instance != this)
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddResource(ResourceType type, int amount)
    {
        if (type == ResourceType.Crystal)
        {
            currentCrystal += amount;
            //update UI
        }
        else if (type == ResourceType.Gold)
        {
            currentGold += amount;
            //update UI
        }
    }

    public bool SpendResources(int gold, int crystal)
    {
        if (currentGold >= gold && currentCrystal >= crystal)
        {
            currentCrystal -= crystal;
            currentGold -= gold;
            return true;
        }
        return false;
    }
    // ���������������Դ�Ĵ�����

    // ��ȡ��ǰľ������
    public int GetCurrentWood()
    {
        return currentCrystal;
    }

    // ��ȡ��ǰ�������
    public int GetCurrentGold()
    {
        return currentGold;
    }

}
