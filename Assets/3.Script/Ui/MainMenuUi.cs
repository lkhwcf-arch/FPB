using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject rankBoardPanel;
    [SerializeField] private InputField nicknameInputField;
    [SerializeField] private GameObject selectCharacterPanel;
    private void Start()
    {
        if (rankBoardPanel != null)
        {
            rankBoardPanel.SetActive(false);
        }
        if (selectCharacterPanel != null)
        {
            selectCharacterPanel.SetActive(false);
        }
        if (nicknameInputField != null)
        {
            nicknameInputField.onEndEdit.AddListener(OnNameEndEdit);
        }
    }
    private void OnDestroy()
    {
        if (nicknameInputField != null)
        {
            nicknameInputField.onEndEdit.RemoveListener(OnNameEndEdit);
        }
    }
    private void OnNameEndEdit(string inputName)
    {
        // Escape로 취소한 경우에는 등록하지 않음
        if (nicknameInputField.wasCanceled)
        {
            return;
        }
        NameInput(inputName);
    }
    public void NameInput(string inputName)
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager가 존재하지 않습니다.");
            return;
        }

        if (nicknameInputField == null)
        {
            return;
        }

        inputName = nicknameInputField.text;

        if (string.IsNullOrWhiteSpace(inputName))
        {
            Debug.LogWarning("닉네임을 입력해주세요.");
            return;
        }

        GameManager.Instance.SetName(inputName);
        nicknameInputField.DeactivateInputField();
        if (selectCharacterPanel != null)
        {
            selectCharacterPanel.SetActive(true);
        }

        Debug.Log($"닉네임 등록: {inputName}");
    }

    public void SelectYellowBird()
    {
        SelectCharacterAndStart(CharacterType.YellowBird);
    }

    public void SelectRedBird()
    {
        SelectCharacterAndStart(CharacterType.RedBird);
    }

    private void SelectCharacterAndStart(CharacterType characterType)
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager가 존재하지 않습니다.");
            return;
        }

        GameManager.Instance.SelectCharacter(characterType);
        SceneManager.LoadScene("1_GameView");
    }

    public void NameInput()
    {
        if (nicknameInputField != null)
        {
            NameInput(nicknameInputField.text);
        }
    }

    public void OpenRankBoard()
    {
        if (rankBoardPanel == null)
        {
            Debug.LogError("RankBoard Panel이 연결되지 않았습니다.");
            return;
        }

        rankBoardPanel.SetActive(true);
    }

    public void CloseRankBoard()
    {
        if (rankBoardPanel == null)
        {
            return;
        }

        rankBoardPanel.SetActive(false);
    }

    // X버튼으로 오브젝트 비활성화 공통
    public void CloseButton(GameObject closeObject)
    {
        if (closeObject is null) return;

        closeObject.SetActive(false);
    }

    public void OpenBoard(GameObject board)
    {
        if (board is null) return;

        board.SetActive(true);
    }

    public void GameEnd()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void InGameLoad()
    {
        SceneManager.LoadScene("1_GameView");
    }
}