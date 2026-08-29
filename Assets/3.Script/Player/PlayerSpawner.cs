using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
    private GameObject yellowBirdPrefab;

    [SerializeField]
    private GameObject redBirdPrefab;

    private void Start()
    {
        SpawnSelectedCharacter();
    }

    private void SpawnSelectedCharacter()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager가 존재하지 않습니다.");
            return;
        }

        if (!GameManager.Instance.HasSelectedCharacter)
        {
            Debug.LogError("선택된 캐릭터가 없습니다.");
            return;
        }

        GameObject selectedPrefab = GameManager.Instance.SelectedCharacter switch
        {
            CharacterType.YellowBird => yellowBirdPrefab,
            CharacterType.RedBird => redBirdPrefab,
            _ => null
        };

        if (selectedPrefab == null)
        {
            Debug.LogError("선택한 캐릭터 프리팹이 연결되지 않았습니다.");
            return;
        }

        Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;

        Quaternion rotation = spawnPoint != null ? spawnPoint.rotation : transform.rotation;

        Instantiate(selectedPrefab, position, rotation);
    }
}
