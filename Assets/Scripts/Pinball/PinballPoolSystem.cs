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
        /// システム生成時の初期化を行う
        /// </summary>
        /// <param name="state">システム状態</param>
        public void OnCreate(ref SystemState state)
        {
            // プールバッファが生成されるまで更新しない
            state.RequireForUpdate<PinballPoolEntry>();
        }

        /// <summary>
        /// プール要求されたピンボールをプール状態へ遷移させる
        /// </summary>
        /// <param name="state">システム状態</param>
        public void OnUpdate(ref SystemState state)
        {
            // EntityCommandBuffer を生成する
            EntityCommandBuffer ecb =
                new EntityCommandBuffer(state.WorldUpdateAllocator);
            
            // プールバッファを取得する
            DynamicBuffer<PinballPoolEntry> pool =
                SystemAPI.GetSingletonBuffer<PinballPoolEntry>();

            // --------------------------------------------------
            // プール要求処理
            // --------------------------------------------------
            foreach (var (requestPool, entity)
                in SystemAPI.Query<RefRO<PinballRequestPool>>()
                .WithEntityAccess())
            {
                // プール配置番号を取得する
                int poolIndex =
                    requestPool.ValueRO.PoolIndex;

                // X 座標を算出する
                float positionX =
                    poolIndex % POOL_ROW_COUNT;

                // Z 座標を算出する
                float positionZ =
                    poolIndex / POOL_ROW_COUNT;

                // ピンボール物理データを取得する
                PinballPhysicsData physics =
                    SystemAPI.GetComponent<PinballPhysicsData>(entity);

                // --------------------------------------------------
                // 位置設定
                // --------------------------------------------------
                // プール位置へ移動する
                SystemAPI.SetComponent(entity, new LocalTransform
                {
                    Position = new float3(
                        positionX,
                        POOL_HEIGHT,
                        positionZ),

                    Rotation = quaternion.identity,
                    Scale = physics.Scale
                });

                // --------------------------------------------------
                // 物理停止
                // --------------------------------------------------
                // 速度を停止する
                SystemAPI.SetComponent(entity, new PhysicsVelocity
                {
                    Linear = float3.zero,
                    Angular = float3.zero
                });

                // 物理影響を無効化する
                SystemAPI.SetComponent(entity, new PhysicsMass
                {
                    InverseMass = 0f,
                    InverseInertia = float3.zero,
                    Transform = RigidTransform.identity
                });

                // 減衰を停止する
                SystemAPI.SetComponent(entity, new PhysicsDamping
                {
                    Linear = 0f,
                    Angular = 0f
                });

                // --------------------------------------------------
                // プール登録
                // --------------------------------------------------
                // プールへ追加する
                pool.Add(new PinballPoolEntry
                {
                    Entity = entity
                });

                // --------------------------------------------------
                // 状態設定
                // --------------------------------------------------
                // プール状態へ変更する
                SystemAPI.SetComponent(entity,
                    new PinballState
                    {
                        State = PinballStateType.Pool
                    });

                // --------------------------------------------------
                // プール要求を削除
                // --------------------------------------------------
                ecb.RemoveComponent<PinballRequestPool>(entity);

                // 構造変更を反映する
                ecb.Playback(state.EntityManager);
            }
        }
    }
}