// ======================================================
// PinballSpawnSystem.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-27
// 更新日時 : 2026-06-27
// 概要     : ピンボールを生成する ECS システム
// ======================================================

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace PinballSystem
{
    /// <summary>
    /// ピンボールの生成処理を行うシステム
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(PinballPoolEntrySystem))]
    public partial struct PinballSpawnSystem : ISystem
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
        /// <param name="state">システムの状態</param>
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PinballSpawnSettings>();
        }

        /// <summary>
        /// ピンボールを生成する
        /// </summary>
        /// <param name="state">システムの状態</param>
        public void OnUpdate(ref SystemState state)
        {
            // 生成設定取得
            PinballSpawnSettings settings = SystemAPI.GetSingleton<PinballSpawnSettings>();

            // プレハブ存在チェック
            if (!state.EntityManager.Exists(settings.Prefab))
            {
                state.Enabled = false;
                return;
            }

            // 一括生成
            NativeArray<Entity> entities =
                state.EntityManager.Instantiate(
                    settings.Prefab,
                    settings.SpawnCount,
                    Allocator.Temp);

            // ピンボールプール取得
            DynamicBuffer<PinballPoolEntry> pool =
                    SystemAPI.GetSingletonBuffer<PinballPoolEntry>();

            // --------------------------------------------------
            // 初期化ループ
            // --------------------------------------------------
            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];

                // --------------------------------------------------
                // 位置設定
                // --------------------------------------------------
                // X 座標を算出する
                float positionX = i % POOL_ROW_COUNT;

                // Z 座標を算出する
                float positionZ = i / POOL_ROW_COUNT; 
                
                SystemAPI.SetComponent(entity, new LocalTransform
                {
                    Position = new float3(positionX, POOL_HEIGHT, positionZ),
                    Rotation = quaternion.identity,
                    Scale = 0f
                });

                // --------------------------------------------------
                // 物理停止
                // --------------------------------------------------
                // 速度をゼロ化
                SystemAPI.SetComponent(entity, new PhysicsVelocity
                {
                    Linear = float3.zero,
                    Angular = float3.zero
                });

                // 物理影響を完全無効化
                SystemAPI.SetComponent(entity, new PhysicsMass
                {
                    InverseMass = 0f,
                    InverseInertia = float3.zero,
                    Transform = RigidTransform.identity
                });

                // ダンピング停止
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
                // 状態設定
                // --------------------------------------------------
                SystemAPI.SetComponent(entity,
                    new PinballState
                    {
                        State = PinballStateType.Pool
                    });
            }

            // NativeArray を破棄
            entities.Dispose();

            // システム停止
            state.Enabled = false;
        }
    }
}