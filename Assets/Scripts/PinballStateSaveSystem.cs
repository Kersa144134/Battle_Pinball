// ======================================================
// PinballStateSaveSystem.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-27
// 更新日時 : 2026-06-27
// 概要     : ピンボールの状態保存・復元を行う ECS システム
// ======================================================

using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace BallSystem
{
    /// <summary>
    /// ピンボールの状態保存・復元を行うシステム
    /// </summary>
    [UpdateInGroup(typeof(BeforePhysicsSystemGroup))]
    public partial class PinballStateSaveSystem : SystemBase
    {
        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>
        /// 保存したピンボールの状態一覧
        /// </summary>
        private NativeArray<(Entity Entity, LocalTransform LocalTransform, PhysicsVelocity PhysicsVelocity)> _savedStates;

        // ======================================================
        // SystemBase 実装
        // ======================================================

        /// <summary>
        /// システム生成時の初期化を行う
        /// </summary>
        protected override void OnCreate()
        {
            // 状態保存設定が存在する場合のみシステムを実行する
            RequireForUpdate<PinballStateSaveData>();
        }

        /// <summary>
        /// システム破棄時の終了処理を行う
        /// </summary>
        protected override void OnDestroy()
        {
            // 保存データが存在する場合のみ破棄する
            if (_savedStates.IsCreated)
            {
                // NativeArray を解放する
                _savedStates.Dispose();
            }
        }

        /// <summary>
        /// 入力イベントの登録を行う
        /// </summary>
        protected override void OnUpdate()
        {
            // 状態保存設定を取得する
            var pinballStateSaveData = SystemAPI.ManagedAPI.GetSingleton<PinballStateSaveData>();

            // 入力イベントが未登録の場合のみ登録する
            if (!pinballStateSaveData.SaveAction.enabled)
            {
                // 保存入力時に状態保存を実行する
                pinballStateSaveData.SaveAction.performed += _ => SaveState();

                // 復元入力時に状態復元を実行する
                pinballStateSaveData.LoadAction.performed += _ => LoadState();

                // 保存入力を有効化する
                pinballStateSaveData.SaveAction.Enable();

                // 復元入力を有効化する
                pinballStateSaveData.LoadAction.Enable();
            }
        }

        // ======================================================
        // パブリックメソッド
        // ======================================================

        /// <summary>
        /// 現在の状態を保存する
        /// </summary>
        private void SaveState()
        {
            // 実行中の Job を完了させる
            Dependency.Complete();

            // 前回保存したデータが存在する場合は破棄する
            if (_savedStates.IsCreated)
            {
                // NativeArray を解放する
                _savedStates.Dispose();
            }

            // 保存対象となる Entity の検索条件を作成する
            var query = SystemAPI.QueryBuilder()
                .WithAll<LocalTransform, PhysicsVelocity>()
                .Build();

            // 保存対象数に応じた NativeArray を生成する
            _savedStates =
                new NativeArray<(Entity, LocalTransform, PhysicsVelocity)>(
                    query.CalculateEntityCount(),
                    Allocator.Persistent);

            // 保存先インデックス
            int index = 0;

            // 保存対象 Entity を順番に取得する
            foreach (var (localTransform, physicsVelocity, entity) in
                     SystemAPI.Query<RefRO<LocalTransform>,
                     RefRO<PhysicsVelocity>>().WithEntityAccess())
            {
                // Entity の状態を保存する
                _savedStates[index] =
                (
                    entity,
                    localTransform.ValueRO,
                    physicsVelocity.ValueRO
                );

                // 次の保存先へ進める
                index++;
            }
        }

        // ======================================================
        // プライベートメソッド
        // ======================================================

        /// <summary>
        /// 保存した状態を復元する
        /// </summary>
        private void LoadState()
        {
            // 保存データが存在しない場合は処理しない
            if (!_savedStates.IsCreated)
            {
                return;
            }

            // 実行中の Job を完了させる
            Dependency.Complete();

            // 保存したすべての状態を復元する
            foreach (var savedState in _savedStates)
            {
                // Transform を復元する
                EntityManager.SetComponentData(
                    savedState.Entity,
                    savedState.LocalTransform);

                // 物理速度を復元する
                EntityManager.SetComponentData(
                    savedState.Entity,
                    savedState.PhysicsVelocity);
            }
        }
    }
}