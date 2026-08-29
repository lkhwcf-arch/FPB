using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Text playerNameText;
    [SerializeField] private Text scoreText;
    [SerializeField] private RectTransform playerNameContainer;
    [SerializeField] private RectTransform scoreContainer;

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager가 없어 결과 정보를 표시할 수 없습니다.", this);
            return;
        }

        if (playerNameText != null)
            playerNameText.text = GameManager.Instance.PlayerName;
        else
            Debug.LogError("Player Name Text가 연결되지 않았습니다.", this);

        if (scoreText != null)
            scoreText.text = GameManager.Instance.CurrentScore.ToString();
        else
            Debug.LogError("Score Text가 연결되지 않았습니다.", this);
    }

    public void Restart()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
            return;
        }

        SceneManager.LoadScene("1_GameView");
    }

    public void ReturnToMain()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMain();
            return;
        }

        SceneManager.LoadScene("0_Main");
    }

}
