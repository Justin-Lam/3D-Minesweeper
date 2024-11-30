using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [TextArea]
    public string line;
    public enum Speaker { SHEEP, CROW, NARRATOR };
    public Speaker speaker;
}
