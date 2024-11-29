using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Based off this dialogue system tutorial: https://gamedevbeginner.com/dialogue-systems-in-unity/

[CreateAssetMenu]
public class Dialogue : ScriptableObject
{
    [TextArea]
    public string[] dialogue;
}
