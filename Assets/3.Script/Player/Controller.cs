using UnityEngine;
using UnityEngine.InputSystem;
public class Controller : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField] private float jumpVelocity = 6f;

    [Header("Gravity")]
    [SerializeField, Range(1f, 3f)] private float gravityScale = 1f;

    [Header("World Boundary")]
    [SerializeField] private float minY = 3f;

    [SerializeField] private float maxY = 10f;
    [SerializeField] private bool useWorldBoundaryDeath;
    private Rigidbody body;
    private bool isDead;
    private float gravityMultiplier = 1f;
    private float lightJumpWindowEndTime = -1f;
    private float lightJumpGravityMultiplier = 0.5f;

    public bool IsDead => isDead;


    private void Awake()
    {
        if (!TryGetComponent(out body))
        {
            Debug.LogError($"{name}: Controller와 같은 오브젝트에 Rigidbody가 필요합니다.", this);
            enabled = false;
            return;
        }

        // 중력 배율을 직접 적용
        body.useGravity = false;

        // 플레이어는 제자리에서 Y축으로만 이동
        body.constraints =
            RigidbodyConstraints.FreezePositionX |
            RigidbodyConstraints.FreezePositionZ |
            RigidbodyConstraints.FreezeRotation;

        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.collisionDetectionMode =
            CollisionDetectionMode.Continuous;
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        CheckJumpInput();
        if (useWorldBoundaryDeath)
        {
            CheckYBoundary();
        }
    }
    private void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }

        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        body.AddForce(
            Physics.gravity * gravityScale * gravityMultiplier,
            ForceMode.Acceleration
        );
    }
    private void CheckJumpInput()
    {
        if ((Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) ||
            (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame))
        {
            Jump();
        }
    }
    private void Jump()
    {
        // 위쪽 한계에서 추가 점프 방지
        if (transform.position.y >= maxY)
        {
            return;
        }

        gravityMultiplier = Time.time <= lightJumpWindowEndTime
            ? lightJumpGravityMultiplier
            : 1f;

        Vector3 velocity = body.linearVelocity;
        velocity.y = jumpVelocity;
        body.linearVelocity = velocity;
    }
    private void CheckYBoundary()
    {
        float playerY = transform.position.y;

        if (playerY <= minY || playerY >= maxY)
        {
            //Dead("Y축 경계 이탈");
            Dead();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (IsDeadlyObject(collision.gameObject))
        {
            Dead();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (IsDeadlyObject(other.gameObject))
        {
            Dead();
        }
    }
    private bool IsDeadlyObject(GameObject target)
    {
        if (target.CompareTag("Enemy"))
        {
            return true;
        }

        Transform root = target.transform.root;
        if (root != target.transform && root.CompareTag("Enemy"))
        {
            return true;
        }

        string objectName = target.name.ToLowerInvariant();
        return objectName.Contains("pipe") ||
               objectName.Contains("ground") ||
               objectName.Contains("floor") ||
               objectName.Contains("ceiling");
    }
    // 사망시 처리 
    private void Dead()
    {
        // 충돌이 겹쳐도 한 번만 사망
        if (isDead)
        {
            return;
        }

        isDead = true;

        body.linearVelocity = Vector3.zero;
        body.isKinematic = true;


        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager가 없습니다.");
            return;
        }

        GameManager.Instance.GameOver();
    }
    public void AddScore()
    {
        if (isDead) return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(1);
        }
    }

    public void EnableLightJumpWindow(float duration, float gravityScaleMultiplier)
    {
        lightJumpWindowEndTime = Time.time + Mathf.Max(0f, duration);
        lightJumpGravityMultiplier = Mathf.Clamp(gravityScaleMultiplier, 0.1f, 1f);
    }
}
