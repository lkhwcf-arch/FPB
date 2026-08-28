using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField]
    private GameObject mainPanel;

    [SerializeField]
    private GameObject rankBoardPanel;

    private void Start()
    {
        if (rankBoardPanel != null)
        {
            rankBoardPanel.SetActive(false);
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
}