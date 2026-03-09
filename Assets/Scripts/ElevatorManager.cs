using UnityEngine;

public class ElevatorManager : MonoBehaviour
{
    public ElevatorController[] elevators;

    public void RequestElevator(int floor, int direction)
    {
        ElevatorController bestElevator = null;

        float bestScore = Mathf.Infinity;

        foreach (ElevatorController elevator in elevators)
        {
            float distance = Mathf.Abs(elevator.currentFloor - floor);

            if (distance < bestScore)
            {
                bestScore = distance;
                bestElevator = elevator;
            }
        }

        if (bestElevator != null)
        {
            bestElevator.AddRequest(floor);
        }
    }
}