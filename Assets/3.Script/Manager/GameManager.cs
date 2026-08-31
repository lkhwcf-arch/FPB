using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public enum CharacterType
{
    YellowBird,
    RedBird
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Background Music")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioClip bgmClip;
    [SerializeField, Range(0f, 1f)] private float bgmVolume = 0.5f;

    [Header("Sound Effects")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip mouseClickClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip skillClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;

    private bool isGameOver;
    private int currentScore;
    private string playerName;

    public int CurrentScore => currentScore;
    public bool IsGameOver => isGameOver;
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

        PrepareAudioSources();
        InitializeGame();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        UpdateBgm(SceneManager.GetActiveScene().buildIndex);
    }

    private void Update()
    {
        CheckMouseClickSound();
    }

    private void PrepareAudioSources()
    {
        // 인스펙터에서 연결하지 않아도 자동으로 생성
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        bgmSource.playOnAwake = false;
        bgmSource.loop = true;
        bgmSource.spatialBlend = 0f;
        bgmSource.volume = bgmVolume;

        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.spatialBlend = 0f;
        sfxSource.volume = sfxVolume;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateBgm(scene.buildIndex);
    }

    private void UpdateBgm(int sceneIndex)
    {
        // 0_Main과 1_GameView에서만 BGM 재생
        bool shouldPlayBgm = sceneIndex == 0 || sceneIndex == 1;

        if (!shouldPlayBgm)
        {
            if (bgmSource.isPlaying)
            {
                bgmSource.Stop();
            }

            return;
        }

        if (bgmClip == null)
        {
            return;
        }

        bgmSource.volume = bgmVolume;

        // 씬 0 → 씬 1 이동 시 처음부터 다시 재생하지 않음
        if (bgmSource.isPlaying && bgmSource.clip == bgmClip)
        {
            return;
        }

        bgmSource.clip = bgmClip;
        bgmSource.Play();
    }

    private void CheckMouseClickSound()
    {
        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            PlaySfx(mouseClickClip);
        }
    }

    private void PlaySfx(AudioClip clip)
    {
        if (clip == null || sfxSource == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayJumpSound()
    {
        PlaySfx(jumpClip);
    }

    public void PlaySkillSound()
    {
        PlaySfx(skillClip);
    }
    public void DeathSound()
    {
        PlaySfx(deathClip);
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
        DeathSound();
        isGameOver = true;

        Debug.Log("게임 오버");
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
            Debug.LogError("플레이어 이름이 없어 랭킹을 등록하지 못했습니다.");
            return;
        }

        RankManager.Instance.AddRank(playerName, currentScore);
        Debug.Log($"랭킹 등록 완료: {PlayerName}, {CurrentScore}점");
    }

    public void RestartGame()
    {
        BeginGame();
        SceneManager.LoadScene("1_GameView");
    }

    public void ReturnToMain()
    {
        InitializeGame();
        SceneManager.LoadScene("0_Main");
    }
}