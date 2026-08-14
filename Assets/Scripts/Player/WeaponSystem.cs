using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ArenaBreak.Player
{
    public class WeaponSystem : MonoBehaviour
    {
        // InputSystem_Actions 에셋에 재장전 액션이 없어서 코드에서 직접 만든다
        private const string ReloadBindingPath = "<Keyboard>/r";

        [Header("References")]
        [SerializeField] private InputActionAsset _inputActions;
        [SerializeField] private Camera _camera;

        [Header("Fire")]
        [SerializeField] private float _fireInterval = 0.15f;
        [SerializeField] private float _range = 50f;

        [Header("Ammo")]
        [SerializeField] private int _magazineSize = 12;
        [SerializeField] private float _reloadDuration = 1.5f;

        [Header("Hit Marker")]
        [SerializeField] private float _hitMarkerLifetime = 0.5f;
        [SerializeField] private float _hitMarkerScale = 0.1f;

        // 벽·표적과 같은 색이면 명중 지점이 보이지 않는다
        [SerializeField] private Color _hitMarkerColor = Color.yellow;

        private InputAction _attackAction;
        private InputAction _reloadAction;

        private int _currentAmmo;
        private float _nextFireTime;
        private bool _isReloading;

        private void Awake()
        {
            _attackAction = _inputActions.FindActionMap("Player", true).FindAction("Attack", true);
            _reloadAction = new InputAction("Reload", InputActionType.Button, ReloadBindingPath);

            _currentAmmo = _magazineSize;
        }

        private void OnEnable()
        {
            _attackAction.Enable();
            _reloadAction.Enable();
        }

        private void OnDisable()
        {
            _attackAction.Disable();
            _reloadAction.Disable();
        }

        // 코드로 만든 액션은 직접 해제해야 한다
        private void OnDestroy()
        {
            _reloadAction.Dispose();
        }

        private void Update()
        {
            if (_isReloading)
            {
                return;
            }

            if (_reloadAction.WasPressedThisFrame() && _currentAmmo < _magazineSize)
            {
                StartCoroutine(Reload());
                return;
            }

            if (_attackAction.IsPressed() && Time.time >= _nextFireTime)
            {
                Fire();
            }
        }

        private void Fire()
        {
            if (_currentAmmo <= 0)
            {
                return;
            }

            _nextFireTime = Time.time + _fireInterval;
            _currentAmmo--;

            Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            if (Physics.Raycast(ray, out RaycastHit hit, _range))
            {
                SpawnHitMarker(hit.point);
            }

            // 3주차에 HUD로 옮긴다
            Debug.Log($"발사 — 남은 탄약 {_currentAmmo}/{_magazineSize}");
        }

        // 풀링 대상: 발사마다 생성/파괴된다
        private void SpawnHitMarker(Vector3 point)
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            // Collider를 남기면 다음 발사가 이 구체에 맞는다
            Destroy(marker.GetComponent<Collider>());

            marker.transform.position = point;
            marker.transform.localScale = Vector3.one * _hitMarkerScale;
            marker.GetComponent<MeshRenderer>().material.color = _hitMarkerColor;

            Destroy(marker, _hitMarkerLifetime);
        }

        private IEnumerator Reload()
        {
            _isReloading = true;
            Debug.Log($"재장전 시작 — {_reloadDuration}초");

            yield return new WaitForSeconds(_reloadDuration);

            _currentAmmo = _magazineSize;
            _isReloading = false;
            Debug.Log($"재장전 완료 — 남은 탄약 {_currentAmmo}/{_magazineSize}");
        }
    }
}
