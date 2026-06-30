// ======================================================
// BlowerInputBridge.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-27
// 更新日時 : 2026-06-27
// 概要     : PlayerInput を使用した ECS 入力ブリッジ Player Id 対応
// ======================================================

using UnityEngine;
using Unity.Entities;
using UnityEngine.InputSystem;
using Unity.Collections;

namespace BlowerSystem
{
    /// <summary>
    /// PlayerInput が同一 GameObject にアタッチされている前提の入力ブリッジ
    /// ECS の BlowerMoveData へプレイヤー入力を転送する
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public sealed class BlowerInputBridge : MonoBehaviour
    {
        // ======================================================
        // インスペクタ設定
        // ======================================================

        /// <summary>
        /// 移動入力アクション参照
        /// </summary>
        [SerializeField]
        private InputActionReference _moveActionReference;

        // ======================================================
        // コンポーネント参照
        // ======================================================

        /// <summary>
        /// PlayerInput
        /// </summary>
        private PlayerInput _playerInput;

        /// <summary>
        /// 移動入力アクション
        /// </summary>
        private InputAction _moveAction;

        /// <summary>
        /// ECS エンティティ管理
        /// </summary>
        private EntityManager _entityManager;

        /// <summary>
        /// ECS エンティティ検索クエリ
        /// </summary>
        private EntityQuery _query;

        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>
        /// このブリッジが担当する Player Id
        /// </summary>
        private int _playerId;

        // ======================================================
        // Unity イベント
        // ======================================================

        private void Awake()
        {
            // 同一 GameObject から PlayerInput を取得する
            _playerInput = GetComponent<PlayerInput>();

            // ECS ワールドから EntityManager を取得する
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            // PlayerInput が割り当てた Player Index を取得する
            _playerId = _playerInput.playerIndex;

            // BlowerId と BlowerMoveData を持つ Entity を検索対象にする
            _query = _entityManager.CreateEntityQuery(
                typeof(BlowerIdComponent),
                typeof(BlowerMoveData)
            );

            // InputActionReference からアクション名を取得し PlayerInput から実体を引く
            if (_moveActionReference != null)
            {
                _moveAction = _playerInput.actions[_moveActionReference.action.name];
                _moveAction.Enable();
            }
        }

        private void Update()
        {
            // Move 入力を取得する
            Vector2 input = _moveAction.ReadValue<Vector2>();

            // 横入力のみ使用する
            float horizontal = input.x;

            // Player 0 の場合は入力方向を反転する
            if (_playerId == 0)
            {
                horizontal *= -1f;
            }

            // ECS エンティティ一覧を取得する
            NativeArray<Entity> entities = _query.ToEntityArray(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];

                // BlowerId を取得する
                BlowerIdComponent id = _entityManager.GetComponentData<BlowerIdComponent>(entity);

                // 自分の PlayerId と一致する場合のみ処理する
                if (id.Value != _playerId)
                {
                    continue;
                }

                // 移動データを取得する
                BlowerMoveData data = _entityManager.GetComponentData<BlowerMoveData>(entity);

                // 入力値をそのまま反映する
                data.Direction = horizontal;

                // ECS へ書き戻す
                _entityManager.SetComponentData(entity, data);
            }

            // NativeArray を解放する
            entities.Dispose();
        }
    }
}