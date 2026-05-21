using UnityEngine;
public class RotarObjeto : MonoBehaviour
{
    void Update() => transform.Rotate(0, 90f * Time.deltaTime, 0);
}
