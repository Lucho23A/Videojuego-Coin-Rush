using UnityEngine;

public class CamaraFollow : MonoBehaviour
{
    public Transform target;

    float distancia    = 6f;
    float altura       = 2.5f;
    float sensibilidad = 3f;
    float suavizado    = 8f;
    float mouseX       = 0f;
    float mouseY       = 20f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        mouseX += Input.GetAxis("Mouse X") * sensibilidad;
        mouseY -= Input.GetAxis("Mouse Y") * sensibilidad;
        mouseY  = Mathf.Clamp(mouseY, -10f, 50f);

        Quaternion rotacion = Quaternion.Euler(mouseY, mouseX, 0);
        Vector3 offset      = rotacion * new Vector3(0, 0, -distancia);
        Vector3 posDeseada  = target.position + Vector3.up * altura + offset;

        transform.position = Vector3.Lerp(transform.position, posDeseada,
                             suavizado * Time.deltaTime);

        transform.LookAt(target.position + Vector3.up * (altura * 0.5f));
    }
}