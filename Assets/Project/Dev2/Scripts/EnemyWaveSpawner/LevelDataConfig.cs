using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelDataConfig", menuName = "HonViet/Waves/Level Data Config")]
public class LevelDataConfig : ScriptableObject
{
    public List<WaveConfig> Waves = new List<WaveConfig>();
}
