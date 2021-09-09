using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Tilemaps;

public class ConvoController : MonoBehaviour {
    public UnityEngine.UI.Text DialogText;

    private List<string> LoadedTexts;
    private GameState GameState;

    public void Start() {
        LoadTexts();
        LoadGameState();
        DisplayText();
    }

    public void OnDestroy() {
        SaveGameState();
    }

    public void OnBackToMap() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MapScene");
    }

    public void OnNext() {
        if (GameState.DialogPosition < LoadedTexts.Count - 1) {
            GameState.DialogPosition++;
            DisplayText();
        }
    }

    public void OnPrev() {
        if (GameState.DialogPosition > 0) {
            GameState.DialogPosition--;
            DisplayText();
        }
    }

    private void DisplayText() {
        if (GameState.DialogPosition < LoadedTexts.Count) {
            DialogText.text = LoadedTexts[GameState.DialogPosition];
        }
    }

    private void LoadTexts() {
        LoadedTexts = new List<string>();
        TextAsset textAsset = Resources.Load<TextAsset>("dialog");
        MemoryStream memStream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(textAsset.text));
        StreamReader reader = new StreamReader(memStream);
        string line;
        while ((line = reader.ReadLine()) != null) {
            LoadedTexts.Add(line);
        }
    }

    private void LoadGameState() {
        GameState = SaveManager.Load();
    }

    private void SaveGameState() {
        SaveManager.Save(GameState);
    }
}