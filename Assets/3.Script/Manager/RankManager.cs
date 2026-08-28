using System;
using System.IO;
using System.Linq;
using UnityEngine;

public class RankManager : MonoBehaviour
{
    public static RankManager Instance { get; private set; }
    private const int MAX_COUNT = 5;
    private string savePath;

    private RankBoardData rankBoard = new();
    public RankBoardData rankBoardData => rankBoard;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);

        savePath = Path.Combine(
            Application.persistentDataPath,
            "RankBoard.json"
        );

        Load();
    }

    public void AddRank(string name,int point)
    {
        RankData rank = new(name,point, DateTime.Now);
        rankBoard.RankBoard.Add(rank);

        SortRank();

        Save();
    }

    private void SortRank()
    {
        rankBoard.RankBoard = rankBoard.RankBoard.OrderByDescending(x => x.point).
            ThenBy(x => DateTime.Parse(x.dateTime)).Take(MAX_COUNT).ToList();

        for (int i = 0; i < rankBoard.RankBoard.Count; i++)
        {
            rankBoard.RankBoard[i].rank = i + 1;
        }
    }
    public void Save()
    {
        string json = JsonUtility.ToJson(rankBoard, true);

        File.WriteAllText(savePath, json);
        Debug.Log($"Rank Save : {savePath}");
    }

    public void Load()
    {
        if (!File.Exists(savePath))
        {
            rankBoard = new RankBoardData();
            return;
        }
        string json = File.ReadAllText(savePath);

        rankBoard = JsonUtility.FromJson<RankBoardData>(json);
        if (rankBoard == null)
        {
            rankBoard = new RankBoardData();
        }

        if (rankBoard.RankBoard == null)
        {
            rankBoard.RankBoard =
                new System.Collections.Generic.List<RankData>();
        }

        SortRank();
    }
}
