using UnityEngine;
using System.Collections.Generic;

public class ElevatorManager : MonoBehaviour
{
    public ElevatorController[] elevators;

    HashSet<string> activeCalls = new HashSet<string>();

    public void RequestElevator(int floor, int requestDirection)
    {
        string callKey = floor + "_" + requestDirection;
        foreach (ElevatorController elevator in elevators)
        {
            if (elevator.direction == requestDirection)
            {
                if (requestDirection == 1 && elevator.currentFloor <= floor)
                {
                    elevator.AddRequest(floor);
                    Debug.Log("Added intermediate stop to " + elevator.name);
                    return;
                }

                if (requestDirection == -1 && elevator.currentFloor >= floor)
                {
                    elevator.AddRequest(floor);
                    Debug.Log("Added intermediate stop to " + elevator.name);
                    return;
                }
            }
        }

        if (activeCalls.Contains(callKey))
        {
            // Verify an elevator still has this request
            foreach (ElevatorController elevator in elevators)
            {
                if (elevator.HasRequest(floor))
                {
                    Debug.Log("Call already active for floor " + floor);
                    return;
                }
            }
            activeCalls.Remove(callKey);
        }

        activeCalls.Add(callKey);

        ElevatorController bestElevator = null;
        float bestScore = Mathf.Infinity;

        foreach (ElevatorController elevator in elevators)
        {
            float distance = Mathf.Abs(elevator.currentFloor - floor);
            float score = distance;

            // BEST CASE: elevator already moving toward the request
            if (elevator.direction == requestDirection)
            {
                if ((requestDirection == 1 && elevator.currentFloor <= floor) ||
                    (requestDirection == -1 && elevator.currentFloor >= floor))
                {
                    score -= 10f; // strong preference
                }
            }

            // Second preference: idle elevators
            if (!elevator.IsBusy())
            {
                score -= 5f;
            }

            if (score < bestScore)
            {
                bestScore = score;
                bestElevator = elevator;
            }
        }

        if (bestElevator != null)
        {
            bestElevator.AddRequest(floor);

            Debug.Log(
                "Dispatcher chose: " + bestElevator.name +
                " | Request floor: " + floor
            );
        }
    }

    public void ClearCall(int floor, int direction)
    {
        string callKey = floor + "_" + direction;
        activeCalls.Remove(callKey);
    }
}