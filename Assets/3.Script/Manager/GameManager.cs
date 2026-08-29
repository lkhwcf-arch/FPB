using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum CharacterType
{
    YellowBird,
    RedBird
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private bool isGameOver = false;
    private int currentScore = 0;
    public int CurrentScore => currentScore;
    public bool IsGameOver => isGameOver;
    private string playerName;
    public string PlayerName => playerName;

    public CharacterType SelectedCharacter { get; private set; }
    public bool HasSelectedCharacter { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeGame();
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        playerName = name.Trim();

        Debug.Log("등록된 이름 : " + playerName);
    }


    public void SelectCharacter(CharacterType characterType)
    {
        SelectedCharacter = characterType;
        HasSelectedCharacter = true;

        Debug.Log($"선택된 캐릭터: {SelectedCharacter}");
    }

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        isGameOver = false;

        currentScore = 0;
    }
    public void BeginGame()
    {
        currentScore = 0;
        isGameOver = false;

        Debug.Log("게임 시작");
    }
    public void AddScore(int point = 1)
    {
        if (isGameOver)
        {
            return;
        }

        currentScore += point;

        Debug.Log("Current Score : " + currentScore);
    }
    public void GameOver()
    {
        if (isGameOver)
        {
            return;
        }
        // 충돌 이벤트가 여러 번 발생해도 한 번만 처리
        isGameOver = true;

        Debug.Log($"게임 오버");
        Debug.Log($"플레이어: {PlayerName}");
        Debug.Log($"최종 점수: {CurrentScore}");

        RegisterRank();
        SceneManager.LoadScene("2_Result");
    }

    private void RegisterRank()
    {
        if (RankManager.Instance == null)
        {
            Debug.LogError("RankManager가 존재하지 않습니다.");
            return;
        }
        if (string.IsNullOrWhiteSpace(PlayerName))
        {
            Debug.LogError(
                "플레이어 이름이 없어 랭킹을 등록하지 못했습니다."
            );
            return;
        }

        RankManager.Instance.AddRank(playerName, currentScore);
        Debug.Log(
           $"랭킹 등록 완료: {PlayerName}, {CurrentScore}점"
       );
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
