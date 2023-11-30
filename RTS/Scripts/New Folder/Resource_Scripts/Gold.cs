using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : MonoBehaviour
{
    public int totalGoldValue = 450;//total value of each object
    public int collectionValue = 10;//each time collect value
    [SerializeField]
    int currentGoldValue;
    // Start is called before the first frame update
    void Start()
    {
        currentGoldValue = totalGoldValue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool CollectResource()
    {
        if (currentGoldValue <= 0)
        {
            return false;
        }
        int collectedValue = Mathf.Min(collectionValue, currentGoldValue);
        currentGoldValue -= collectedValue;

        if (currentGoldValue <= 0)
        {
            Destroy(gameObject);
        }
        return true;
    }
}
