using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerSkill : MonoBehaviour
{
    [Header("Skill Info")]
    [SerializeField] private string skillName;

    [Header("Common")]
    [SerializeField] private float cooldown = 20f;
    [SerializeField] private float effectDuration = 3f;

    [Header("Red Bird")]
    [SerializeField, Range(0.1f, 1f)] private float redGravityMultiplier = 0.5f;

    [Header("Yellow Bird")]
    [SerializeField, Range(0f, 1f)] private float yellowSpeedMultiplier = 0.8f;

    [Header("UI Highlight")]
    [SerializeField] private Color highlightColor = Color.red;
    [SerializeField] private Vector2 highlightDistance = new(4f, -4f);

    private CharacterType characterType;
    private Controller controller;
    private float nextUseTime;
    private bool initialized;
    private Outline skillOutline;
    private Image cooldownMask;
    private ScrollingMap[] scrollingMaps;

    public string SkillName => string.IsNullOrWhiteSpace(skillName)
        ? characterType == CharacterType.RedBird
            ? "가벼워져라"
            : "세상아 느려져라"
        : skillName;

    public void Initialize(CharacterType selectedCharacter)
    {
        characterType = selectedCharacter;
        controller = GetComponent<Controller>();
        scrollingMaps = FindObjectsByType<ScrollingMap>(FindObjectsSortMode.None);
        SetupSkillUI();
        initialized = true;
    }

    private void Update()
    {
        UpdateCooldownMask();

        if (!initialized ||
            controller == null ||
            controller.IsDead ||
            (GameManager.Instance != null && GameManager.Instance.IsGameOver))
        {
            return;
        }

        if (Keyboard.current != null &&
            Keyboard.current.aKey.wasPressedThisFrame &&
            Time.time >= nextUseTime)
        {
            ActivateSkill();
        }
    }

    private void ActivateSkill()
    {
        nextUseTime = Time.time + Mathf.Max(0.01f, cooldown);
        UpdateCooldownMask();

        if (characterType == CharacterType.RedBird)
        {
            controller.EnableLightJumpWindow(effectDuration, redGravityMultiplier);
        }
        else
        {
            SetScrollingSpeed(yellowSpeedMultiplier);
        }

        StopAllCoroutines();
        StartCoroutine(FinishEffectAfterDelay());
    }

    private IEnumerator FinishEffectAfterDelay()
    {
        if (skillOutline != null)
            skillOutline.enabled = true;

        yield return new WaitForSeconds(effectDuration);

        if (characterType == CharacterType.YellowBird)
            SetScrollingSpeed(1f);

        if (skillOutline != null)
            skillOutline.enabled = false;
    }

    private void SetScrollingSpeed(float multiplier)
    {
        foreach (ScrollingMap scrollingMap in scrollingMaps)
        {
            if (scrollingMap != null)
                scrollingMap.SetSpeedMultiplier(multiplier);
        }

    }

    private void SetupSkillUI()
    {
        GameObject skillObject = GameObject.Find("Skill");
        if (skillObject == null)
        {
            Debug.LogWarning("Canvas 아래 Skill UI를 찾지 못했습니다.", this);
            return;
        }

        if (!skillObject.TryGetComponent(out skillOutline))
            skillOutline = skillObject.AddComponent<Outline>();

        skillOutline.effectColor = highlightColor;
        skillOutline.effectDistance = highlightDistance;
        skillOutline.enabled = false;

        Transform labelTransform = skillObject.transform.Find("SkillNameValue");
        if (labelTransform != null && labelTransform.TryGetComponent(out Text label))
        {
            label.text = SkillName;
        }
        else
        {
            Debug.LogWarning(
                "Skill 아래 SkillNameValue Text 오브젝트가 없습니다. 씬에서 연결 상태를 확인하세요.",
                skillObject
            );
        }


        Transform maskTransform = skillObject.transform.Find("SkillCooldownMask");
        if (maskTransform != null && maskTransform.TryGetComponent(out cooldownMask))
        {
            cooldownMask.fillAmount = 0f;
            cooldownMask.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning(
                "Skill 아래 SkillCooldownMask Image 오브젝트가 없습니다.",
                skillObject
            );
        }
    }

    private void UpdateCooldownMask()
    {
        if (!initialized || cooldownMask == null)
            return;

        float remaining = Mathf.Max(0f, nextUseTime - Time.time);
        bool coolingDown = remaining > 0f;

        if (cooldownMask.gameObject.activeSelf != coolingDown)
            cooldownMask.gameObject.SetActive(coolingDown);

        if (coolingDown)
            cooldownMask.fillAmount = Mathf.Clamp01(remaining / Mathf.Max(0.01f, cooldown));
    }

    private void OnDisable()
    {
        if (initialized && characterType == CharacterType.YellowBird)
            SetScrollingSpeed(1f);
    }
}
