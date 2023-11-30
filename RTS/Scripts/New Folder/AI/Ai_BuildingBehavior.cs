using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ai_BuildingBehavior : MonoBehaviour
{
    public int UpNeedNum = 12;
    public int MaxDroneNum = 10;
    // Update is called once per frame
    void Update()
    {
        Ai_Behavior behavior = gameObject.GetComponent<Ai_Behavior>();
        foreach (var b in behavior.Buildings)
        {
            if (b != null)
            {
                MilitaryCamp mc = b.GetComponent<MilitaryCamp>();
                Base bs = b.GetComponent<Base>();

                if (mc != null)//product fighter
                {
                    ProductFighter(mc);
                }
                else if (bs != null)
                {
                    ProductDrone(bs);
                    Upgrade(bs);
                }
            }
            else if (b == null)
            {
                behavior.Buildings = behavior.Buildings.FindAll(x => x != null);
            }
        }
    }
    public void ProductFighter(MilitaryCamp mc)
    {
        Ai_Behavior obj = gameObject.GetComponent<Ai_Behavior>();
        UnitBehavior ubTank = mc.gameObject.GetComponent<MilitaryCamp>().Tank.GetComponent<UnitBehavior>();
        UnitBehavior ubGrav = mc.gameObject.GetComponent<MilitaryCamp>().Grav.GetComponent<UnitBehavior>();

        if (mc.productionQuene < 6 && obj.currentCrystal >= ubTank.crystalCost && obj.currentGold >= ubTank.goldCost)//product tank
        {
            obj.currentCrystal -= ubTank.crystalCost;
            obj.currentGold -= ubTank.goldCost;
            if (obj.currentCrystal < 0 || obj.currentGold < 0)
            {
                obj.currentCrystal += ubTank.crystalCost;
                obj.currentGold += ubTank.goldCost;
                return;
            }
            mc.productionQuene += 1;
            mc.queue.Enqueue(1);
        }

        if (mc.productionQuene < 6 && obj.currentCrystal >= 1.5 * ubGrav.crystalCost && obj.currentGold >= 1.5 * ubGrav.goldCost)
        {
            obj.currentCrystal -= ubGrav.crystalCost;
            obj.currentGold -= ubGrav.goldCost;
            if (obj.currentCrystal < 0 || obj.currentGold < 0)
            {
                obj.currentCrystal += ubGrav.crystalCost;
                obj.currentGold += ubGrav.goldCost;
                return;
            }
            mc.productionQuene += 1;
            mc.queue.Enqueue(2);
        }
    }
    public void ProductDrone(Base bs)
    {
        Ai_Behavior obj = gameObject.GetComponent<Ai_Behavior>();
        UnitBehavior ubDrone = bs.gameObject.GetComponent<Base>().Drone.GetComponent<UnitBehavior>();
        int num = CalcDroneNum() + bs.productionQuene;
        if (bs.productionQuene < 6 && obj.currentCrystal >= ubDrone.crystalCost && obj.currentGold >= ubDrone.goldCost && num < MaxDroneNum)//wont product infinity drone
        {
            obj.currentCrystal -= ubDrone.crystalCost;
            obj.currentGold -= ubDrone.goldCost;
            if (obj.currentCrystal < 0 || obj.currentGold < 0)
            {
                obj.currentCrystal += ubDrone.crystalCost;
                obj.currentGold += ubDrone.goldCost;
                return;
            }
            bs.productionQuene += 1;
        }
    }
    public void Upgrade(Base bs)
    {
        Debug.Log("AI base id:" + bs.GetComponent<Building>().owner.Id);
        Ai_Behavior obj = gameObject.GetComponent<Ai_Behavior>();
        UpgradeManager manager = gameObject.GetComponent<Players>().upgradeManager;
        if (obj.currentCrystal >= bs.UpCrystalCost && obj.currentGold >= bs.UpGoldCost && obj.Units.Count >= UpNeedNum)
        {
            if (manager.Attack_Time < manager.MaxAttack)
            {
                obj.currentCrystal -= bs.UpCrystalCost;
                obj.currentGold -= bs.UpGoldCost;
                if (obj.currentCrystal < 0 || obj.currentGold < 0)
                {
                    obj.currentCrystal += bs.UpCrystalCost;
                    obj.currentGold += bs.UpGoldCost;
                    return;
                }
                bs.IsAtkUp = true;
            }
            if (manager.Armor_Time < manager.MaxArmor)
            {
                obj.currentCrystal -= bs.UpCrystalCost;
                obj.currentGold -= bs.UpGoldCost;
                if (obj.currentCrystal < 0 || obj.currentGold < 0)
                {
                    obj.currentCrystal += bs.UpCrystalCost;
                    obj.currentGold += bs.UpGoldCost;
                    return;
                }
                bs.IsAmrUp = true;
            }
        }
    }
    public int CalcDroneNum()
    {
        Ai_Behavior obj = gameObject.GetComponent<Ai_Behavior>();
        int num = 0;
        foreach (var unit in obj.Units)
        {
            Drone drone = unit.GetComponent<Drone>();
            if (drone != null)
            {
                num++;
            }
        }
        return num;
    }
}
