using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    public float dragSpeed = 2f;
    public float minX = -20f;
    public float maxX = 20f;
    public float minY = -20f;
    public float maxY = 20f;

    private Vector3 dragOrigin;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0))
            return;

        Vector3 currentMouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 originMouseWorldPos = Camera.main.ScreenToWorldPoint(dragOrigin);
        Vector3 difference = originMouseWorldPos - currentMouseWorldPos;
        Vector3 newPos = transform.position + difference;
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
        newPos.z = transform.position.z;
        transform.position = newPos;
        dragOrigin = Input.mousePosition;
    }
}