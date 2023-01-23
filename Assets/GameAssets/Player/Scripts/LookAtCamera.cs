using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform mainCamera;
    private Transform player;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 rotationOffset;
    [SerializeField] private float scaleAmount;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main.transform.parent;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        
        transform.position = player.position + offset;
        transform.localRotation = mainCamera.rotation * Quaternion.Euler(rotationOffset);

    }
}
