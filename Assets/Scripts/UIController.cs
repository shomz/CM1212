using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Canvas HomeCanvas;
    public Canvas TypeCanvas;
    public Canvas GameCanvas;
    public Canvas GameOverCanvas;
    private Canvas[] AllCanvases;
    [SerializeField] private TMP_Text Score;
    [SerializeField] private TMP_Text ScoreChange;
    [SerializeField] private Button ButtonPrefab;
    [SerializeField] private Button ContinueButton;

    private void Start()
    {
        AllCanvases = new Canvas[] { HomeCanvas, TypeCanvas, GameCanvas, GameOverCanvas };
        ScoreChange.gameObject.SetActive(false);
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

    public void UpdateScore((int totalScore, int? scoreChange) newScore)
    {
        if (newScore.scoreChange == null)
        {
            return;
        }

        Score.text = newScore.totalScore.ToString();
        ScoreChange.text = $"+{newScore.scoreChange}";
        ScoreChange.gameObject.SetActive(true);

        StopCoroutine(HideScoreChange());
        StartCoroutine(HideScoreChange());
    }

    private IEnumerator HideScoreChange(int delaySecs = 2)
    {
        yield return new WaitForSeconds(delaySecs);

        ScoreChange.gameObject.SetActive(false);
    }

    public void EnableContinueButton(bool isOn)
    {
        ContinueButton.interactable = isOn;
    }
}
