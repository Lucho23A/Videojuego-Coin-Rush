using UnityEngine;
using UnityEngine.InputSystem;

public class JugadorController : MonoBehaviour
{
    CharacterController cc;
    Vector3 velocidad;
    float speed = 6f;
    float runSpeed = 9.6f;
    float jumpForce = 8f;
    float gravity = -20f;
    Transform camara;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        camara = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        bool enSuelo = cc.isGrounded;
        if (enSuelo && velocidad.y < 0) velocidad.y = -2f;

        // Nuevo Input System
        Vector2 moveInput = new Vector2(
            Keyboard.current != null && Keyboard.current.dKey.isPressed ? 1f :
            Keyboard.current != null && Keyboard.current.aKey.isPressed ? -1f : 0f,
            Keyboard.current != null && Keyboard.current.wKey.isPressed ? 1f :
            Keyboard.current != null && Keyboard.current.sKey.isPressed ? -1f : 0f
        );

        Vector3 forward = camara.forward;
        Vector3 right   = camara.right;
        forward.y = 0; right.y = 0;
        forward.Normalize(); right.Normalize();

        Vector3 move = (forward * moveInput.y + right * moveInput.x);
        bool corriendo = Keyboard.current != null && 
                         Keyboard.current.leftShiftKey.isPressed;
        float currentSpeed = corriendo ? runSpeed : speed;
        cc.Move(move * currentSpeed * Time.deltaTime);

        if (move != Vector3.zero)
            transform.forward = Vector3.Lerp(transform.forward, move, 0.2f);

        // Salto
        bool salto = Keyboard.current != null && 
                     Keyboard.current.spaceKey.wasPressedThisFrame;
        if (salto && enSuelo)
            velocidad.y = jumpForce;

        velocidad.y += gravity * Time.deltaTime;
        cc.Move(velocidad * Time.deltaTime);
    }
}