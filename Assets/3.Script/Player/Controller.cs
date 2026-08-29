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
    private Rigidbody body;
    bool isDead;


    private void Awake()
    {
        body = GetComponent<Rigidbody>();

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

        if (GameManager.Instance == null ||
            GameManager.Instance.IsGameOver)
        {
            return;
        }

        CheckJumpInput();
        CheckYBoundary();
    }
    private void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }

        if (GameManager.Instance == null ||
            GameManager.Instance.IsGameOver)
        {
            return;
        }

        body.AddForce(
            Physics.gravity * gravityScale,
            ForceMode.Acceleration
        );
    }
    private void CheckJumpInput()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
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
        return target.CompareTag("Enemy");
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

        GameManager.Instance.AddScore(1);
    }
}
