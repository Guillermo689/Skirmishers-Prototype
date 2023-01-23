using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeRadomizer : MonoBehaviour
{
    private Vector3 randomSize;
    private Vector3 randomRotation;

    void Start()
    {
        randomSize = new Vector3(1, Random.Range(0.8f, 2f), 1);
        randomRotation = new Vector3(0, Random.Range(0, 180), 0);
        transform.localScale = randomSize;
        transform.localRotation = Quaternion.Euler(randomRotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
