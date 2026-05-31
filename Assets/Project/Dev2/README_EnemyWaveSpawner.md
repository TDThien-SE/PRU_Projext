# Dev2 Enemy & Wave Spawner Module

## Scene test

Open:

`Assets/Project/Dev2/Scenes/Dev2_EnemyWaveTest.unity`

Press Play. The scene contains:

- `Dev2_EnemyPool`
- `Dev2_WaveSpawner`
- 5 spawn points on the right side
- A blue base reach line at `x = -8`
- `Dev2_EventDebugListener` for event logs

## Test data

Enemy data:

- `ScriptableObjects/Enemies/EnemyData_BoBinhCoKhi.asset`
- `ScriptableObjects/Enemies/EnemyData_KyBinhCoKhi.asset`
- `ScriptableObjects/Enemies/EnemyData_ThietGiap.asset`

Wave data:

- `ScriptableObjects/Waves/WaveConfig_01_Training.asset`
- `ScriptableObjects/Waves/WaveConfig_02_Mixed.asset`
- `ScriptableObjects/Waves/WaveConfig_03_Armored.asset`

Level data:

- `ScriptableObjects/Levels/LevelData_World1_Level1_Dev2Test.asset`

## Expected behavior

- Enemies spawn from right-side spawn points.
- Enemies move strictly from right to left.
- When an enemy reaches `x <= -8`, `OnEnemyReachedBase` is raised and the enemy returns to the pool.
- When `EnemyMovement.Die()` is called, `OnEnemyKilled(EnemyData data)` is raised and the enemy returns to the pool.
- `OnEnemySpawned(GameObject enemy)` is raised every time the spawner takes an enemy from the pool.

## Manual kill test

During Play Mode:

1. Select `Dev2_EventDebugListener`.
2. Open the component context menu.
3. Click `Kill First Active Enemy`.

This calls `EnemyMovement.Die()` on one active enemy and should print the killed event in Console.
