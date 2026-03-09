using UnityEngine;

public class FloorButton : MonoBehaviour
{
    public int floorNumber;

    public int direction;
    // 1 = up
    // -1 = down

    public ElevatorManager manager;

    public void PressButton()
    {
        manager.RequestElevator(floorNumber, direction);
    }
}