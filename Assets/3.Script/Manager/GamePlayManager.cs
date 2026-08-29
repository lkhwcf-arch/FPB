using UnityEngine;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    [SerializeField]
    private Text playerNameText;

    [SerializeField]
    private Text scoreText;

    private float scoreTimer;

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError(
                "GameManager가 없습니다. 메인 씬부터 실행해주세요."
            );

            enabled = false;
            return;
        }

        GameManager.Instance.BeginGame();

        if (playerNameText != null)
        {
            playerNameText.text =
                GameManager.Instance.PlayerName;
        }

        UpdateScoreUI();
    }

    private void Update()
    {
        if (GameManager.Instance == null ||
            GameManager.Instance.IsGameOver)
        {
            return;
        }

        scoreTimer += Time.deltaTime;

        while (scoreTimer >= 1f)
        {
            scoreTimer -= 1f;

            GameManager.Instance.AddScore(1);
            UpdateScoreUI();
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text =
                GameManager.Instance.CurrentScore.ToString();
        }
    }
}
