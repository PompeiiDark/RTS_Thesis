using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingMenu : MonoBehaviour
{
    public void SetFullScreen(bool isFull)
    {
        Screen.fullScreen= isFull;
    }
    public void SetQuality(int qualityNum)
    {
        QualitySettings.SetQualityLevel(qualityNum);
    }

    public void SetScreenSize(int num)
    {
        if (num == 0)
        {
            Screen.SetResolution(1920, 1080, true);
        }
        else if (num == 1)
        {
            Screen.SetResolution(2560, 1440, true);
        }
        else if (num == 2)
        {
            Screen.SetResolution(3840, 2160, true);
        }
           
                
    }
}
