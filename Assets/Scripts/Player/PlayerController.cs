using UnityEngine;

/// <summary>
/// Controlador básico del jugador. WASD + Mouse + Salto.
/// En el nivel 1 será reemplazado por el de Starter Assets (requisito).
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Configuración (desde CharacterData)")]
    [SerializeField] private CharacterData data;

    [Header("Referencias")]
    [SerializeField] private Transform cameraTransform;

    [Header("Física")]
    [SerializeField] private float gravity = -25f;
    [SerializeField] private LayerMask groundLayer;

    private CharacterController controller;
    private Vector3 velocity;
    private int currentHealth;
    private bool isGrounded;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;
        if (data != null) currentHealth = data.maxHealth;
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
        ApplyGravity();
    }

    private void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Movimiento relativo a la cámara (clave para tercera persona)
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0; camRight.y = 0;
        camForward.Normalize(); camRight.Normalize();

        Vector3 move = (camForward * v + camRight * h);
        float speed = Input.GetKey(KeyCode.LeftShift) ? data.runSpeed : data.moveSpeed;
        controller.Move(move * speed * Time.deltaTime);

        // Rotar al personaje hacia donde se mueve
        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
        }
    }

    private void HandleJump()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = data.jumpForce;
        }
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void TakeDamage(int amount = 1)
    {
        currentHealth -= amount;
        Debug.Log($"[Player] Vida: {currentHealth}");
        if (currentHealth <= 0 && GameManager.Instance != null)
        {
            GameManager.Instance.LoseLife();
        }
    }
}