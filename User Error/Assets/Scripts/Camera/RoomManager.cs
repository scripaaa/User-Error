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
    private GameObject[] roomCenters;

    void Start()
    {
        targetCameraPos = mainCamera.transform.position;
        roomCenters = GameObject.FindGameObjectsWithTag("RoomCenter");
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

    // ћгновенное перемещение камеры к позиции
    public void SnapCameraToPosition(Vector3 playerPosition)
    {
        // Ќаходим ближайший центр комнаты к позиции игрока
        GameObject nearestRoom = FindNearestRoomCenter(playerPosition);

        if (nearestRoom != null)
        {
            targetCameraPos = new Vector3(nearestRoom.transform.position.x, nearestRoom.transform.position.y, mainCamera.transform.position.z);
            mainCamera.transform.position = targetCameraPos;
        }
        else
        {
            // ≈сли не нашли центры, используем позицию игрока
            targetCameraPos = new Vector3(playerPosition.x, playerPosition.y, mainCamera.transform.position.z);
            mainCamera.transform.position = targetCameraPos;
        }

        isMovingCamera = false;
        StopAllCoroutines();
    }

    // Ќаходит ближайший центр комнаты к заданной позиции
    private GameObject FindNearestRoomCenter(Vector3 position)
    {
        GameObject nearest = null;
        float minDistance = Mathf.Infinity;

        // ≈сли еще не нашли центры комнат, ищем их
        if (roomCenters == null || roomCenters.Length == 0)
        {
            roomCenters = GameObject.FindGameObjectsWithTag("RoomCenter");
        }

        if (roomCenters == null || roomCenters.Length == 0)
        {
            return null;
        }

        foreach (GameObject roomCenter in roomCenters)
        {
            if (roomCenter == null) continue;

            float distance = Vector3.Distance(position, roomCenter.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = roomCenter;
            }
        }

        return nearest;
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