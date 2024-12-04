using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Based off this dialogue system tutorial: https://gamedevbeginner.com/dialogue-systems-in-unity/


[CreateAssetMenu]
public class Dialogue : ScriptableObject
{
    public DialogueLine[] dialogueLines;
}

[System.Serializable]
public class DialogueLine
{
    public enum Speaker { SHEEP, CROW, NARRATOR };
    public Speaker speaker;

    public Sprite portrait;
    public Sprite graphic;

    public bool hasPauseBefore = false;
    public float pauseLength;

    [TextArea]
    public string line;
}

