using System;
using System.Collections;
using System.Collections.Generic;
using ArenaBreak.Enemy;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

namespace ArenaBreak.Core
{
    public class WaveSpawner : MonoBehaviour
    {
        [Header("Waves")]
        [SerializeField] private List<WaveData> _waves = new List<WaveData>();
        [SerializeField] private float _delayBetweenWaves = 3f;

        // 죽은 뒤 사라지기까지. 풀 반환은 이 시간이 지나고 한다
        [SerializeField] private float _despawnDelay = 1f;

        [Header("Scene References")]
        // 자식 Transform을 모아 쓴다. 스폰 위치를 늘려도 인스펙터를 다시 연결할 필요가 없다
        [SerializeField] private Transform _spawnPointsRoot;
        [SerializeField] private Transform _player;

        public event Action<int> WaveStarted;
        public event Action<int> WaveCleared;
        public event Action AllWavesCleared;
        public event Action<int> KillCountChanged;

        public int KillCount => _killCount;

        private readonly List<Transform> _spawnPoints = new List<Transform>();
        private readonly Dictionary<GameObject, ObjectPool<GameObject>> _pools =
            new Dictionary<GameObject, ObjectPool<GameObject>>();

        private int _aliveCount;
        private int _killCount;

        private void Awake()
        {
            if (_spawnPointsRoot == null)
            {
                return;
            }

            foreach (Transform child in _spawnPointsRoot)
            {
                _spawnPoints.Add(child);
            }
        }

        private void Start()
        {
            if (_spawnPoints.Count == 0)
            {
                Debug.LogError("스폰 위치가 없다. Spawn Points Root 를 연결할 것", this);
                return;
            }

            StartCoroutine(RunWaves());
        }

        private IEnumerator RunWaves()
        {
            for (int i = 0; i < _waves.Count; i++)
            {
                int waveNumber = i + 1;

                WaveStarted?.Invoke(waveNumber);
                Debug.Log($"웨이브 {waveNumber} 시작 — 적 {_waves[i].EnemyCount}마리");

                yield return SpawnWave(_waves[i]);

                while (_aliveCount > 0)
                {
                    yield return null;
                }

                WaveCleared?.Invoke(waveNumber);
                Debug.Log($"웨이브 {waveNumber} 클리어");

                if (i < _waves.Count - 1)
                {
                    yield return new WaitForSeconds(_delayBetweenWaves);
                }
            }

            AllWavesCleared?.Invoke();
            Debug.Log("전체 웨이브 클리어");
        }

        private IEnumerator SpawnWave(WaveData wave)
        {
            for (int i = 0; i < wave.EnemyCount; i++)
            {
                Spawn(wave.EnemyPrefab);
                yield return new WaitForSeconds(wave.SpawnInterval);
            }
        }

        private void Spawn(GameObject prefab)
        {
            Transform point = _spawnPoints[UnityEngine.Random.Range(0, _spawnPoints.Count)];
            ObjectPool<GameObject> pool = GetPool(prefab);
            GameObject enemy = pool.Get();

            // NavMeshAgent는 transform을 직접 옮기면 내부 위치와 어긋난다. Warp를 쓴다
            if (enemy.TryGetComponent(out NavMeshAgent agent))
            {
                agent.Warp(point.position);
            }
            else
            {
                enemy.transform.position = point.position;
            }

            enemy.transform.rotation = point.rotation;

            // 프리팹은 씬의 Player를 참조할 수 없다. 스폰한 쪽이 넣어준다
            if (enemy.TryGetComponent(out EnemyAI ai))
            {
                ai.SetTarget(_player);
            }

            _aliveCount++;

            // 매 프레임 씬을 훑는 대신, 스폰한 적의 사망만 듣는다.
            // 풀 반환도 여기서 한다 — 특정 AI 컴포넌트가 아니라 Health만 있으면 되게
            if (enemy.TryGetComponent(out Health health))
            {
                void OnEnemyDied()
                {
                    health.Died -= OnEnemyDied;
                    _aliveCount--;
                    _killCount++;
                    KillCountChanged?.Invoke(_killCount);

                    StartCoroutine(ReleaseAfterDelay(pool, enemy));
                }

                health.Died += OnEnemyDied;
            }
        }

        private IEnumerator ReleaseAfterDelay(ObjectPool<GameObject> pool, GameObject enemy)
        {
            // 죽는 순간 timeScale이 0이 될 수 있다. 그래도 반환은 되어야 한다
            yield return new WaitForSecondsRealtime(_despawnDelay);

            pool.Release(enemy);
        }

        private ObjectPool<GameObject> GetPool(GameObject prefab)
        {
            if (_pools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
            {
                return pool;
            }

            pool = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(prefab),
                actionOnGet: enemy => enemy.SetActive(true),
                actionOnRelease: enemy => enemy.SetActive(false),
                actionOnDestroy: enemy => Destroy(enemy));

            _pools.Add(prefab, pool);
            return pool;
        }
    }
}
