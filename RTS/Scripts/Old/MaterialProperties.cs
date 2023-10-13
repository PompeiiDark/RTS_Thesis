using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MateiralProperties : MonoBehaviour
{
    //Apply change the single unit's material property
    public MaterialPropertyBlock block;
    public Renderer Renderer;
    private void Awake()
    {
        block = new MaterialPropertyBlock();
    }
    public void Apply()
    {
        this.Renderer.SetPropertyBlock(block);
    }



}
