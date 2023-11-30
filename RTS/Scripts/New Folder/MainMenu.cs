using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject startMenu;
    public GameObject settingMenu;

    public Dropdown DifficultyDropdown;
    public Dropdown QuantityDropdown;

    public int Difficulty;
    public int Quantity;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        mainMenu.SetActive(true);
        startMenu.SetActive(false);
        settingMenu.SetActive(false);
    }
    public void OnStartButtonClick()//go to game detail select
    {
        mainMenu.SetActive(false);
        startMenu.SetActive(true);
    }
    public void OnSettingButtonClick()
    {
        mainMenu.SetActive(false);
        settingMenu.SetActive(true);
    }
    public void OnBackButtonClick()
    {
        settingMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
    public void OnExitButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//Quit running in the Unity3D editor
#else
#endif
        Application.Quit();//Quit in .exe
    }



    public void OnDetailStartButtonClick()
    {
        Difficulty = DifficultyDropdown.value;
        Quantity = QuantityDropdown.value;
        SceneManager.LoadScene(1);
    }
}
