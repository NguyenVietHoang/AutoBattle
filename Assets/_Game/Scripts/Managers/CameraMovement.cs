using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    #region Variables

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private float zoomStep, minCamsize, maxCamsize;

    [SerializeField]
    private SpriteRenderer bgRenderer;
    private float mapMinX, mapMinY, mapMaxX, mapMaxY;

    private Vector3 dragOrigin;

    #endregion

    private void Start()
    {
        var limit = bgRenderer.bounds;
        mapMinX = bgRenderer.transform.position.x - limit.size.x / 2f;
        mapMaxX = bgRenderer.transform.position.x + limit.size.x / 2f;

        mapMinY = bgRenderer.transform.position.y - limit.size.y / 2f;
        mapMaxY = bgRenderer.transform.position.y + limit.size.y / 2f;
    }

    private void Update()
    {
        PanCamera();
    }

    Vector3 ClampCamera(Vector3 targetPosition)
    {
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        float minX = mapMinX + camWidth;
        float maxX = mapMaxX - camWidth;
        float minY = mapMinY + camHeight;
        float maxY = mapMaxY - camHeight;

        float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float newY = Mathf.Clamp(targetPosition.y, minY, maxY);

        return new Vector3(newX, newY, targetPosition.z);
    }

    private void PanCamera()
    {
        if(Input.GetMouseButtonDown(0))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if(Input.GetMouseButton(0))
        {
            Vector3 dif = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);

            cam.transform.position += dif;

            cam.transform.position = ClampCamera(cam.transform.position);
        }
    }

    public void ZoomIn()
    {
        float newSize = cam.orthographicSize - zoomStep;

        cam.orthographicSize = Mathf.Clamp(newSize, minCamsize, maxCamsize);

        cam.transform.position = ClampCamera(cam.transform.position);
    }

    public void ZoomOut()
    {
        float newSize = cam.orthographicSize + zoomStep;

        cam.orthographicSize = Mathf.Clamp(newSize, minCamsize, maxCamsize);

        cam.transform.position = ClampCamera(cam.transform.position);
    }
}
