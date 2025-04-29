using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    //this class holds menu button functions to allow GameController to be notified through events to control game states
    public event Action OnEnterMenu;
    public event Action OnExitMenu;
    [SerializeField] EssentialObject essentialObject;
    [SerializeField] List<GameObject> keys = new List<GameObject>();
    [SerializeField] PuzzleItems puzzleItems;

    public void EnterMenu()
    {
        OnEnterMenu();
    }

    public void ExitMenu()
    {
        OnExitMenu();
    }

    public void ItemsMenu()
    {
        if (puzzleItems.puzzleItems["Ranch Key"])       { keys[0].SetActive(true); }
        if (puzzleItems.puzzleItems["River Key"])       { keys[1].SetActive(true); }
        if (puzzleItems.puzzleItems["Snowy Key"])       { keys[2].SetActive(true); }
        if (puzzleItems.puzzleItems["Bio Key"])         { keys[3].SetActive(true); }
        if (puzzleItems.puzzleItems["Cave Key"])        { keys[4].SetActive(true); }
        if (puzzleItems.puzzleItems["Flower"])          { keys[5].SetActive(true); }
        if (puzzleItems.puzzleItems["Relic Piece 1"])   { keys[6].SetActive(true); }
        if (puzzleItems.puzzleItems["Relic Piece 2"])   { keys[7].SetActive(true); }
        if (puzzleItems.puzzleItems["Relic Piece 3"])   { keys[8].SetActive(true); }
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene(0);
        Destroy(essentialObject.gameObject);
    }
}
