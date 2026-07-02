// ======================================================
// LauncherSystem.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-30
// 更新日時 : 2026-07-02
// 概要     : ピンボールをプール状態から発射する ECS システム
// ======================================================

using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using BallSystem;

namespace LauncherSystem
{
    /// <summary>
    /// プール状態のピンボールを発射するシステム
    /// 各ランチャーの発射条件に応じてプールからボールを消費する
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct LauncherSystem : ISystem
    {
        // ======================================================
        // ISystem 実装
        // ======================================================

        public void OnUpdate(ref SystemState state)
        {
            // --------------------------------------------------
            // プール取得
            // --------------------------------------------------
            DynamicBuffer<PinballPoolEntry> pool =
                SystemAPI.GetSingletonBuffer<PinballPoolEntry>();

            // --------------------------------------------------
            // 各ランチャーを更新
            // --------------------------------------------------
            foreach (var (launcherSettings, launcherState)
                in SystemAPI.Query<
                    RefRO<LauncherSettings>,
                    RefRW<LauncherState>>())
            {
                // 経過時間更新
                launcherState.ValueRW.ElapsedTime += SystemAPI.Time.DeltaTime;

                // 発射条件未達の場合はスキップ
                if (launcherState.ValueRO.ElapsedTime <
                    launcherSettings.ValueRO.Interval)
                {
                    continue;
                }

                // タイマーリセット
                launcherState.ValueRW.ElapsedTime = 0f;

                // --------------------------------------------------
                // プール空チェック
                // --------------------------------------------------
                if (pool.Length == 0)
                {
                    break;
                }

                // --------------------------------------------------
                // 発射対象取得
                // --------------------------------------------------
                Entity entity = pool[0].Entity;

                // キューから取り出し
                pool.RemoveAt(0);

                // --------------------------------------------------
                // 発射元 Transform 取得
                // --------------------------------------------------
                LocalTransform launcherTransform =
                    SystemAPI.GetComponent<LocalTransform>(
                        launcherSettings.ValueRO.LauncherEntity);

                // --------------------------------------------------
                // 物理キャッシュ取得
                // --------------------------------------------------
                PinballPhysicsCache cache =
                    SystemAPI.GetComponent<PinballPhysicsCache>(entity);

                // --------------------------------------------------
                // Transform 再配置
                // --------------------------------------------------
                SystemAPI.SetComponent(entity, new LocalTransform
                {
                    Position = launcherTransform.Position,
                    Rotation = launcherTransform.Rotation,
                    Scale = cache.CachedScale
                });

                // --------------------------------------------------
                // 発射方向計算
                // --------------------------------------------------
                float3 direction =
                    math.mul(
                        launcherTransform.Rotation,
                        new float3(0f, 0f, 1f));

                // --------------------------------------------------
                // 物理状態復元
                // --------------------------------------------------
                SystemAPI.SetComponent(entity, cache.CachedMass);
                SystemAPI.SetComponent(entity, cache.CachedDamping);

                // --------------------------------------------------
                // 初速度付与
                // --------------------------------------------------
                SystemAPI.SetComponent(entity, new PhysicsVelocity
                {
                    Linear = direction * launcherSettings.ValueRO.Speed,
                    Angular = float3.zero
                });

                // --------------------------------------------------
                // プール状態解除
                // --------------------------------------------------
                SystemAPI.SetComponentEnabled<PinballState>(entity, true);
            }
        }
    }
}