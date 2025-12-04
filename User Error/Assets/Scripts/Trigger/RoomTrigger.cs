using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public RoomManager roomManager;
    public Transform targetRoomCenter;
    public Transform currentCheckpoint;
    public GameObject barrier;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            roomManager.MoveToRoom(targetRoomCenter.position);
            roomManager.SetCheckpoint(currentCheckpoint.position);
            BoxCollider2D boxCollider2D = barrier.gameObject.GetComponent<BoxCollider2D>();
            boxCollider2D.isTrigger = false;
        }
    }
}
