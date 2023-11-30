using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public int Attack_Time = 0;
    public int MaxAttack = 2;
    public int Armor_Time = 0;
    public int MaxArmor = 2;

    public int AddAtkNum = 5;
    public int AddArmNum = 1;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetAtkUp(Players p)
    {
        if (Attack_Time < MaxAttack)
        {
            Attack_Time += 1;
            if (p.Id == 0)
            {
                foreach (var u in GameControllor.instance.existingunit)
                {
                    if (u != null)
                    {
                        u.GetComponent<UnitBehavior>().AttackPower += AddAtkNum;
                        u.GetComponent<UnitBehavior>().AtkTxt.text = u.GetComponent<UnitBehavior>().AttackPower.ToString();
                    }
                }
                if (BuildingManager.instance.buildings != null)
                {
                    bool Isbase;
                    foreach (var building in BuildingManager.instance.buildings)
                    {
                        if (Isbase = building.GetComponent<Base>())
                        {
                            building.GetComponent<Base>().AtkLv.text = Attack_Time.ToString();
                        }
                    }
                }
            }
            else
            {
                Ai_Behavior ai = p.gameObject.GetComponent<Ai_Behavior>();
                foreach (var u in ai.Units)
                {
                    if (u != null)
                    {
                        u.GetComponent<UnitBehavior>().AttackPower += AddAtkNum;
                        u.GetComponent<UnitBehavior>().AtkTxt.text = u.GetComponent<UnitBehavior>().AttackPower.ToString();
                    }
                }
                if (ai.Buildings != null)
                {
                    bool Isbase;
                    foreach (var building in ai.Buildings)
                    {
                        if (Isbase = building.GetComponent<Base>())
                        {
                            building.GetComponent<Base>().AtkLv.text = Attack_Time.ToString();
                        }
                    }
                }
            }
        }
    }
    public void SetAmrUp(Players p)
    {
        if (Armor_Time < MaxArmor)
        {
            Armor_Time += 1;
            if (p.Id == 0)
            {
                foreach (var u in GameControllor.instance.existingunit)
                {
                    if (u != null)
                    {
                        u.GetComponent<UnitBehavior>().Armor += AddArmNum;
                        u.GetComponent<UnitBehavior>().AmrTxt.text = u.GetComponent<UnitBehavior>().Armor.ToString();
                    }
                }
                if (BuildingManager.instance.buildings != null)
                {
                    foreach (var building in BuildingManager.instance.buildings)
                    {
                        if (building.GetComponent<Base>())
                        {
                            building.GetComponent<Base>().AtkLv.text = Attack_Time.ToString();
                        }
                    }
                }
            }
            else
            {
                Ai_Behavior ai = p.gameObject.GetComponent<Ai_Behavior>();
                foreach (var u in ai.Units)
                {
                    if (u != null)
                    {
                        u.GetComponent<UnitBehavior>().Armor += AddArmNum;
                        u.GetComponent<UnitBehavior>().AmrTxt.text = u.GetComponent<UnitBehavior>().Armor.ToString();
                    }
                }
                if (ai.Buildings != null)
                {
                    foreach (var building in ai.Buildings)
                    {
                        if (building.GetComponent<Base>())
                        {
                            building.GetComponent<Base>().AtkLv.text = Attack_Time.ToString();
                        }
                    }
                }
            }
        }
    }
}
