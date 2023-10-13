using Cinemachine;
using UnityEngine;

public class CemeraSystem : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private float FOV_Max = 50f;
    [SerializeField] private float FOV_Min = 10f;

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

        //Move with Keyboard
        //if (Input.GetKey(KeyCode.W)) inputDir.z = +1f;
        //if (Input.GetKey(KeyCode.S)) inputDir.z = -1f;
        //if (Input.GetKey(KeyCode.D)) inputDir.x = +1f;
        //if (Input.GetKey(KeyCode.A)) inputDir.x = -1f;


        //Screen Move with Mouse Move
        if (Input.mousePosition.x < edgeScrollSize) inputDir.x = -1f;
        if (Input.mousePosition.y < edgeScrollSize) inputDir.z = -1f;
        if (Input.mousePosition.x > Screen.width - edgeScrollSize) inputDir.x = +1f;
        if (Input.mousePosition.y > Screen.height - edgeScrollSize) inputDir.z = +1f;


        //correct direction
        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
        float moveSpeed = 20f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
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
