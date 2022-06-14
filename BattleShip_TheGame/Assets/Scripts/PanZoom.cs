using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanZoom : MonoBehaviour
{
    [SerializeField] Camera mainCam;

    Vector3 startingMousePos;
    [SerializeField] float minZoom = 1;
    [SerializeField] float maxZoom = 8;

    [SerializeField] SpriteRenderer sprite;
    [SerializeField][Range(5, 10)] float scroolMultiplier = 5f;


    Vector3 cameraPosition;
    Vector3 mousePositionOnScreen;
    Vector3 mousePositionOnScreen1;
    Vector3 camPos1;
    Vector3 mouseOnWorld;
    Vector3 camDragBegin;
    Vector3 camDragNext;



    float mapMinX, mapMaxX, mapMinY, mapMaxY;
    Vector3 difference;
    private void Awake()
    {
        mapMinX = sprite.transform.position.x - sprite.bounds.size.x / 2f;
        mapMaxX = sprite.transform.position.x + sprite.bounds.size.x / 2f;
        mapMinY = sprite.transform.position.y - sprite.bounds.size.y / 2f;
        mapMaxY = sprite.transform.position.y + sprite.bounds.size.y / 2f;
    }

    private void Start()
    {
        cameraPosition = Camera.main.transform.position;
        mousePositionOnScreen = new Vector3();
        mousePositionOnScreen1 = new Vector3();
        camPos1 = new Vector3();
        mouseOnWorld = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startingMousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float differenceBetweenTouch = currentMagnitude - prevMagnitude;

            ZoomCamera();
        }
        else if (Input.GetMouseButton(0))
        {
            difference = startingMousePos - mainCam.ScreenToWorldPoint(Input.mousePosition);
            mainCam.transform.position = ClampCamera(difference + mainCam.transform.position);
        }
        ZoomCamera();
    }

    void ZoomCamera()
    {
        mousePositionOnScreen = mousePositionOnScreen1;
        mousePositionOnScreen1 = Input.mousePosition;
        if (Vector3.Distance(mousePositionOnScreen, mousePositionOnScreen1) == 0)
        {
            float fov = Camera.main.orthographicSize;
            fov -= Input.GetAxis("Mouse ScrollWheel") * scroolMultiplier;
            fov = Mathf.Clamp(fov, minZoom, maxZoom);
            Camera.main.orthographicSize = fov;
            Vector3 mouseOnWorld1 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 posDiff = mouseOnWorld - mouseOnWorld1;
            Vector3 camPos = Camera.main.transform.position;
            mainCam.transform.position = ClampCamera(new Vector3(camPos.x + posDiff.x, camPos.y + posDiff.y, camPos.z));
        }
        else
        {
            mouseOnWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    Vector3 ClampCamera(Vector3 targetPosition)
    {
        float camHeight = mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;

        float minX = mapMinX + camWidth;
        float maxX = mapMaxX - camWidth;
        float minY = mapMinY + camHeight;
        float maxY = mapMaxY - camHeight;

        float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float newY = Mathf.Clamp(targetPosition.y, minY, maxY);

        return new Vector3(newX, newY, targetPosition.z);
    }
}
