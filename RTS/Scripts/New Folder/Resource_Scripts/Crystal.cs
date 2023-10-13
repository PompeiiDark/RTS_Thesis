using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    public int totalCrystalValue = 450;//total value of each crystal object
    public int collectionValue = 10;//each time collect value

    int currentCrystalValue;
    // Start is called before the first frame update
    void Start()
    {
        currentCrystalValue = totalCrystalValue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool CollectResource()
    {
        if (currentCrystalValue <= 0)
        {
            return false;
        }
        int collectedValue = Mathf.Min(collectionValue, currentCrystalValue);
        currentCrystalValue -= collectedValue;

        if (currentCrystalValue <= 0)
        {
            Destroy(gameObject);
        }
        return true;
    }
}
