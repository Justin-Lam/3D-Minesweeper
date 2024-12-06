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

    int currentLine = -1;
    Player playerScript;


    void Start()
    {
        Debug.Log("current line test 1: " + currentLine);
        playerScript = playerObject.GetComponent<Player>();
        Debug.Log("current line test 2: " + currentLine);

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
                Debug.Log("Graphic is being displayed, next line...");
                CallNextLine();
            }
        }
    }

    public DialogueLine GetCurrentLine()
    {
        Debug.Log("current line: " + currentLine);
        return dialogueObj.dialogueLines[currentLine];
    }

    public void SetCurrentLine(int lineNum)
    {
        currentLine = lineNum;
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
        Debug.Log("test test");

        if (narrationDisplay.activeSelf)
        {
            HideNarration();
            HideGraphic();
        }

        if (dialogueDisplay.activeSelf == false)
        {
            Debug.Log("display set active!!!");
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
        HideGraphic();

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
        Debug.Log("called for next line");
        currentLine++;

        if (currentLine > dialogueObj.dialogueLines.Length - 1)
        {
            Debug.Log("case 1");
            HideDialogueLines();
            return;
        }

        // checks if there's a tutorial manager (only the tutorial has preconditions) and if precondition is met
        // if precondition is not met, hide dialogue
        if (tutorialManager != null && GetCurrentLine().precondition != Precondition.none && tutorialManager.CheckPrecondition(GetCurrentLine().precondition) == false)
        {
            Debug.Log("case 2");
            HideDialogueLines();
            HideNarration();
            HideGraphic();
            currentLine--; // to prevent currentLine from updating

            return;
        }

        if (GetCurrentLine().pauseLength > 0)
        {
            Debug.Log("case 3");
            AddDialoguePause(GetCurrentLine().pauseLength);
        }
        else
        {
            Debug.Log("case 4");
            NextLine();
        }
    }

    void NextLine()
    {
        // call function based on type of dialogue (standard line vs narration)
        if (GetCurrentLine().speaker == Speaker.NARRATOR)
        {
            Debug.Log("next line case1");
            ShowNarration(GetCurrentLine().line);
        }
        else
        {
            Debug.Log("next line case2");
            ShowDialogueLine(GetCurrentLine().line, GetCurrentLine().speaker.ToString(), GetCurrentLine().portrait);
        }

        if (GetCurrentLine().graphic != null)
        {
            Debug.Log("next line case3");
            ShowGraphic(GetCurrentLine().graphic);
        }

        // check if there's a tutorial manager and if the current line has an event
        if (tutorialManager != null && GetCurrentLine().dialogueEvent != DialogueEvent.none)
        {
            Debug.Log("next line case4");
            tutorialManager.ToggleEvent(GetCurrentLine().dialogueEvent);
        }
    }
}
