using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitShenn : MonoBehaviour
{
    //change the material property of Shader when it is not selected
    public MateiralProperties MateiralProperties;
    public Infomation Infomation;

    public float Intensity = 5.5f;
    public float Sharpness = 4.5f;
    // Start is called before the first frame update
    void Start()
    {
        this.MateiralProperties.block.SetColor("_SheenColor", this.Infomation.player.color);
        this.MateiralProperties.block.SetFloat("_SheenSharpness", Sharpness);
        this.MateiralProperties.block.SetFloat("_SheenIntensity", Intensity);
        this.MateiralProperties.Apply();
    }

}
