using UnityEngine;

/// <summary>
/// Datos de un personaje. Se crea como asset desde el menú Create → CoinRush → Character Data.
/// </summary>
[CreateAssetMenu(fileName = "NewCharacterData", menuName = "CoinRush/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Identidad")]
    public string characterName = "Integrante 1";
    public string nickname = "El Ágil";
    [TextArea] public string description = "Descripción del personaje...";

    [Header("Stats de movimiento")]
    public float moveSpeed = 6f;
    public float runSpeed = 9.6f;
    public float jumpForce = 8f;

    [Header("Stats de combate")]
    public int maxHealth = 3;

    [Header("Habilidad especial")]
    public string specialAbility = "Doble salto";

    [Header("Prefab")]
    public GameObject characterPrefab;

    [Header("UI Preview")]
    public Sprite portrait;
}
