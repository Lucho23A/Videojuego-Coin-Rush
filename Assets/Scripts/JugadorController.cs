using UnityEngine;
using UnityEngine.InputSystem;

public class JugadorController : MonoBehaviour
{
    CharacterController cc;
    Vector3 velocidad;
    float speed    = 6f;
    float runSpeed = 9.6f;
    float jumpForce = 8f;
    float gravity   = -20f;
    Transform camara;

    // ── Animación ────────────────────────────────────────────────
    Animator anim;
    static readonly int hashVelocidad = Animator.StringToHash("Velocidad");
    static readonly int hashEnSuelo   = Animator.StringToHash("EnSuelo");
    static readonly int hashSaltar    = Animator.StringToHash("Saltar");

    void Start()
    {
        cc     = GetComponent<CharacterController>();
        camara = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;

        // Busca el Animator en el modelo hijo (ModeloPersonaje)
        anim = GetComponentInChildren<Animator>();
        if (anim == null)
            Debug.LogWarning("[JugadorController] No se encontró Animator en los hijos. " +
                             "Asegúrate de que Luis.glb tenga el PlayerAnimator asignado.");
    }

    void Update()
    {
        bool enSuelo = cc.isGrounded;
        if (enSuelo && velocidad.y < 0f) velocidad.y = -2f;

        // ── Input ────────────────────────────────────────────────
        float h = LeerEje(Key.D, Key.A);
        float v = LeerEje(Key.W, Key.S);

        Vector3 forward = camara.forward;
        Vector3 right   = camara.right;
        forward.y = 0f; right.y = 0f;
        forward.Normalize(); right.Normalize();

        Vector3 move        = forward * v + right * h;
        bool    corriendo   = Keyboard.current != null &&
                              Keyboard.current.leftShiftKey.isPressed;
        float   currentSpeed = corriendo ? runSpeed : speed;

        cc.Move(move * currentSpeed * Time.deltaTime);

        // Rotar hacia donde se mueve
        if (move.sqrMagnitude > 0.01f)
            transform.forward = Vector3.Lerp(transform.forward, move, 0.2f);

        // ── Salto ────────────────────────────────────────────────
        bool saltando = Keyboard.current != null &&
                        Keyboard.current.spaceKey.wasPressedThisFrame;
        if (saltando && enSuelo)
        {
            velocidad.y = jumpForce;
            anim?.SetTrigger(hashSaltar);
        }

        velocidad.y += gravity * Time.deltaTime;
        cc.Move(velocidad * Time.deltaTime);

        // ── Animaciones ──────────────────────────────────────────
        ActualizarAnimaciones(move, corriendo, enSuelo);
    }

    void ActualizarAnimaciones(Vector3 move, bool corriendo, bool enSuelo)
    {
        if (anim == null) return;

        // Velocidad normalizada: 0 = quieto, 0.5 = caminar, 1 = correr
        float speed01 = 0f;
        if (move.sqrMagnitude > 0.01f)
            speed01 = corriendo ? 1f : 0.5f;

        // Suaviza la transición para que no sea brusca
        float velocidadActual = anim.GetFloat(hashVelocidad);
        float velocidadSuave  = Mathf.Lerp(velocidadActual, speed01, Time.deltaTime * 8f);

        anim.SetFloat(hashVelocidad, velocidadSuave);
        anim.SetBool(hashEnSuelo,   enSuelo);
    }

    // Devuelve 1, -1 o 0 según qué tecla está presionada
    float LeerEje(Key positivo, Key negativo)
    {
        if (Keyboard.current == null) return 0f;
        if (Keyboard.current[positivo].isPressed)  return  1f;
        if (Keyboard.current[negativo].isPressed) return -1f;
        return 0f;
    }
}
