using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// partially referenced from the dialogue system tutorial by https://gamedevbeginner.com/dialogue-systems-in-unity/

public class DialogueManager : MonoBehaviour
{
    public Dialogue dialogueObj;
    public TutorialManager tutorialManager;
    public GameObject playerObject;

    [Header("Dialogue Lines")]
    [SerializeField] GameObject dialogueDisplay;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image portraitHolder;

    [Header("Narration Lines")]
    [SerializeField] GameObject narrationDisplay;
    [SerializeField] TextMeshProUGUI narrationText;

    [Header("Displaying Graphics")]
    [SerializeField] GameObject graphicalDisplay;
    [SerializeField] Image graphicHolder;

    int currentLine;
    Player playerScript;


    void Start()
    {
        playerScript = playerObject.GetComponent<Player>();
        currentLine = -1;

        dialogueDisplay.SetActive(false);
        narrationDisplay.SetActive(false);
        graphicalDisplay.SetActive(false);
    }

    void Update()
    {
        // Check for next button (X)
        if (Input.GetButtonDown("Next"))
        {
            // make sure there's actually dialogue being displayed currently
            if (dialogueDisplay.activeSelf || narrationDisplay.activeSelf || graphicalDisplay.activeSelf)
            {
                CallNextLine();
            }
        }
    }

    public DialogueLine GetCurrentLine()
    {
        return dialogueObj.dialogueLines[currentLine];
    }

    public void AddDialoguePause(float pauseLength)
    {
        HideDialogueLines();
        HideNarration();
        HideGraphic();
        StartCoroutine(DialoguePause(pauseLength));
    }

    IEnumerator DialoguePause(float pauseLength)
    {
        yield return new WaitForSeconds(pauseLength);
        NextLine();
    }

    public void ShowDialogueLine(string dialogue, string name, Sprite portrait)
    {
        HideNarration();
        HideGraphic();

        if (!dialogueDisplay.activeSelf)
        {
            dialogueDisplay.SetActive(true);
        }
        if (tutorialManager != null && tutorialManager.CheckPrecondition(Precondition.firstBlockEaten)) // if player actions are enabled
        {
            playerScript.enabled = false; // disable player actions when dialogue is playing
        }

        nameText.text = name;
        dialogueText.text = dialogue;
        portraitHolder.sprite = portrait;
    }

    public void HideDialogueLines()
    {
        if (tutorialManager != null && tutorialManager.CheckPrecondition(Precondition.firstBlockEaten))
        {
            playerScript.enabled = true; // enable player actions when dialogue is done
        }

        nameText.text = null;
        dialogueText.text = null;
        portraitHolder.sprite = null;
        dialogueDisplay.SetActive(false);
    }

    public void ShowNarration(string dialogue)
    {
        HideDialogueLines();

        if (tutorialManager != null && tutorialManager.CheckPrecondition(Precondition.firstBlockEaten))
        {
            playerScript.enabled = false;
        }

        narrationDisplay.SetActive(true);
        narrationText.text = dialogue;
    }

    public void HideNarration()
    {
        if (tutorialManager != null && tutorialManager.CheckPrecondition(Precondition.firstBlockEaten))
        {
            playerScript.enabled = true;
        }

        narrationText.text = null; ;
        narrationDisplay.SetActive(false);
    }

    public void ShowGraphic(Sprite graphic)
    {
        if (tutorialManager != null && tutorialManager.CheckPrecondition(Precondition.firstBlockEaten))
        {
            playerScript.enabled = false;
        }

        graphicalDisplay.SetActive(true);
        graphicHolder.sprite = graphic;
    }

    public void HideGraphic()
    {
        if (tutorialManager != null && tutorialManager.CheckPrecondition(Precondition.firstBlockEaten))
        {
            playerScript.enabled = true;
        }

        graphicHolder.sprite = null;
        graphicalDisplay.SetActive(false);
    }

    // checks if NextLine() can be validly called
    public void CallNextLine()
    {
        currentLine++;

        // checks if there's a tutorial manager (only the tutorial has preconditions) and if precondition is met
        // if precondition is not met, hide dialogue
        if (tutorialManager != null && GetCurrentLine().precondition != Precondition.none && tutorialManager.CheckPrecondition(GetCurrentLine().precondition) == false)
        {
            HideDialogueLines();
            HideNarration();
            HideGraphic();
            currentLine--; // to prevent currentLine from updating

            return;
        }

        if (currentLine > dialogueObj.dialogueLines.Length - 1)
        {
            HideDialogueLines();
        }
        else
        {
            if (GetCurrentLine().pauseLength > 0)
            {
                AddDialoguePause(GetCurrentLine().pauseLength);
            }
            else
            {
                NextLine();
            }
        }
    }

    void NextLine()
    {
        // call function based on type of dialogue (standard line vs narration)
        if (GetCurrentLine().speaker == Speaker.NARRATOR)
        {
            ShowNarration(GetCurrentLine().line);
            ShowGraphic(GetCurrentLine().graphic);
        }
        else
        {
            ShowDialogueLine(GetCurrentLine().line, GetCurrentLine().speaker.ToString(), GetCurrentLine().portrait);
        }

        // check if there's a tutorial manager and if the current line has an event
        if (tutorialManager != null && GetCurrentLine().dialogueEvent != DialogueEvent.none)
        {
            tutorialManager.ToggleEvent(GetCurrentLine().dialogueEvent);
        }
    }
}
