using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text continueText;

    [SerializeField] private Animator animator;
    private GameEvents gameEvents;
    private int isOpenID;
    void Start()
    {
        sentences = new Queue<string>();
        gameEvents = FindObjectOfType<GameEvents>();
        isOpenID = Animator.StringToHash("IsOpen");
        if (gameEvents.data.spanish)
        {
            continueText.text = "Continuar";
        }
        else
        {
            continueText.text = "Continue";
        }
    }

   public void StartDialogue(Dialogue dialogue)
    {
        animator.SetBool(isOpenID, true);
        nameText.text = dialogue.name;
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    void EndDialogue()
    {
        animator.SetBool(isOpenID, false);
    }
}
