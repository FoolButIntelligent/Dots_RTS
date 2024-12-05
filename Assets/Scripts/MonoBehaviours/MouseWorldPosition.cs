using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class MouseWorldPosition : ThreadSafeSingleton<MouseWorldPosition>
{
    public Vector3 GetPosition()
    {
        Ray mouseCameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        //基于物理检测射线是否与物体相交
        // if (Physics.Raycast(mouseCameraRay, out RaycastHit hit))
        // {
        //     return hit.point;
        // }
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        
        //基于数学检测射线是否与平面相交
        if (plane.Raycast(mouseCameraRay, out float distance))
        {
            return mouseCameraRay.GetPoint(distance);
        }
        else
        {
            return Vector3.zero;
        }
    }
}
