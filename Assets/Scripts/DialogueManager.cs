using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public Dialogue dialogue;

    private bool lineFinished = true;

    // Start is called before the first frame update
    void Start()
    {
        // start with first piece of dialogue in the Dialogue object, then whenever a button is pressed the next piece of dialogue is played
        // dialogue objects have elements, start from 0 and go from there
    }

    // Update is called once per frame
    void Update()
    {
        // Check for next button
        if (Input.GetButtonDown("Next") && lineFinished)
        {
            NextLine();
        }
        else if (Input.GetButtonDown("Next") && !lineFinished)
        {
            SkipLine();
        }
    }

    void SkipLine()
    {
        Debug.Log("skipped!");
    }

    void NextLine()
    {
        Debug.Log("next!");
    }
}
