using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitColor : MonoBehaviour
{
    public int playerNumber;
    public GameObject[] colorParts;
    public Color[] color;
    public GameObject shiny;
    [SerializeField] private float emissionIntensity;
    // Start is called before the first frame update
    void Start()
    {
        ColorCheck();
        ChangeColor();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void ColorCheck()
    {
        color[0] = Color.grey;
        color[1] = Color.red;
        color[2] = Color.blue;
        color[3] = Color.green;
        color[4] = Color.yellow;
        color[5] = Color.magenta;
        color[7] = Color.white;
    }
    public void ChangeColor()
    {
        if (transform.CompareTag("Player"))
        {
            playerNumber = GetComponent<PlayerMain>().playerNumber;
            Renderer renderer = shiny.GetComponent<Renderer>();
            renderer.material.color = color[playerNumber];
            renderer.material.SetColor("_EmissionColor", color[playerNumber] * emissionIntensity);
        }
        if (transform.CompareTag("Unit"))
        {
            playerNumber = GetComponent<UnitMain>().playerNumber;
        }
        foreach (GameObject part in colorParts)
        {
            
                part.GetComponent<Renderer>().material.color = color[playerNumber];

        }
    }
}
