using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceholderBuilding : MonoBehaviour
{
    private Renderer _placeholderChecker;
    public bool isBuilding = false;
    private int _collisionHit = 0;
    // Start is called before the first frame update
    void Start()
    {
        _placeholderChecker = transform.Find("Placeholder_Check").GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if (_collisionHit > 0)
        {
            isBuilding = false;
            _placeholderChecker.material.color = Color.red;
        }
        else
        {
            isBuilding = true;
            _placeholderChecker.material.color = Color.green;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Platform") _collisionHit++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Platform") _collisionHit--;
    }
}
