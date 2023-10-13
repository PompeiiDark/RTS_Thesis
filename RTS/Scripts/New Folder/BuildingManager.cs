using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _placeholderBuilding;
    private GameObject _placeholder;

    [SerializeField]
    private GameObject _building;

    private Vector3 _mousePosition;
    private float _previousX;
    private float _previousY;
    private float _previousZ;

    private Building _buildingScript;
    // Start is called before the first frame update
    void Start()
    {
        _placeholder = Instantiate(_placeholderBuilding);//move those to drone build UI clicked
        _buildingScript = _placeholder.GetComponent<Building>();
    }

    // Update is called once per frame
    void Update()
    {
        _mousePosition = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(_mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            float positionX = hit.point.x;
            float positionZ = hit.point.z;
            float positionY = hit.point.y;

            if (_previousX != positionX || _previousZ != positionZ || _previousY != positionY)
            {
                _previousX = positionX;
                _previousZ = positionZ;
                _previousY = positionY;

                _placeholder.transform.position = new Vector3(positionX, positionY, positionZ);//placeholder always at mouse pos
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (_buildingScript.isBuilding)
                {
                    Instantiate(_building, _placeholder.transform.position, Quaternion.identity);//create the building
                }
            }
        }
    }
}
