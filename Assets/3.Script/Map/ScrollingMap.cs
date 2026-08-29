using System.Collections.Generic;
using UnityEngine;

public class ScrollingMap : MonoBehaviour
{
    private const int PoolSize = 4;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject[] scrollingPrefabs;

    [Header("Start Position")]
    [SerializeField]
    private Vector3 startPosition =
        new Vector3(-13.5f, 0f, 1.25f);

    [Header("Scrolling")]
    [SerializeField]
    private float sectionWidth = 50f;

    [SerializeField]
    private float scrollSpeed = 5f;

    [SerializeField]
    private float recycleX = -45f;

    private float speedMultiplier = 1f;

    private readonly List<Transform> sections = new();

    private void Start()
    {
        CreateSections();
    }

    private void Update()
    {
        if (GameManager.Instance != null &&
            GameManager.Instance.IsGameOver)
        {
            return;
        }

        MoveSections();
        RecycleSections();
    }

    private void CreateSections()
    {
        if (scrollingPrefabs == null || scrollingPrefabs.Length < PoolSize)
        {
            Debug.LogError("ScrollingObject_0~3 네 개를 순서대로 연결해야 합니다.", this);
            enabled = false;
            return;
        }

        for (int i = 0; i < PoolSize; i++)
        {
            GameObject prefab = scrollingPrefabs[i];

            if (prefab == null)
            {
                Debug.LogError($"Scrolling Prefab {i}가 없습니다.");
                continue;
            }

            Vector3 position = startPosition;
            position.x += sectionWidth * i;

            GameObject section = Instantiate(
                prefab,
                position,
                Quaternion.identity,
                transform
            );

            section.name = prefab.name;
            sections.Add(section.transform);
        }
    }

    private void MoveSections()
    {
        float movement = scrollSpeed * speedMultiplier * Time.deltaTime;

        foreach (Transform section in sections)
        {
            section.position += Vector3.left * movement;
        }
    }

    private void RecycleSections()
    {
        foreach (Transform section in sections)
        {
            float rightEdge =
                section.position.x + sectionWidth * 0.5f;

            if (rightEdge > recycleX)
            {
                continue;
            }

            float rightmostX = GetRightmostX(section);

            Vector3 position = section.position;
            position.x = rightmostX + sectionWidth;
            section.position = position;
        }
    }

    private float GetRightmostX(Transform excludedSection)
    {
        float rightmostX = float.MinValue;

        foreach (Transform section in sections)
        {
            if (section == excludedSection)
            {
                continue;
            }

            if (section.position.x > rightmostX)
            {
                rightmostX = section.position.x;
            }
        }

        return rightmostX;
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = Mathf.Max(0f, multiplier);
    }
}
