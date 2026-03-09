using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    public Transform[] floors;

    public int currentFloor = 0;

    public float speed = 2f;

    private bool isMoving = false;

    private SortedSet<int> requests = new SortedSet<int>();

    private int direction = 0;
    // 1 = up
    // -1 = down
    // 0 = idle

    void Update()
    {
        if (!isMoving && requests.Count > 0)
        {
            int nextFloor = GetNextFloor();
            StartCoroutine(MoveToFloor(nextFloor));
        }
    }

    public void AddRequest(int floor)
    {
        requests.Add(floor);

        if (direction == 0)
        {
            if (floor > currentFloor)
                direction = 1;
            else if (floor < currentFloor)
                direction = -1;
        }
    }

    int GetNextFloor()
    {
        if (direction >= 0)
        {
            foreach (int floor in requests)
            {
                if (floor >= currentFloor)
                    return floor;
            }

            direction = -1;
        }

        if (direction <= 0)
        {
            foreach (int floor in requests)
            {
                if (floor <= currentFloor)
                    return floor;
            }

            direction = 1;
        }

        return currentFloor;
    }

    IEnumerator MoveToFloor(int targetFloor)
    {
        isMoving = true;

        Vector3 targetPos = new Vector3(
        transform.position.x,
        floors[targetFloor].position.y,
        transform.position.z
        );

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                speed * Time.deltaTime
            );

            yield return null;
        }

        currentFloor = targetFloor;

        requests.Remove(targetFloor);

        if (requests.Count == 0)
            direction = 0;

        isMoving = false;
    }
}