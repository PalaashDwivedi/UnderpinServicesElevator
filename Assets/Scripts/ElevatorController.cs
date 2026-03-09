using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    public Transform[] floors;

    public int currentFloor = 0;

    public float speed = 2f;

    private bool isMoving = false;

    private SortedSet<int> requests = new SortedSet<int>();

    public ElevatorManager manager;

    public TMP_Text floorDisplay;
    public bool IsBusy()
    {
        return requests.Count > 0 || isMoving;
    }

    public int direction = 0;
    // 1 = up
    // -1 = down
    // 0 = idle

    void Start()
    {
        UpdateFloorDisplay();
    }
    void Update()
    {
        UpdateCurrentFloor();

        if (!isMoving && requests.Count > 0)
        {
            int nextFloor = GetNextFloor();
            StartCoroutine(MoveOneFloor(nextFloor));
        }
    }

    void UpdateCurrentFloor()
    {
        float closestDistance = Mathf.Infinity;
        int closestFloor = currentFloor;

        for (int i = 0; i < floors.Length; i++)
        {
            float distance = Mathf.Abs(transform.position.y - floors[i].position.y);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestFloor = i;
            }
        }

        if (closestFloor != currentFloor)
        {
            currentFloor = closestFloor;
            UpdateFloorDisplay();
        }
    }

    void UpdateFloorDisplay()
    {
        if (floorDisplay != null)
        {
            floorDisplay.text = currentFloor.ToString();
        }
    }

    public void AddRequest(int floor)
    {
        if (floor == currentFloor)
        {
            Debug.Log(gameObject.name + " already at floor " + floor);
            return;
        }

        requests.Add(floor);
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

    public bool HasRequest(int floor)
    {
        return requests.Contains(floor);
    }

    IEnumerator MoveOneFloor(int targetFloor)
    {
        isMoving = true;

        int step = (targetFloor > currentFloor) ? 1 : -1;
        direction = step;

        int nextFloor = currentFloor + step;

        Vector3 targetPos = new Vector3(
            transform.position.x,
            floors[nextFloor].position.y + 0.5f,
            transform.position.z
        );

        Debug.Log(gameObject.name + " moving to floor " + nextFloor);

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                speed * Time.deltaTime
            );

            yield return null;
        }

        currentFloor = nextFloor;

        if (requests.Contains(currentFloor))
        {
            Debug.Log(gameObject.name + " stopping at floor " + currentFloor);

            requests.Remove(currentFloor);

            manager.ClearCall(currentFloor, direction);

            yield return new WaitForSeconds(1.0f);
        }

        if (requests.Count == 0)
            direction = 0;

        isMoving = false;
    }
}