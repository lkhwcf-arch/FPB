using UnityEngine;

public class RankBoardUI : MonoBehaviour
{
    [SerializeField]
    private RankItem[] rankItems;

    public void Open()
    {
        gameObject.SetActive(true);

        Refresh();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void Refresh()
    {
        RankManager.Instance.Load();

        var rankList =
            RankManager.Instance.rankBoardData.RankBoard;

        for (int i = 0; i < rankItems.Length; i++)
        {
            if (i < rankList.Count)
            {
                rankItems[i].SetData(
                    rankList[i]
                );
            }
            else
            {
                rankItems[i].Clear();
            }
        }
    }
}