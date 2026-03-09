using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Settings")]
    public float textSpeed = 0.05f;

    [Header("Current Dialogue")]
    public DialogueData currentDialogue;
    private int currentLineIndex = 0;
    private bool isDialogueActive = false;
    private Coroutine typingCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartDialogue(DialogueData dialogue)
    {
        if (dialogue == null) return;

        currentDialogue = dialogue;
        currentLineIndex = 0;
        isDialogueActive = true;

        UIManager.Instance?.ShowDialogue(dialogue.speakerName, "", dialogue.speakerPortrait);

        ShowNextLine();
    }

    public void ShowNextLine()
    {
        if (!isDialogueActive) return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        if (currentLineIndex < currentDialogue.dialogueLines.Length)
        {
            string line = currentDialogue.dialogueLines[currentLineIndex];
            typingCoroutine = StartCoroutine(TypeText(line));
            currentLineIndex++;
        }
        else
        {
            EndDialogue();
        }
    }

    private IEnumerator TypeText(string text)
    {
        UIManager.Instance.dialogueContentText.text = "";

        foreach (char c in text)
        {
            UIManager.Instance.dialogueContentText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    private void EndDialogue()
    {
        isDialogueActive = false;

        if (currentDialogue.nextDialogue != null)
        {
            StartDialogue(currentDialogue.nextDialogue);
        }
        else
        {
            UIManager.Instance?.HideDialogue();
        }
    }

    private void Update()
    {
        if (isDialogueActive && Input.GetMouseButtonDown(0))
        {
            ShowNextLine();
        }
    }
}