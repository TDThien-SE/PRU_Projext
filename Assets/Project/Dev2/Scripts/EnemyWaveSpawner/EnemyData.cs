using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "HonViet/Enemies/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Identity")]
    public string EnemyName;

    [Header("Base Stats")]
    [Min(1f)] public float BaseHealth = 100f;
    [Min(0f)] public float MoveSpeed = 1f;

    [Header("Prefab")]
    public GameObject Prefab;
}
