using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWaveConfig", menuName = "HonViet/Waves/Wave Config")]
public class WaveConfig : ScriptableObject
{
    [Header("Enemies")]
    public List<EnemyData> EnemiesToSpawn = new List<EnemyData>();

    [Header("Timing")]
    [Min(0f)] public float SpawnInterval = 1f;
    [Min(0f)] public float DelayBeforeNextWave = 5f;
}
