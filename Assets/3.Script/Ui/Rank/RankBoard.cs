using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankBoardUI : MonoBehaviour
{
    [SerializeField] private RankItem[] rankItems;
    [SerializeField] private RectTransform entriesContainer;
    [SerializeField, Min(1f)] private float refreshInterval = 10f;

    private readonly List<Text> nicknameTexts = new();
    private readonly List<Text> scoreTexts = new();
    private bool runtimeRowsCreated;
    private float nextRefreshTime;

    private void OnEnable()
    {
        TryScheduledRefresh();
    }

    private void Update()
    {
        TryScheduledRefresh();
    }

    public void Open()
    {
        gameObject.SetActive(true);

        Refresh();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Refresh()
    {
        if (RankManager.Instance == null)
        {
            Debug.LogError("RankManager가 없어 랭킹을 표시할 수 없습니다.", this);
            return;
        }

        RankManager.Instance.Load();

        var rankList =
            RankManager.Instance.rankBoardData.RankBoard;

        if (rankItems != null && rankItems.Length > 0)
        {
            for (int i = 0; i < rankItems.Length; i++)
            {
                if (rankItems[i] == null)
                    continue;

                if (i < rankList.Count)
                    rankItems[i].SetData(rankList[i]);
                else
                    rankItems[i].Clear();
            }

            return;
        }

        EnsureRuntimeRows();
        for (int i = 0; i < 5; i++)
        {
            bool hasData = i < rankList.Count;
            nicknameTexts[i].text = hasData ? rankList[i].name : "-";
            scoreTexts[i].text = hasData ? rankList[i].point.ToString() : "-";
        }
    }

    private void TryScheduledRefresh()
    {
        if (Time.unscaledTime < nextRefreshTime)
            return;

        Refresh();
        nextRefreshTime = Time.unscaledTime + refreshInterval;
    }

    private void EnsureRuntimeRows()
    {
        if (runtimeRowsCreated)
            return;

        RectTransform container = entriesContainer != null
            ? entriesContainer
            : transform as RectTransform;

        if (container == null)
        {
            Debug.LogError("랭킹 텍스트를 배치할 RectTransform이 없습니다.", this);
            return;
        }

        for (int i = 0; i < 5; i++)
        {
            float y = 145f - i * 116f;
            nicknameTexts.Add(CreateText(container, $"RankNickname_{i + 1}", -20f, y, 460f));
            scoreTexts.Add(CreateText(container, $"RankScore_{i + 1}", 390f, y, 220f));
        }

        runtimeRowsCreated = true;
    }

    private static Text CreateText(
        RectTransform parent,
        string objectName,
        float x,
        float y,
        float width)
    {
        GameObject textObject = new(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(x, y);
        rect.sizeDelta = new Vector2(width, 90f);

        Text text = textObject.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 42;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color32(35, 59, 104, 255);
        text.raycastTarget = false;
        return text;
    }
}
