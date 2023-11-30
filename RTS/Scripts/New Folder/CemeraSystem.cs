using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class CemeraSystem : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private float FOV_Max = 85f;
    [SerializeField] private float FOV_Min = 10f;

    [SerializeField] private float maxX = 55f;
    [SerializeField] private float maxZ = 75f;
    [SerializeField] private float minX = -95f;
    [SerializeField] private float minZ = -75f;

    private float targetFOV = 50;
    // Update is called once per frame
    void Update()
    {
        HandleCameraRotation();
        HandleCameraMove();
        HandleCameraZoom();
    }
    private void HandleCameraMove()
    {
        Vector3 inputDir = new Vector3(0, 0, 0);
        int edgeScrollSize = 15;

        //Screen Move with Mouse Move
        if (Input.mousePosition.x < edgeScrollSize) inputDir.x = -1f;
        if (Input.mousePosition.y < edgeScrollSize) inputDir.z = -1f;
        if (Input.mousePosition.x > Screen.width - edgeScrollSize) inputDir.x = +1f;
        if (Input.mousePosition.y > Screen.height - edgeScrollSize) inputDir.z = +1f;


        //correct direction
        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
        float moveSpeed = 20f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
        if (transform.position.x > maxX)
        {
            transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < minX)
        {
            transform.position = new Vector3(minX, transform.position.y, transform.position.z);
        }

        if (transform.position.z > maxZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, maxZ);
        }
        else if (transform.position.z < minZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, minZ);
        }

    }
    private void HandleCameraRotation()
    {
        float rotateDir = 0f;
        float rotateSpeed = 120f;
        if (Input.GetKey(KeyCode.Q)) rotateDir = +1f;
        if (Input.GetKey(KeyCode.E)) rotateDir = -1f;
        transform.eulerAngles += new Vector3(0, rotateDir * rotateSpeed * Time.deltaTime, 0);
    }
    private void HandleCameraZoom()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            targetFOV -= 5;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            targetFOV += 5;
        }
        targetFOV = Mathf.Clamp(targetFOV,FOV_Min,FOV_Max);//give a range of FOV

        float zoomSpeed = 10f;
        cinemachineVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(cinemachineVirtualCamera.m_Lens.FieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
    }
}
