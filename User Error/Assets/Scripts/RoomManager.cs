using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public Camera mainCamera;
    public float cameraMoveSpeed = 2f;
    private Vector3 targetCameraPos;
    private bool isMovingCamera;

    void Start()
    {
        targetCameraPos = mainCamera.transform.position;
    }

    public void MoveToRoom(Vector3 roomCenter)
    {
        targetCameraPos = new Vector3(roomCenter.x, roomCenter.y, mainCamera.transform.position.z);
        isMovingCamera = true;
    }

    void Update()
    {
        if (isMovingCamera)
        {
            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position,
                targetCameraPos,
                cameraMoveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(mainCamera.transform.position, targetCameraPos) < 0.1f)
            {
                isMovingCamera = false;
            }
        }
    }
}
