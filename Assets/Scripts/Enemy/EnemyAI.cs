using ArenaBreak.Core;
using UnityEngine;
using UnityEngine.AI;

namespace ArenaBreak.Enemy
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Health))]
    public class EnemyAI : MonoBehaviour
    {
        private enum State
        {
            Idle,
            Chase,
            Attack,
            Dead
        }

        [Header("References")]
        [SerializeField] private Transform _player;

        [Header("Range")]
        // 아레나 40×40의 대각선이 약 57m다. 이보다 작으면 먼 구석의 적이 오지 않는다
        [SerializeField] private float _chaseDistance = 60f;
        [SerializeField] private float _attackDistance = 2f;

        [Header("Attack")]
        [SerializeField] private float _attackInterval = 2f;
        [SerializeField] private int _attackDamage = 10;


        private NavMeshAgent _agent;
        private Health _health;
        private IDamageable _playerDamageable;

        private State _state;
        private float _nextAttackTime;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _health = GetComponent<Health>();
        }

        // 풀에서 다시 꺼내질 때마다 실행된다. 체력과 상태를 여기서 되돌린다
        private void OnEnable()
        {
            _health.Died += OnDied;
            _health.ResetHealth();
            _state = State.Idle;
        }

        private void OnDisable()
        {
            _health.Died -= OnDied;
        }

        private void Start()
        {
            ResolveTarget();
        }

        // 프리팹은 씬 오브젝트를 참조할 수 없다. 스폰한 쪽이 넣어준다
        public void SetTarget(Transform player)
        {
            _player = player;
            ResolveTarget();
        }

        private void ResolveTarget()
        {
            // 플레이어가 Health를 갖는 것은 3주차다. 없으면 로그만 남긴다
            if (_player != null)
            {
                _player.TryGetComponent(out _playerDamageable);
            }
        }

        private void Update()
        {
            if (_player == null)
            {
                return;
            }

            float distance = Vector3.Distance(transform.position, _player.position);

            switch (_state)
            {
                case State.Idle:
                    if (distance <= _chaseDistance)
                    {
                        _state = State.Chase;
                    }
                    break;

                case State.Chase:
                    if (distance > _chaseDistance)
                    {
                        StopMoving();
                        _state = State.Idle;
                    }
                    else if (distance <= _attackDistance)
                    {
                        StopMoving();
                        _state = State.Attack;
                    }
                    else if (_agent.isOnNavMesh)
                    {
                        _agent.isStopped = false;
                        _agent.SetDestination(_player.position);
                    }
                    break;

                case State.Attack:
                    if (distance > _attackDistance)
                    {
                        _state = State.Chase;
                    }
                    else if (Time.time >= _nextAttackTime)
                    {
                        Attack();
                    }
                    break;

                case State.Dead:
                    break;
            }
        }

        private void Attack()
        {
            _nextAttackTime = Time.time + _attackInterval;
            _playerDamageable?.TakeDamage(_attackDamage);

            // 3주차에 HUD와 게임오버로 옮긴다
            Debug.Log($"적 공격 — 데미지 {_attackDamage}");
        }

        // 사라지는 시점과 풀 반환은 스포너가 정한다. 여기서는 멈추기만 한다
        private void OnDied()
        {
            _state = State.Dead;
            StopMoving();
        }

        private void StopMoving()
        {
            if (_agent.isOnNavMesh)
            {
                _agent.isStopped = true;
                _agent.ResetPath();
            }
        }
    }
}
