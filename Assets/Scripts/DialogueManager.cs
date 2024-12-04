using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// based on the dialogue system tutorial by https://gamedevbeginner.com/dialogue-systems-in-unity/
public class DialogueManager : MonoBehaviour
{
    public Dialogue dialogueObj;
    public int currentLine;

    [SerializeField] GameObject dialogueDisplay;
    [SerializeField] GameObject narrationDisplay;
    [SerializeField] GameObject graphicalDisplay;

    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI narrationText;

    [SerializeField] Image portraitHolder;
    [SerializeField] Image graphicHolder;


    // Start is called before the first frame update
    void Start()
    {
        currentLine = -1;
        dialogueDisplay.SetActive(false);
        narrationDisplay.SetActive(false);
        graphicalDisplay.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Check for next button
        if (Input.GetButtonDown("Next"))
        {
            CallNextLine();
        }
    }

    public void AddDialoguePause(float pauseLength)
    {
        EndDialogue();
        HideNarration();
        HideGraphic();
        StartCoroutine(DialoguePause(pauseLength));
    }

    IEnumerator DialoguePause(float pauseLength)
    {
        yield return new WaitForSeconds(pauseLength);
        NextLine();
    }

    public void ShowLine(string dialogue, string name, Sprite portrait)
    {
        HideNarration();
        HideGraphic();
        if (!dialogueDisplay.activeSelf)
        {
            dialogueDisplay.SetActive(true);
        }

        nameText.text = name;
        dialogueText.text = dialogue;
        portraitHolder.sprite = portrait;
    }

    public void EndDialogue()
    {
        nameText.text = null;
        dialogueText.text = null;
        portraitHolder.sprite = null;
        dialogueDisplay.SetActive(false);
    }

    public void ShowNarration(string dialogue)
    {
        EndDialogue();
        narrationDisplay.SetActive(true);
        narrationText.text = dialogue;
    }

    public void HideNarration()
    {
        narrationText.text = null; ;
        narrationDisplay.SetActive(false);
    }

    public void ShowGraphic(Sprite graphic)
    {
        graphicalDisplay.SetActive(true);
        graphicHolder.sprite = graphic;
    }

    public void HideGraphic()
    {
        graphicHolder.sprite = null;
        graphicalDisplay.SetActive(false);
    }

    public void CallNextLine()
    {
        currentLine++;

        if (currentLine > dialogueObj.dialogueLines.Length - 1)
        {
            EndDialogue();
        }
        else
        {
            if (dialogueObj.dialogueLines[currentLine].hasPauseBefore == true)
            {
                AddDialoguePause(dialogueObj.dialogueLines[currentLine].pauseLength);
            }
            else
            {
                NextLine();
            }
        }
    }

    void NextLine()
    {
        if (dialogueObj.dialogueLines[currentLine].speaker.ToString() == "NARRATOR")
        {
            ShowNarration(dialogueObj.dialogueLines[currentLine].line);
            ShowGraphic(dialogueObj.dialogueLines[currentLine].graphic);
        }
        else
        {
            ShowLine(dialogueObj.dialogueLines[currentLine].line, dialogueObj.dialogueLines[currentLine].speaker.ToString(), dialogueObj.dialogueLines[currentLine].portrait);
        }
    }
}
