using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Canvas HomeCanvas;
    public Canvas TypeCanvas;
    public Canvas GameCanvas;
    private Canvas[] AllCanvases;
    [SerializeField] private Button ButtonPrefab;

    private void Start()
    {
        AllCanvases = new Canvas[] { HomeCanvas, TypeCanvas, GameCanvas };
    }

    public void ShowCanvas(Canvas canvas)
    {
        foreach (var c in AllCanvases)
        {
            c.gameObject.SetActive(c == canvas);
        }
    }

    public void BuildGameTypeButtons(GameType[] gameTypes, Action<GameType> startGame)
    {
        foreach (var gameType in gameTypes)
        {
            var button = Instantiate(ButtonPrefab, TypeCanvas.transform);
            var text = button.GetComponentInChildren<TMP_Text>();
            text.text = $"{gameType.rows}X{gameType.cols}";

            button.onClick.AddListener(() => startGame(gameType));
        }
    }
}
