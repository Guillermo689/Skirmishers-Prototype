using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TransitionManager transition;
    [SerializeField] private SavedData savedData;
    [SerializeField] private GameObject[] ESPText;
    [SerializeField] private GameObject[] ENGText;
    public Texture2D cursorHand;
    void Start()
    {
        CursorHand();
        transition.gameObject.SetActive(true);
        transition.FadeIn();
    }
    private void CursorHand()
    {
        Cursor.SetCursor(cursorHand, Vector2.zero, CursorMode.Auto);
    }
    // Update is called once per frame
    void Update()
    {
        SpanishToggle();
    }
    private void SpanishToggle()
    {
        if (savedData.spanish)
        {
            foreach (GameObject text in ESPText)
            {
                text.SetActive(true);
            }
            foreach (GameObject text in ENGText)
            {
                text.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject text in ESPText)
            {
                text.SetActive(false);
            }
            foreach (GameObject text in ENGText)
            {
                text.SetActive(true);
            }
        }
    }
    public void Quit()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        transition.FadeOut();
        Invoke("LoadLevel", 1f);
;    }
    private void LoadLevel()
    {
        SceneManager.LoadScene(1);
    }
    public void Spanish()
    {
        savedData.spanish = true;
    }
    public void English()
    {
        savedData.spanish = false;
    }
}
