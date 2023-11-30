using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.UI.CanvasScaler;
using Random = UnityEngine.Random;

public class Ai_fighter : MonoBehaviour
{
    public int totalFighterNum = 0;
    public float assembleRadius = 10f; // 中心位置随机偏移的半径
    Vector3 targetPosition = Vector3.zero;

    float timer = 0f;
    float fightInterval = 15f;
    // Update is called once per frame
    void Update()
    {
        Ai_Behavior behavior = gameObject.GetComponent<Ai_Behavior>();
        totalFighterNum = CalcFighterNum();
        if (totalFighterNum >= 4)
        {
            timer += Time.deltaTime;
            if (timer >= fightInterval)
            { 
                Assemble();
            }
        }
        foreach (var fighter in behavior.Units) //fighters go fight when they re assemble
        {
            if (fighter != null && !fighter.GetComponent<Drone>())
            {

                if (fighter.GetComponent<UnitBehavior>().IsAttacking != true)//fighter
                {
                    Collider[] colliders = Physics.OverlapSphere(fighter.transform.position, fighter.GetComponent<UnitBehavior>().FindRange, fighter.GetComponent<UnitBehavior>().unit);
                    List<Collider> FighterColliders = new List<Collider>();
                    if (colliders.Length > 0)
                    {
                        foreach (var c in colliders)
                        {
                            if (c != null && fighter.GetComponent<UnitBehavior>().owner.Id == c.gameObject.GetComponent<UnitBehavior>().owner.Id)
                            {
                                FighterColliders.Add(c);//get all fighter units around
                            }
                        }
                        if (FighterColliders.Count > 3)
                        {
                            Vector3 pos = Vector3.zero;
                            UnitBehavior ub = fighter.GetComponent<UnitBehavior>();
                            if (ub.owner.IsEnemy(GameObject.Find("PlayerObject").GetComponent<Players>()))//if player is the enemy,attack player first
                            {
                                pos = GetPlayerBasePos();
                                SetFighters_Fight(FighterColliders, pos);
                                SetFighter_Fight(ub, pos);
                            }
                            else
                            {
                                List<Players> enemys = gameObject.GetComponent<Players>().enemys;
                                if (enemys != null)
                                {
                                    foreach (var e in enemys)
                                    {
                                        if (e != null)
                                        {
                                            pos = GetAiEnemyBasePos(e.gameObject.GetComponent<Ai_Behavior>());
                                            break;
                                        }
                                    }
                                }
                                SetFighters_Fight(FighterColliders, pos);
                                SetFighter_Fight(ub, pos);
                            }
                        }
                    }
                }
            }
            else if (fighter == null)
            {
                behavior.Units = behavior.Units.FindAll(x => x != null);
            }
        }
    }
    public int CalcFighterNum()
    {
        Ai_Behavior obj = gameObject.GetComponent<Ai_Behavior>();
        int num = 0;
        foreach (var unit in obj.Units)
        {
            if (unit != null && !unit.GetComponent<Drone>())
            {
                num++;
            }
        }
        return num;
    }
    Vector3 CalculateFightersAveragePosition()
    {
        Ai_Behavior behavior = gameObject.GetComponent<Ai_Behavior>();
        List<GameObject> fighters = new List<GameObject>();
        foreach (var fighter in behavior.Units)
        {
            if (fighter != null && !fighter.GetComponent<Drone>())//fighter
            {
                fighters.Add(fighter);
            }
            else if (fighter == null)
            {
                behavior.Units = behavior.Units.FindAll(x => x != null);
            }
        }
        GameObject[] units = fighters.ToArray();
        Vector3 sum = Vector3.zero;

        foreach (GameObject unit in units)
        {
            if (unit != null)
            { 
                sum += unit.transform.position; 
            }
        }

        return sum / units.Length;
    }
    public void Assemble()
    {
        Ai_Behavior behavior = gameObject.GetComponent<Ai_Behavior>();        
        Vector3 centerPosition = CalculateFightersAveragePosition();
        Vector3 randomOffset = new Vector3(
                    Random.Range(-assembleRadius, assembleRadius),
                    0f,
                    Random.Range(-assembleRadius, assembleRadius)
                );

        // 计算最终目标位置
        targetPosition = centerPosition + randomOffset;
        foreach (var fighter in behavior.Units)
        {
            UnitBehavior ub = fighter.GetComponent<UnitBehavior>();
            if (fighter != null && !fighter.GetComponent<Drone>() && fighter.GetComponent<UnitBehavior>().IsAttacking == false)//fighter go to assemble position and check enemy on the way
            {
                if (targetPosition != Vector3.zero)
                {
                    ub.agent.SetDestination(targetPosition);
                    ub.IsFindingEnemy = true;
                }
            }
            else if (fighter == null)
            {
                behavior.Units = behavior.Units.FindAll(x => x != null);
            }
        }
        timer = 0f;
    }
    public Vector3 GetPlayerBasePos()
    {
        Vector3 pos = Vector3.zero;
        foreach (var b in BuildingManager.instance.buildings)
        {
            if (b != null && b.GetComponent<Base>())
            {
                pos = b.transform.position;
                break;
            }
        }
        return pos;
    }
    public Vector3 GetAiEnemyBasePos(Ai_Behavior ai)
    {
        Vector3 pos = Vector3.zero;
        foreach (var b in ai.Buildings)
        {
            if (b != null && b.GetComponent<Base>())
            {
                pos = b.transform.position;
                break;
            }
        }
        return pos;
    }
    public void SetFighter_Fight(UnitBehavior ub, Vector3 pos)
    {
        if (ub != null && pos != Vector3.zero && ub.IsAttacking == false)
        {
            ub.agent.SetDestination(pos);
            ub.IsFindingEnemy = true;
        }
    }
    public void SetFighters_Fight(List<Collider> FighterColliders,Vector3 pos)
    {
        if (FighterColliders.Count > 0)
        {
            foreach (var f in FighterColliders)
            {
                UnitBehavior ub1 = f.gameObject.GetComponent<UnitBehavior>();
                if (ub1 != null && pos != Vector3.zero && ub1.IsAttacking == false)
                {
                    ub1.agent.SetDestination(pos);
                    ub1.IsFindingEnemy = true;
                }
            }
        }
    }
}
