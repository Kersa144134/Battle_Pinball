// ======================================================
// PinballPoolSystem.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-07-06
// 更新日時 : 2026-07-06
// 概要     : ピンボールをプール状態へ遷移させる ECS システム
// ======================================================

using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace PinballSystem
{
    /// <summary>
    /// プール要求されたピンボールをプール状態へ遷移させるシステム
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(PinballSpawnSystem))]
    public partial struct PinballPoolSystem : ISystem
    {
        // ======================================================
        // 定数
        // ======================================================

        /// <summary>
        /// プールを 1 列に配置する最大個数
        /// </summary>
        private const int POOL_ROW_COUNT = 50;

        /// <summary>
        /// プール配置高さ
        /// </summary>
        private const float POOL_HEIGHT = 50f;

        // ======================================================
        // ISystem 実装
        // ======================================================

        /// <summary>
        /// プール要求を処理し、ピンボールをプール状態へ移行する
        /// </summary>
        /// <param name="state">システム状態</param>
        public void OnUpdate(ref SystemState state)
        {
            // プール管理情報（単一エンティティ）を取得する
            RefRW<PinballPoolInfo> poolInfo =
                SystemAPI.GetSingletonRW<PinballPoolInfo>();

            // プールバッファを取得する（メインスレッド管理）
            DynamicBuffer<PinballPoolEntry> pool =
                SystemAPI.GetSingletonBuffer<PinballPoolEntry>();

            // 構造変更を遅延するためのECBを作成する
            EntityCommandBuffer ecb =
                new EntityCommandBuffer(state.WorldUpdateAllocator);

            // --------------------------------------------------
            // プール要求処理
            // --------------------------------------------------
            foreach (var (request, entity)
                in SystemAPI.Query<RefRO<PinballRequestPool>>()
                .WithEntityAccess())
            {
                // 現在のプール Index を取得
                int poolIndex = poolInfo.ValueRO.NextPoolIndex;

                // 次回 Index を先に計算する
                int nextIndex = poolIndex + 1;

                // 最大値を超えた場合はリセット
                if (nextIndex == int.MaxValue)
                {
                    nextIndex = 0;
                }

                // Index を確定して更新する
                poolInfo.ValueRW.NextPoolIndex = nextIndex;

                // --------------------------------------------------
                // 座標計算
                // --------------------------------------------------
                float x = poolIndex % POOL_ROW_COUNT;
                float z = poolIndex / POOL_ROW_COUNT;

                // ピンボール物理情報を取得
                PinballPhysicsData physics =
                    SystemAPI.GetComponent<PinballPhysicsData>(entity);

                // --------------------------------------------------
                // 位置設定
                // --------------------------------------------------
                SystemAPI.SetComponent(entity, new LocalTransform
                {
                    Position = new float3(x, POOL_HEIGHT, z),
                    Rotation = quaternion.identity,
                    Scale = physics.Scale
                });

                // --------------------------------------------------
                // 物理停止処理
                // --------------------------------------------------
                SystemAPI.SetComponent(entity, new PhysicsVelocity
                {
                    Linear = float3.zero,
                    Angular = float3.zero
                });

                SystemAPI.SetComponent(entity, new PhysicsMass
                {
                    InverseMass = 0f,
                    InverseInertia = float3.zero,
                    Transform = RigidTransform.identity
                });

                SystemAPI.SetComponent(entity, new PhysicsDamping
                {
                    Linear = 0f,
                    Angular = 0f
                });

                // --------------------------------------------------
                // プール登録
                // --------------------------------------------------
                pool.Add(new PinballPoolEntry
                {
                    Entity = entity
                });

                // --------------------------------------------------
                // 状態更新
                // --------------------------------------------------
                SystemAPI.SetComponent(entity, new PinballState
                {
                    State = PinballStateType.Pool
                });

                // --------------------------------------------------
                // プール要求コンポーネント削除
                // --------------------------------------------------
                ecb.RemoveComponent<PinballRequestPool>(entity);
            }

            // 構造変更を反映する
            ecb.Playback(state.EntityManager);
        }
    }
}