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

namespace BallSystem
{
    /// <summary>
    /// ピンボールの生成処理を行うシステム
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(PinballPoolSystem))]
    public partial struct PinballSpawnSystem : ISystem
    {
        // ======================================================
        // 定数
        // ======================================================

        /// <summary>
        /// プール配置高さ
        /// </summary>
        private const float POOL_HEIGHT = 10f;

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
            var settings = SystemAPI.GetSingleton<PinballSpawnSettings>();

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

            // --------------------------------------------------
            // 初期化ループ
            // --------------------------------------------------
            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];

                // ピンボール状態を取得する
                PinballState pinballState =
                    SystemAPI.GetComponent<PinballState>(entity);

                //// ピンボールIDを設定する
                //pinballState.Id = i;

                //// 更新した情報を書き戻す
                //SystemAPI.SetComponent(entity, pinballState);

                // 現在の物理状態を取得する
                PhysicsMass currentMass =
                    SystemAPI.GetComponent<PhysicsMass>(entity);

                // 減衰設定を取得する
                PhysicsDamping currentDamping =
                    SystemAPI.GetComponent<PhysicsDamping>(entity);

                // 速度を取得する
                PhysicsVelocity currentVelocity =
                    SystemAPI.GetComponent<PhysicsVelocity>(entity);

                // 現在の Transform を取得する
                LocalTransform currentTransform =
                    SystemAPI.GetComponent<LocalTransform>(entity);

                // キャッシュコンポーネントを書き込み
                SystemAPI.SetComponent(entity, new PinballPhysicsCache
                {
                    CachedPosition = currentTransform.Position,
                    CachedRotation = currentTransform.Rotation,
                    CachedScale = currentTransform.Scale,
                    
                    CachedLinearVelocity = currentVelocity.Linear,
                    CachedAngularVelocity = currentVelocity.Angular,

                    CachedMass = currentMass,
                    CachedDamping = currentDamping
                });

                // --------------------------------------------------
                // 位置をプール固定
                // --------------------------------------------------
                SystemAPI.SetComponent(entity, new LocalTransform
                {
                    Position = new float3(0f, POOL_HEIGHT, 0f),
                    Rotation = quaternion.identity,
                    Scale = currentTransform.Scale
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
                DynamicBuffer<PinballPoolEntry> pool =
                    SystemAPI.GetSingletonBuffer<PinballPoolEntry>();

                pool.Add(new PinballPoolEntry
                {
                    Entity = entity
                });

                // プール状態設定
                SystemAPI.SetComponentEnabled<PinballState>(entity, false);
            }

            // NativeArray を破棄
            entities.Dispose();

            // システム停止
            state.Enabled = false;
        }
    }
}