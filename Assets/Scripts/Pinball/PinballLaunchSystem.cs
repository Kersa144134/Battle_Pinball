// ======================================================
// PinballLaunchSystem.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-30
// 更新日時 : 2026-06-30
// 概要     : ピンボールをプール状態から発射する ECS システム
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
    /// プール状態のピンボールを発射するシステム
    /// Authoring の位置・角度を基準に発射する
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct PinballLaunchSystem : ISystem
    {
        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>
        /// 経過時間
        /// </summary>
        private float _timer;

        // ======================================================
        // ISystem 実装
        // ======================================================

        public void OnUpdate(ref SystemState state)
        {
            // --------------------------------------------------
            // 発射設定取得
            // --------------------------------------------------
            PinballLaunchSettings settings =
                SystemAPI.GetSingleton<PinballLaunchSettings>();

            _timer += SystemAPI.Time.DeltaTime;

            // 発射間隔チェック
            if (_timer < settings.Interval)
            {
                return;
            }

            _timer = 0f;

            // --------------------------------------------------
            // プール状態の対象取得
            // --------------------------------------------------
            EntityQuery query = SystemAPI.QueryBuilder()
                .WithAll<PinballTag>()
                .WithDisabled<PinballPoolState>()
                .Build();

            NativeArray<Entity> entities =
                query.ToEntityArray(Allocator.Temp);

            if (entities.Length == 0)
            {
                entities.Dispose();
                return;
            }

            // --------------------------------------------------
            // 発射処理
            // --------------------------------------------------
            // 1体だけ発射
            Entity entity = entities[0];

            // 発射元 Transform 取得
            LocalTransform launcherTransform =
                SystemAPI.GetComponent<LocalTransform>(settings.LauncherEntity);

            // Physics キャッシュ取得
            PinballPhysicsCache cache =
                SystemAPI.GetComponent<PinballPhysicsCache>(entity);

            // ピンボール Transform 更新
            SystemAPI.SetComponent(entity, new LocalTransform
            {
                Position = launcherTransform.Position,
                Rotation = launcherTransform.Rotation,
                Scale = cache.CachedScale
            });

            // 発射方向
            float3 direction = math.mul(launcherTransform.Rotation, new float3(0f, 0f, 1f));

            // Physics 復元
            SystemAPI.SetComponent(entity, cache.CachedMass);
            SystemAPI.SetComponent(entity, cache.CachedDamping);

            // 速度付与
            SystemAPI.SetComponent(entity, new PhysicsVelocity
            {
                Linear = direction * settings.Speed,
                Angular = float3.zero
            });

            // プール解除
            SystemAPI.SetComponentEnabled<PinballPoolState>(entity, true);

            entities.Dispose();
        }
    }
}