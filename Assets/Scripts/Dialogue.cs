using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// partially referenced from the dialogue system tutorial by https://gamedevbeginner.com/dialogue-systems-in-unity/

[CreateAssetMenu]
public class Dialogue : ScriptableObject
{
    public DialogueLine[] dialogueLines;
}

public enum Speaker { SHEEP, CROW, NARRATOR };
public enum Precondition { none, firstBlockEaten, secondBlockEaten, firstMineEaten, firstFlag, mineEaten, win }; // what is needed for the line to trigger
public enum DialogueEvent { none, allowEat, allowMove, allowFlag, explode, endTutorial }; // what triggers after the line

[System.Serializable]
public class DialogueLine
{
    public Speaker speaker;

    public Sprite portrait;
    public Sprite graphic;

    public float pauseLength; // how long to pause for before the dialogue is displayed

    public Precondition precondition = Precondition.none;
    public DialogueEvent dialogueEvent = DialogueEvent.none;

    [TextArea]
    public string line;
}

