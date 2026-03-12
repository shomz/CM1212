using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private UIController UI;
    [SerializeField] private Transform CardContainer;
    [SerializeField] private CardController CardPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var gameTypes = new GameType[]
        {
            new(2, 2),
            new(4, 4),
            new(5, 6)
        };

        UI.BuildGameTypeButtons(gameTypes);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame(GameType gameType)
    {
        var grid = CardContainer.GetComponent<GridLayoutGroup>();
        grid.constraintCount = gameType.rows;

        SpawnCards(gameType.rows * gameType.cols);
    }

    private void SpawnCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Instantiate(CardPrefab, CardContainer);
        }
    }
}
