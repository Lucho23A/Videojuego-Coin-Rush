using UnityEngine;
public class CamaraFollow : MonoBehaviour
{
    public Transform target;
    float distancia = 6f;
    float altura    = 3f;
    float mouseX, mouseY;
    float sensibilidad = 3f;

    void LateUpdate()
    {
        if (target == null) return;
        mouseX += Input.GetAxis("Mouse X") * sensibilidad;
        mouseY -= Input.GetAxis("Mouse Y") * sensibilidad;
        mouseY  = Mathf.Clamp(mouseY, -10f, 40f);

        Quaternion rot = Quaternion.Euler(mouseY, mouseX, 0);
        Vector3 offset  = rot * new Vector3(0, altura, -distancia);
        Camera.main.transform.position = target.position + offset + Vector3.up;
        Camera.main.transform.LookAt(target.position + Vector3.up);
    }
}   