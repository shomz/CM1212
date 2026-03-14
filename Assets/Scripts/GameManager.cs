using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private UIController UI;
    [SerializeField] private ScoreManager ScoreManager;
    [SerializeField] private AudioManager Audio;
    [SerializeField] private InputController InputController;
    [SerializeField] private Transform CardContainer;
    [SerializeField] private CardController CardPrefab;
    [SerializeField] private int CardVariationsCount = 4;
    private List<CardController> Deck;
    private CardController CurrentlySelectedCard;
    private bool AllowedToSelectCards;
    private int Solved;
    private GameState SavedState;
    private GameType CurrentGameType;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var gameTypes = new GameType[]
        {
            new(2, 2),
            new(2, 3),
            new(4, 4),
            new(4, 6),
            new(5, 6)
        };

        UI.ShowCanvas(UI.HomeCanvas);
        UI.BuildGameTypeButtons(gameTypes, StartGame);

        Physics.queriesHitBackfaces = true;
        InputController.OnHitHappened += HandleInput;
    }

    private void HandleInput(RaycastHit hit)
    {
        if (AllowedToSelectCards && hit.collider.gameObject.CompareTag("cardBack"))
        {
            HandleCardClicked(hit.collider.gameObject.GetComponentInParent<CardController>());
        }
    }

    private void HandleCardClicked(CardController card)
    {
        if (card == CurrentlySelectedCard)
        {
            return;
        }

        card.SetFrontFace(true);

        if (CurrentlySelectedCard == null)
        {
            CurrentlySelectedCard = card;
            Audio.PlayFlip();
        }
        else
        {
            if (card.Id != CurrentlySelectedCard.Id)
            {
                Audio.PlayMismatch();
                StartCoroutine(FlipCardsBack(1, new CardController[] { card, CurrentlySelectedCard }));
            }
            else
            {
                Audio.PlayMatch();
                AllowedToSelectCards = true;
                Solved += 2;
            }

            var newScore = ScoreManager.AddScore(card.Id == CurrentlySelectedCard.Id);
            UI.UpdateScore(newScore);
            CurrentlySelectedCard = null;

            CheckGameOver();
        }
    }

    private void CheckGameOver()
    {
        if (Deck.Count == Solved)
        {
            StartCoroutine(ShowCongratulations());
        }
    }

    private IEnumerator ShowCongratulations()
    {
        yield return new WaitForSeconds(2);

        Audio.PlayVictory();
        UI.ShowCanvas(UI.GameOverCanvas);
    }

    public void ShowGameTypes()
    {
        UI.ShowCanvas(UI.TypeCanvas);
    }

    public void StartGame(GameType gameType)
    {
        SavedState = null;
        CurrentGameType = gameType;
        GridSetup(gameType);

        ClearCardContainer();

        Deck = SpawnCards(gameType.rows * gameType.cols);
        Solved = 0;
        SetScale();

        UI.ShowCanvas(UI.GameCanvas);

        StartCoroutine(FlipCardsBack());
    }



    public void LoadGame()
    {
        var gameType = SavedState.GameType;
        GridSetup(gameType);

        ClearCardContainer();

        Deck = SpawnCards(gameType.rows * gameType.cols);
        LoadState();
        Solved = SavedState.Solved;

        SetScale();

        UI.ShowCanvas(UI.GameCanvas);

        var cardsToFlipBack = new List<CardController>();
        for (int i = 0; i < SavedState.CardStates.Length; i++)
        {
            if (SavedState.CardStates[i])
            {
                cardsToFlipBack.Add(Deck[i]);
            }
        }
        StartCoroutine(FlipCardsBack(1, cardsToFlipBack.ToArray()));
    }

    public void EndGame()
    {
        if (Solved != Deck.Count)
        {
            SaveState();
        }
        else
        {
            SavedState = null;
        }

        UI.ShowCanvas(UI.HomeCanvas);
        UI.EnableContinueButton(Solved != Deck.Count);
    }

    private void GridSetup(GameType gameType)
    {
        var grid = CardContainer.GetComponent<GridLayoutGroup>();
        grid.constraintCount = gameType.rows;

        var spacingCoef = 0.2f;
        var rectTransform = CardContainer.GetComponent<RectTransform>();
        var fittingCoef = Mathf.Min(rectTransform.sizeDelta.x / gameType.rows, rectTransform.sizeDelta.y / gameType.cols);

        grid.cellSize = Vector2.one * (fittingCoef * (1 - spacingCoef));
        grid.spacing = Vector2.one * (fittingCoef * spacingCoef);
    }

    private void SaveState()
    {
        var score = ScoreManager.GetScoreAndStreak();
        SavedState = new GameState()
        {
            CardIds = Deck.Select(c => c.Id).ToArray(),
            CardStates = Deck.Select(c => c.GetComponent<Animator>().GetBool("Flipped")).ToArray(),
            Score = score.score,
            Streak = score.streak,
            GameType = CurrentGameType,
            Solved = Solved
        };
    }

    private void LoadState()
    {
        for (int i = 0; i < Deck.Count; i++)
        {
            Deck[i].SetId(SavedState.CardIds[i]);
            Deck[i].SetFrontFace(SavedState.CardStates[i]);
        }

        ScoreManager.SetScoreAndStreak(SavedState.Score, SavedState.Streak);
    }

    private IEnumerator FlipCardsBack(int secsDelay = 2, CardController[] cards = null)
    {
        yield return new WaitForSeconds(secsDelay);

        cards ??= Deck.ToArray();

        foreach (var card in cards)
        {
            card.SetFrontFace(false);
        }

        AllowedToSelectCards = true;

        Audio.PlayFlip();
    }

    private void ClearCardContainer()
    {
        for (int i = CardContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(CardContainer.GetChild(i).gameObject);
        }
    }

    private List<CardController> SpawnCards(int count)
    {
        List<CardController> cards = new();
        var ids = GenerateRandomIds(count);

        for (int i = 0; i < count; i++)
        {
            var card = Instantiate(CardPrefab, CardContainer);

            card.SetId(ids[i]);

            cards.Add(card);
        }

        return cards;
    }

    private List<int> GenerateRandomIds(int count)
    {
        List<int> output = new();
        int cardVariation = 0;
        for (int i = 0; i < count; i += 2)
        {
            output.Add(cardVariation % CardVariationsCount);
            output.Add(cardVariation % CardVariationsCount);
            cardVariation++;
        }

        return output.OrderBy(item => System.Guid.NewGuid()).ToList();
    }

    private void SetScale()
    {
        float scale = CardContainer.GetComponent<GridLayoutGroup>().cellSize.x / 160;
        foreach (var card in Deck)
        {
            card.transform.localScale = Vector3.one * scale;
        }
    }
}

[Serializable]
public class GameState
{
    public int[] CardIds;
    public bool[] CardStates;
    public int Score;
    public int Streak;
    public int Solved;
    public GameType GameType;
}