using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// based on the dialogue system tutorial by https://gamedevbeginner.com/dialogue-systems-in-unity/
public class DialogueManager : MonoBehaviour
{
    public Dialogue dialogueObj;
    public int currentLine;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] GameObject dialogueDisplay;

    // private int currentLine;


    // Start is called before the first frame update
    void Start()
    {
        currentLine = -1;
    }

    // Update is called once per frame
    void Update()
    {
        // Check for next button
        if (Input.GetButtonDown("Next"))
        {
            NextLine();
        }
    }

    public void AddDialoguePause(float pauseLength)
    {
        EndDialogue();
        StartCoroutine(DialoguePause(pauseLength));
    }

    IEnumerator DialoguePause(float pauseLength)
    {
        yield return new WaitForSeconds(pauseLength);
        NextLine();
    }

    public void ShowLine(string dialogue, string name)
    {
        if (!dialogueDisplay.activeSelf)
        {
            dialogueDisplay.SetActive(true);
        }
        nameText.text = name;
        dialogueText.text = dialogue;
    }

    public void EndDialogue()
    {
        nameText.text = null;
        dialogueText.text = null; ;
        dialogueDisplay.SetActive(false);
    }

    public void NextLine()
    {
        currentLine++;
        if (currentLine > dialogueObj.dialogueLines.Length - 1)
        {
            EndDialogue();
        }
        else
        {
            ShowLine(dialogueObj.dialogueLines[currentLine], dialogueObj.speakers[currentLine].ToString());
        }
    }
}