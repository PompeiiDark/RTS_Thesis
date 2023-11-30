using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    private bool IsPause = false;

    public GameObject EndUI;
    public Text EndMessage;

    public GameObject canvasObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !IsPause)
        {
            EnablePauseMenu();
            IsPause = true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && IsPause)
        {
            DisablePauseMenu();
            IsPause = false;
        }

        //CheckWin();
        //CheckLose();
    }

    private void EnablePauseMenu()
    {
        Time.timeScale = 0;
        canvasObject.SetActive(true);
        canvasObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(true);
        canvasObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.SetActive(false);
    }
    private void DisablePauseMenu()
    {
        Time.timeScale = 1;
        canvasObject.SetActive(false);
    }

    public void OnSettingButton()
    {
        canvasObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(false);
        canvasObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.SetActive(true);
    }
    public void OnMainMenuButton()
    {
        SceneManager.LoadScene(0);
    }
    public void OnExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//Quit running in the Unity3D editor
#else
#endif
        Application.Quit();//Quit in .exe

    }


    public void CheckWin()
    {
        int Num = 0;
        foreach (var b in BuildingManager.instance.enemysObject)
        {
            if (b == null)
            {
                BuildingManager.instance.enemysObject = BuildingManager.instance.enemysObject.FindAll(x => x != null);
            }
            else if (b.GetComponent<Base>())
            {
                Num++;
            }
        }
        foreach (var u in GameControllor.instance.enemysObject)
        {
            if (u == null)
            {
                GameControllor.instance.enemysObject = GameControllor.instance.enemysObject.FindAll(x => x != null);
            }
            else if (u.GetComponent<Drone>())
            {
                Num++;
            }
        }
        Debug.Log("checkwin num:" + Num);
        if (Num == 1)
        {
            //win
            Time.timeScale = 0;
            EndUI.SetActive(true);
            EndMessage.text += "Win!!!";
        }
    }
    public void CheckLose()
    {
        int Num = 0;
        foreach (var b in BuildingManager.instance.buildings)
        {
            if (b == null)
            {
                BuildingManager.instance.buildings = BuildingManager.instance.buildings.FindAll(x => x != null);
            }
            else if (b.GetComponent<Base>())
            {
                Num++;
            }
        }
        foreach (var u in GameControllor.instance.existingunit)
        {
            if (u == null)
            {
                GameControllor.instance.existingunit = GameControllor.instance.existingunit.FindAll(x => x != null);
            }
            else if (u.GetComponent<Drone>())
            {
                Num++;
            }
        }
        Debug.Log("checklose num:" + Num);
        if (Num == 1)
        {
            //lose
            Time.timeScale = 0;
            EndUI.SetActive(true);
            EndMessage.text += "Failed..";
        }
    }
}
