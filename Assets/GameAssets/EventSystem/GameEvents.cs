using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEvents : MonoBehaviour
{
    private GameObject player;
    private PlayerMain mainScript;
    private PlayerInput playerInput;
    private InputAction pause;
    private bool isPaused;
    private List<GameObject> enemies = new List<GameObject>();
    GameObject[] units;
    private MixerController mixerController;

    //================= Menu variables ================
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject nextLevelButton;

    [SerializeField] private GameObject pauseMenuESP;
    [SerializeField] private GameObject gameOverMenuESP;
    [SerializeField] private GameObject nextLevelButtonESP;

    public SavedData data;
    public Dialogue dialogue;
    public Dialogue dialogueESP;
    private bool dialogueTriggered = false;

    [SerializeField] private TransitionManager transition;
    private bool GameoverTriggered = false;


    // Start is called before the first frame update
    void Start()
    {
       
        Time.timeScale = 1;
        player = GameObject.FindGameObjectWithTag("Player");
        playerInput = player.GetComponent<PlayerInput>();
        mainScript = player.GetComponent<PlayerMain>();
        pause = playerInput.actions["Pause"];
        units = GameObject.FindGameObjectsWithTag("Unit");
        transition.gameObject.SetActive(true);
        transition.FadeIn();
        EnemyManagement();
        VolumeLoad();
        nextLevelButton.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        nextLevelButtonESP.SetActive(false);
        pauseMenuESP.SetActive(false);
        gameOverMenuESP.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (pause.WasPressedThisFrame()) Pause();
        EnemyManagement();
        if (enemies.Count <= 0) LevelWon();
        if (mainScript.isDead)
        {
            if (!GameoverTriggered)
            {
                Invoke("GameOver", 3);
                GameoverTriggered = true;
            }
        }

    }
   
    private void EnemyManagement()
    {
      
        for (int i = 0; i < units.Length; i++)
        {
            if (units[i] != null)
            {
                UnitMain unit = units[i].GetComponent<UnitMain>();
                if (!unit.isDead)
                {
                    if (unit.playerGroup == 0)
                    {
                        if (!enemies.Contains(unit.gameObject))
                        {
                            enemies.Add(unit.gameObject);
                        }
                    }
                   
                   
                }
                if (unit.isDead)
                {
                    enemies.Remove(units[i]);
                }
            }
            
        }
    }
    private void GameOver()
    {
        Time.timeScale = 0;
        
        if (data.spanish)
        {
            gameOverMenuESP.SetActive(true);
        }
        else
        {
            gameOverMenu.SetActive(true);
        }



    }
    private void Pause()
    {
        if (!isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        if (data.spanish)
        {
            pauseMenuESP.SetActive(true);
        }
        else
        {
            pauseMenu.SetActive(true);
        }
    }
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        if (data.spanish)
        {
            pauseMenuESP.SetActive(false);
        }
        else
        {
            pauseMenu.SetActive(false);
        }
    }
    public void QuitGame()
    {
        Time.timeScale = 1;
        Application.Quit();
    }
    public void MainMenu()
    {
        transition.FadeOut();
        Time.timeScale = 1;
        VolumeSave();
        Invoke("LoadMainMenu", 1);
    }
    private void LoadMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
    public void RestartLevel()
    {
        transition.FadeOut();
        Time.timeScale = 1;
        VolumeSave();
        Invoke("LoadRestartLevel", 1);
    }
    private void LoadRestartLevel()
    {
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void TriggerDialogue()
    {
       if (data.spanish)
        {
            FindObjectOfType<DialogueManager>().StartDialogue(dialogueESP);
        }
       else
        {
            FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        }
    }
    public void LevelWon()
    {
        if (!dialogueTriggered)
        {
            TriggerDialogue();
            dialogueTriggered = true;
           if (data.spanish)
            {
                nextLevelButtonESP.SetActive(true);
            }
           else
            {
                nextLevelButton.SetActive(true);
            }
        }
        
    }
    public void LoadNextLevel()
    {
        transition.FadeOut();
        VolumeSave();
        Invoke("LoadScene", 1f);
    }
    private void LoadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
     private void VolumeSave()
    {
        mixerController = GetComponent<MixerController>();
        if (data.spanish)
        {
            data.masterSlider = mixerController.masterSliderESP.value;
            data.musicSlider = mixerController.musicSliderESP.value;
            data.effectsSlider = mixerController.effectsSliderESP.value;
        }
        else
        {
            data.masterSlider = mixerController.masterSlider.value;
            data.musicSlider = mixerController.musicSlider.value;
            data.effectsSlider = mixerController.effectsSlider.value;
        }
        
    }
    private void VolumeLoad()
    {
       
        mixerController = GetComponent<MixerController>();
        if (data.spanish)
        {
            mixerController.masterSliderESP.value = data.masterSlider;
            mixerController.musicSliderESP.value = data.musicSlider;
            mixerController.effectsSliderESP.value = data.effectsSlider;
        }
        else
        {
            mixerController.masterSlider.value = data.masterSlider;
            mixerController.musicSlider.value = data.musicSlider;
            mixerController.effectsSlider.value = data.effectsSlider;
        }
       
    }

}
