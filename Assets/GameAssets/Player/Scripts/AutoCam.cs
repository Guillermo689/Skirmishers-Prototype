using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCam : MonoBehaviour
{
    [SerializeField] private Transform[] points;
    [SerializeField] private float smoothSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float waitTime;
    private bool isMoving;
    private Vector3 desiredPosition;
    private int choosePoint;
    private Vector3 newPosition;
    void Start()
    {
        StartCoroutine(CamCounter());
        choosePoint = 0;
    }
    IEnumerator CamCounter()
    {
        yield return new WaitForSeconds(waitTime);
        if (isMoving)
        {
            isMoving = false;
        }
        StartCoroutine(CamCounter());
    }
    private void Update()
    {
        CameraMove();
    }
    private void CameraMove()
    {
        if (!isMoving )
        {
            ChoosePoint();
            isMoving = true;
        }
        else
        {
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
            var targetRotation = Quaternion.LookRotation(points[choosePoint].position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        }
    }
    private void ChoosePoint()
    {
        choosePoint ++;
        if (choosePoint >= points.Length) choosePoint = 0;
        desiredPosition = new Vector3(points[choosePoint].position.x, transform.position.y, points[choosePoint].position.z);



    }
}
