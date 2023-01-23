using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smoothSpeed = 0.5f;
    [SerializeField] private float rotationSpeed;
        [SerializeField] private float zoomSpeed;

    private float maxHeight = 20f;
    private float minHeight = 4f;
    float xRotation;
    
  

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Transform cameraChild = transform.GetChild(0);
        
    }

    void FixedUpdate()
    {
        FollowPlayer();
        CameraRotarion();
    }

    private void CameraRotarion()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
           // p1 = Mouse.current.position.ReadValue();

        }
        if (Mouse.current.rightButton.isPressed)
        {
           
            Cursor.lockState = CursorLockMode.Locked;
            float mouseX = Mouse.current.delta.ReadValue().x * rotationSpeed * Time.deltaTime;
            float mouseY = Mouse.current.delta.ReadValue().y * rotationSpeed * Time.deltaTime;
            Transform cameraChild = transform.GetChild(0);
            transform.Rotate( Vector3.up * mouseX);
            
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -120f, 25f);
            cameraChild.transform.localRotation = Quaternion.Euler(100 +  xRotation, 0, 0);
            
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
        offset.y -= Mouse.current.scroll.ReadValue().y  * zoomSpeed * Time.deltaTime;
        if ((offset.y >= maxHeight))
        {
            offset.y = maxHeight;
        }
        else if ((offset.y <= minHeight))
        {
            offset.y = minHeight;
        }
    }
    private void FollowPlayer()
    {
        Vector3 desiredPosition = player.transform.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
      
    }
}
