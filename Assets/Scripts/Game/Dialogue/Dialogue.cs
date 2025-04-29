using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

//This class holds dialogue and is used only by DialogueManager 
public class Dialogue
{
    [SerializeField] List<string> lines;

    public List<string> Lines
    {
        get { return lines; }
    }
}