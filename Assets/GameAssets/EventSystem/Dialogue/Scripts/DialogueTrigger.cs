using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public Dialogue dialogueESP;
    private bool dialogueTriggered = false;
    private bool exitDialogueTriggered = false;
    [SerializeField] private Dialogue exitDialogue;
    [SerializeField] private Dialogue exitDialogueESP;
    [SerializeField] private bool hasExitDialogue;
    private GameEvents gameEvents;
    [SerializeField] private float unitRadius;
    [SerializeField] private GameObject player;
    private void Start()
    {
        gameEvents = FindObjectOfType<GameEvents>();
    }
    private void Update()
    {
        CheckPlayer();
    }
    public void TriggerDialogue()
    {
        if (gameEvents.data.spanish)
        {
            FindObjectOfType<DialogueManager>().StartDialogue(dialogueESP);
        }
        else
        {
            FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        }
    }
    public void TriggerExitDialogue()
    {
        if (gameEvents.data.spanish)
        {
            FindObjectOfType<DialogueManager>().StartDialogue(exitDialogueESP);
        }
       else
        {
            FindObjectOfType<DialogueManager>().StartDialogue(exitDialogue);
        }
    }
    private void CheckPlayer()
    {
        Collider[] units = Physics.OverlapSphere(transform.position, unitRadius);
        foreach(Collider unit in units)
        {
            if (unit.CompareTag("Player"))
            {
                //======== Check if player is local when network behaviour
                //if (unit.isLocal)
                // {
                player = unit.gameObject;
                if (!dialogueTriggered)
                {
                    TriggerDialogue();
                    dialogueTriggered = true;
                }
                // }
            }
        }
        if (player != null && dialogueTriggered)
        {
            if (Vector3.Distance(transform.position, player.transform.position) > unitRadius +1 && Vector3.Distance(transform.position, player.transform.position) < unitRadius + 1.2f)
            {
                if (hasExitDialogue)
                {
                    if (player.GetComponent<PlayerMain>().commandScript.allUnits.Count <= 0)
                    {
                        if (!exitDialogueTriggered)
                        {
                            TriggerExitDialogue();
                            exitDialogueTriggered = true;
                            //StartCoroutine(ResetExitDialogue());
                        }
                    }
                }
            }
        }
        
    }
   
    IEnumerator ResetExitDialogue()
    {
        yield return new WaitForSeconds(1f);
        exitDialogueTriggered = false;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, unitRadius);
        
    }
}
