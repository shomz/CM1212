using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private UIController UI;
    [SerializeField] private Transform CardContainer;
    [SerializeField] private CardController CardPrefab;
    [SerializeField] private int CardTypesCount = 4;

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
    }

    // Update is called once per frame
    void Update()
    {
        
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

        SpawnCards(gameType.rows * gameType.cols);

        UI.ShowCanvas(UI.GameCanvas);
    }

    public void EndGame()
    {
        UI.ShowCanvas(UI.HomeCanvas);
    }

    private void ClearCardContainer()
    {
        for (int i = CardContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(CardContainer.GetChild(i).gameObject);
        }
    }

    private void SpawnCards(int count)
    {
        var ids = GenerateIds(count);

        for (int i = 0; i < count; i++)
        {
            var card = Instantiate(CardPrefab, CardContainer);

            card.Id = ids[i];
        }
    }

    private List<int> GenerateIds(int count)
    {
        List<int> output = new();
        var lastId = -1;
        var id = -1;
        for (int i = 0; i < count / 2; i++)
        {
            while (id == lastId)
            {
                id = Random.Range(0, CardTypesCount);
            }
            output.Add(id);
            lastId = id;
        }

        output.AddRange(output);
        Debug.Log(JsonUtility.ToJson(output));
        return output.OrderBy(item => System.Guid.NewGuid()).ToList();
    }
}
