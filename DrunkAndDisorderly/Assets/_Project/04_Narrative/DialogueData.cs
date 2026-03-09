using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "DrunkAndDisorderly/DialogueData")]
public class DialogueData : ScriptableObject
{
    [Header("Basic Info")]
    public string dialogueId;
    public int triggerDay; // в какой день происходит

    [Header("Content")]
    public string speakerName;
    [TextArea(3, 5)] public string[] dialogueLines;
    public Sprite speakerPortrait;

    [Header("Next")]
    public DialogueData nextDialogue;
    public bool isChoice = false;

    [Header("Choices (if isChoice)")]
    public string[] choiceTexts;
    public DialogueData[] choiceNextDialogues;
    public int[] choiceReputationEffects; // влияние на репутацию (если вернём)

    [Header("Events")]
    public string onStartEvent; // событие при начале диалога
    public string onEndEvent; // событие при завершении
}