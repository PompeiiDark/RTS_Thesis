using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

[RequireComponent(typeof(BoxCollider))]
public class Building : MonoBehaviour
{
    public GameObject BuildedEffect;
    public GameObject DamagedEffect;

    public Players owner;

    public Slider slider;
    public Text HpTxt;

    public string buildingName = "Building";
    public int crystalCost = 100;
    public int goldCost = 200;
    public int Hp = 500;
    [SerializeField]
    int currentHp;
    public float buildTime = 30;
    public GameObject buildingPrefab; // ���ǽ���������ɺ����Ϸ����

    public float buildProgress = 0.0f;

    public Transform cameraPos;
    // Start is called before the first frame update
    void Start()
    {
        if (IsUnit())
        {
            BuildingManager.instance.buildings.Add(gameObject);
        }
        else if (owner.IsEnemy(GameObject.Find("PlayerObject").GetComponent<Players>()))
        {
            BuildingManager.instance.enemysObject.Add(gameObject);
            owner.gameObject.GetComponent<Ai_Behavior>().Buildings.Add(gameObject);
        }
        //BuildingManager.instance.buildings.Add(gameObject);
        buildProgress = 0.0f;
        currentHp = Hp;
        if (HpTxt != null)
        {
            HpTxt.text = currentHp.ToString();
        }
        if (owner.gameObject.GetComponent<Ai_Behavior>())
        {
            Ai_Behavior ub = owner.gameObject.GetComponent<Ai_Behavior>();
            if (ub.diffLevel == 0)
            {
                buildTime += 5;
                crystalCost += 50;
                goldCost += 100;
            }
            else if (ub.diffLevel == 2)
            {
                buildTime -= 5;
                crystalCost -= 50;
                goldCost -= 100;
            }

            if (owner.IsEnemy(GameObject.Find("PlayerObject").GetComponent<Players>()))
            {
                transform.GetChild(2).GetComponent<MeshRenderer>().material.color = Color.red;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // �ж��Ƿ����㹻����Դ�����콨��
    public bool CanAfford()
    {
        // ������Ϸ�е���Դ����ϵͳ���ж�
        // �����������ΪResourceManager�Ĺ�����
        int currentCrystal = ResourceManager.instance.GetCurrentCrystal();
        int currentGold = ResourceManager.instance.GetCurrentGold();
        if (currentGold >= goldCost && currentCrystal >= crystalCost)
        {
            return true;
        } 
        return false;
    }

    // �۳���Դ
    public void DeductResources()
    {
        // ������Ϸ�е���Դ����ϵͳ���۳���Դ
        ResourceManager.instance.SpendResources(goldCost, crystalCost);
    }
    public void GetDamage(int damage)
    {
        if (currentHp <= 0)
        {
            return;
        }
        GameObject effect = Instantiate(DamagedEffect, transform.position, transform.rotation);
        Destroy(effect, 1.5f);
        currentHp -= damage;
        HpTxt.text = currentHp.ToString();
        slider.value = (float)currentHp / Hp;
        if (currentHp <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        //died effection
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        if (GameControllor.instance.avatar_cameraTrans.IsChildOf(cameraPos))
        {
            GameControllor.instance.avatar_cameraTrans.SetParent(null);
        }
        GameObject effect = Instantiate(DamagedEffect, transform.position, transform.rotation);
        Destroy(effect, 1.5f);
        //Destory
        Destroy(gameObject);
        GameObject.Find("Manager").transform.GetChild(5).GetComponent<InGameMenu>().CheckLose();
        GameObject.Find("Manager").transform.GetChild(5).GetComponent<InGameMenu>().CheckWin();
    }
    public bool IsUnit()
    {
        if (owner.Id == 0)
        {
            return true;
        }
        return false;

    }
}
