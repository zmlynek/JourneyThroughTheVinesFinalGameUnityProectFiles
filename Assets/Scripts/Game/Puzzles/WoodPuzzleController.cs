using System.Collections.Generic;
using UnityEngine;

public class WoodPuzzleController : MonoBehaviour
{
    private int numWood = 10;
    [SerializeField] List<WoodPiece> woodPieces = new List<WoodPiece>();
    [SerializeField] LoneVillagerNPC loneVillager;
    private void Update()
    {
        foreach (var piece in woodPieces.ToArray())
        {
            if (!piece.gameObject.activeSelf)
            {
                woodPieces.Remove(piece);
                numWood = woodPieces.Count;
                Debug.Log(numWood);
            }
        }

        if (numWood == 0)
        {
            loneVillager.FinishPuzzle();
        }
    }
}
