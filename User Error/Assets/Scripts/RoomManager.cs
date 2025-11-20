using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public Camera mainCamera;
    public float cameraMoveSpeed = 2f;
    public Vector3 currentCheckpoint;
    private Vector3 targetCameraPos;
    private bool isMovingCamera;

    void Start()
    {
        targetCameraPos = mainCamera.transform.position;
    }

    // движение камеры в центр комнаты
    public void MoveToRoom(Vector3 roomCenter)
    {
        targetCameraPos = new Vector3(roomCenter.x, roomCenter.y, mainCamera.transform.position.z);
        isMovingCamera = true;
    }

    // установка чекпоинта
    public void SetCheckpoint(Vector3 checkpoint)
    {
        currentCheckpoint = checkpoint;
    }

    // возрождение
    public void Respawn(GameObject player)
    {
        player.transform.position = currentCheckpoint;
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
