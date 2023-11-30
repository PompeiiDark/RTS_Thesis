using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInit : MonoBehaviour
{
    int difficulty;
    int quantity;

    public GameObject player;
    public GameObject ai1;
    public GameObject ai2;
    public GameObject ai3;
    // Start is called before the first frame update
    void Start()
    {
        difficulty = GameObject.Find("MenuManager").GetComponent<MainMenu>().Difficulty;
        quantity = GameObject.Find("MenuManager").GetComponent<MainMenu>().Quantity;
        SetQuan();
        SetDiff();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetDiff()
    {
        List<Players> players = new List<Players>();
        players = player.GetComponent<Players>().enemys;
        foreach (var p in players)
        {
            p.gameObject.GetComponent<Ai_Behavior>().diffLevel = difficulty;
        }
    }
    void SetQuan()
    {
        GameObject[] unit = GameObject.FindGameObjectsWithTag("Unit");
        GameObject[] building = GameObject.FindGameObjectsWithTag("Building");
        List<GameObject> units = new List<GameObject>();
        units.AddRange(unit);
        List<GameObject> buildings = new List<GameObject>();
        buildings.AddRange(building);
        player.GetComponent<Players>().enemys.Clear();
        ai3.GetComponent<Players>().enemys.Clear();
        ai1.GetComponent<Players>().enemys.Clear();
        ai2.GetComponent<Players>().enemys.Clear();

        if (quantity == 0)//1v1
        {
            player.GetComponent<Players>().enemys.Add(ai1.GetComponent<Players>());
            ai1.GetComponent<Players>().enemys.Add(player.GetComponent<Players>());
            ai2.gameObject.SetActive(false);
            ai3.gameObject.SetActive(false);
        }
        else if (quantity == 1)
        {
            player.GetComponent<Players>().enemys.Add(ai1.GetComponent<Players>());
            player.GetComponent<Players>().enemys.Add(ai2.GetComponent<Players>());

            ai1.GetComponent<Players>().enemys.Add(player.GetComponent<Players>());
            ai2.GetComponent<Players>().enemys.Add(player.GetComponent<Players>());
            ai3.gameObject.SetActive(false);
        }
        else if (quantity == 2)
        {
            player.GetComponent<Players>().enemys.Add(ai1.GetComponent<Players>());
            player.GetComponent<Players>().enemys.Add(ai2.GetComponent<Players>());
            ai3.GetComponent<Players>().enemys.Add(ai1.GetComponent<Players>());
            ai3.GetComponent<Players>().enemys.Add(ai2.GetComponent<Players>());

            ai1.GetComponent<Players>().enemys.Add(player.GetComponent<Players>());
            ai1.GetComponent<Players>().enemys.Add(ai3.GetComponent<Players>());
            ai2.GetComponent<Players>().enemys.Add(player.GetComponent<Players>());
            ai2.GetComponent<Players>().enemys.Add(ai3.GetComponent<Players>());
        }

        foreach (var u in units)
        {
            if (quantity == 0)//1v1
            {
                if (u.GetComponent<UnitBehavior>().owner.Id == 0 || u.GetComponent<UnitBehavior>().owner.Id == 1)
                {
                    Debug.Log("ID of unit inside: " + u.GetComponent<UnitBehavior>().owner.Id);
                    u.gameObject.SetActive(true);
                }
                else
                {
                    Destroy(u.gameObject);
                }
                if (u.GetComponent<UnitBehavior>().owner.Id == 1)
                {
                    u.transform.GetChild(2).GetComponent<MeshRenderer>().material.color = Color.red;
                }
            }
            else if (quantity == 1)
            {

                if (u.GetComponent<UnitBehavior>().owner.Id == 0 || u.GetComponent<UnitBehavior>().owner.Id == 1 || u.GetComponent<UnitBehavior>().owner.Id == 2)
                {
                    u.gameObject.SetActive(true);
                }
                else
                {
                    Destroy(u.gameObject);
                }

                if (u.GetComponent<UnitBehavior>().owner.Id == 1 || u.GetComponent<UnitBehavior>().owner.Id == 2)
                {
                    u.transform.GetChild(2).GetComponent<MeshRenderer>().material.color = Color.red;
                }
            }
            else if (quantity == 2)
            {

                if (u.GetComponent<UnitBehavior>().owner.Id == 1 || u.GetComponent<UnitBehavior>().owner.Id == 2)
                {
                    u.transform.GetChild(2).GetComponent<MeshRenderer>().material.color = Color.red;
                }
                else
                {
                    u.transform.GetChild(2).GetComponent<MeshRenderer>().material.color = Color.green;
                }
            }
        }
        foreach (var b in buildings)
        {
            if (quantity == 0)
            {
                if (b.GetComponent<Building>().owner.Id == 0 || b.GetComponent<Building>().owner.Id == 1)
                {
                    b.gameObject.SetActive(true);
                }
                else
                {
                    Destroy(b.gameObject);
                }
                if (b.GetComponent<Building>().owner.Id == 1)
                {
                    b.transform.GetChild(2).GetComponent<MeshRenderer>().material.color = Color.red;
                }
            }
            else if (quantity == 1)
            {
                if (b.GetComponent<Building>().owner.Id == 0 || b.GetComponent<Building>().owner.Id == 1 || b.GetComponent<Building>().owner.Id == 2)
                {
                    b.gameObject.SetActive(true);
                }
                else
                {
                    Destroy(b.gameObject);
                }
                if (b.GetComponent<Building>().owner.Id == 1 || b.GetComponent<Building>().owner.Id == 2)
                {
                    b.transform.GetChild(2).GetComponent<MeshRenderer>().material.color = Color.red;
                }
            }
            else if (quantity == 2)
            {
                if (b.GetComponent<Building>().owner.Id == 1 || b.GetComponent<Building>().owner.Id == 2)
                {
                    b.transform.GetChild(2).GetComponent<MeshRenderer>().material.color = Color.red;
                }
                else
                {
                    b.transform.GetChild(2).GetComponent<MeshRenderer>().material.color = Color.green;
                }
            }
        }
    }
}
