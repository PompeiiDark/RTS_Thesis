using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelected : MonoBehaviour
{
    public MateiralProperties MateiralProperties;
    public float Intensity = 5.5f;
    public float Sharpness = 4.5f;
    public float changeTime = 0.5f;

    private float counter = 0;
    public bool IsSelected = false;

    private void Start()
    {
        enabled= false;
        this.MateiralProperties.block.SetFloat("_SelectionSharpness", 0);
        this.MateiralProperties.block.SetFloat("_SelectionIntensity", 0);
        this.MateiralProperties.Apply();
    }

    // Update is called once per frame
    //make selected units flash
    void Update()
    {
        counter += Time.deltaTime;
        var lerp = counter / changeTime;
        if (counter > changeTime)
        {
            counter = changeTime;
            lerp = 1;
            enabled = false;
        }
        if (!IsSelected) lerp = 1 - lerp;

        this.MateiralProperties.block.SetFloat("_SelectionSharpness", lerp*Sharpness);
        this.MateiralProperties.block.SetFloat("_SelectionIntensity", lerp*Intensity);
        this.MateiralProperties.Apply();
    }
    public void Select()
    {
        enabled = true;
        counter= 0;
        IsSelected= true;
    }
    public void UnSelect()
    {
        enabled= true;
        counter= 0;
        IsSelected= false; 
    }
}
