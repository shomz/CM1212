using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private UIController UI;
    [SerializeField] private Transform CardContainer;
    [SerializeField] private CardController CardPrefab;
    [SerializeField] private int CardVariationsCount = 4;
    private List<CardController> deck;
    private CardController currentlySelectedCard;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var gameTypes = new GameType[]
        {
            new(2, 2),
            new(4, 4),
            new(5, 6)
        };

        UI.ShowCanvas(UI.HomeCanvas);
        UI.BuildGameTypeButtons(gameTypes, StartGame);

        Physics.queriesHitBackfaces = true;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.CompareTag("cardBack"))
                {
                    HandleCardClicked(hit.collider.gameObject.GetComponentInParent<CardController>());
                }
            }
        }
    }

    private void HandleCardClicked(CardController card)
    {
        if (card == currentlySelectedCard)
        {
            return;
        }

        card.SetFrontFace(true);

        if (currentlySelectedCard == null)
        {
            currentlySelectedCard = card;
        }
        else
        {
            if (card.Id != currentlySelectedCard.Id)
            {
                StartCoroutine(FlipCardsBack(1, new CardController[] { card, currentlySelectedCard }));
            }
            else
            {
                
            }
            currentlySelectedCard = null;
        }
    }

    public void ShowGameTypes()
    {
        UI.ShowCanvas(UI.TypeCanvas);
    }

    public void StartGame(GameType gameType)
    {
        var grid = CardContainer.GetComponent<GridLayoutGroup>();
        grid.constraintCount = gameType.rows;

        var spacingCoef = 0.2f;
        var rectTransform = CardContainer.GetComponent<RectTransform>();
        var fittingCoef = Mathf.Min(rectTransform.sizeDelta.x / gameType.rows, rectTransform.sizeDelta.y / gameType.cols);

        grid.cellSize = Vector2.one * (fittingCoef * (1 - spacingCoef));
        grid.spacing = Vector2.one * (fittingCoef * spacingCoef);

        ClearCardContainer();

        deck = SpawnCards(gameType.rows * gameType.cols);

        UI.ShowCanvas(UI.GameCanvas);

        StartCoroutine(FlipCardsBack());
    }

    public void EndGame()
    {
        UI.ShowCanvas(UI.HomeCanvas);
    }

    private IEnumerator FlipCardsBack(int secsDelay = 2, CardController[] cards = null)
    {
        yield return new WaitForSeconds(secsDelay);

        cards ??= deck.ToArray();

        foreach (var card in cards)
        {
            card.SetFrontFace(false);
        }
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
}
