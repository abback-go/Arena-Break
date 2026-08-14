using UnityEngine;

namespace ArenaBreak.Core
{
    [CreateAssetMenu(fileName = "Wave", menuName = "ArenaBreak/Wave Data")]
    public class WaveData : ScriptableObject
    {
        [SerializeField] private GameObject _enemyPrefab;
        [SerializeField] private int _enemyCount = 3;
        [SerializeField] private float _spawnInterval = 1f;

        public GameObject EnemyPrefab => _enemyPrefab;
        public int EnemyCount => _enemyCount;
        public float SpawnInterval => _spawnInterval;
    }
}
