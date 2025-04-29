using System.Collections.Generic;
using UnityEngine;

public class PuzzleItems : MonoBehaviour
{
    public Dictionary<string, bool> puzzleItems { get; private set; }

    private void Awake()
    {
        if (puzzleItems == null)
        {
            Debug.Log("Initializing Puzzle Items Dictionary");
            puzzleItems = new Dictionary<string, bool>();

            puzzleItems.Add("Ranch Key", true);
            puzzleItems.Add("River Key", true);
            puzzleItems.Add("Snowy Key", true);
            puzzleItems.Add("Bio Key", true);
            puzzleItems.Add("Cave Key", true);
            puzzleItems.Add("Relic Piece 1", false);
            puzzleItems.Add("Relic Piece 2", false);
            puzzleItems.Add("Relic Piece 3", false);
            puzzleItems.Add("Flower", false);
        }
        else Debug.Log("Puzzle items dictionary already initialized.");
    }

    public void GiveRanchKey()
    {
        puzzleItems["Ranch Key"] = true;
    }

    public void GiveRiverKey()
    {
        puzzleItems["River Key"] = true;
    }

    public void GiveSnowyKey()
    {
        puzzleItems["Snowy Key"] = true;
    }

    public void GiveBioKey()
    {
        puzzleItems["Bio Key"] = true;
    }

    public void GiveCaveKey()
    {
        puzzleItems["Cave Key"] = true;
    }

    public void GiveRelicPiece1()
    {
        puzzleItems["Relic Piece 1"] = true;
    }

    public void GiveRelicPiece2()
    {
        puzzleItems["Relic Piece 2"] = true;
    }

    public void GiveRelicPiece3()
    {
        puzzleItems["Relic Piece 3"] = true;
    }

    public void GiveFlowerItem()
    {
        puzzleItems["Flower"] = true;
    }
}
