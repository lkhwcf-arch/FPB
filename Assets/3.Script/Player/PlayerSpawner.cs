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

    [SerializeField]
    private Vector3 fallbackSpawnPosition = new Vector3(-43f, 6.5f, 0f);

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

        bool hasSceneSpawnPoint = spawnPoint != null && spawnPoint.gameObject.scene.IsValid();
        Vector3 position = hasSceneSpawnPoint ? spawnPoint.position : fallbackSpawnPosition;
        Quaternion rotation = hasSceneSpawnPoint ? spawnPoint.rotation : Quaternion.identity;

        if (!hasSceneSpawnPoint)
        {
            Debug.LogWarning(
                "PlayerSpawner의 Spawn Point가 씬 오브젝트가 아닙니다. " +
                $"Fallback Spawn Position {fallbackSpawnPosition}을 사용합니다.",
                this
            );
        }

        GameObject player = Instantiate(selectedPrefab, position, rotation);
        player.name = selectedPrefab.name;

        PlayerSkill playerSkill = player.GetComponent<PlayerSkill>();
        if (playerSkill == null)
        {
            playerSkill = player.AddComponent<PlayerSkill>();
        }

        playerSkill.Initialize(GameManager.Instance.SelectedCharacter);
    }
}
