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
using PinballSystem;

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

            // プールが存在しないなら処理なし
            if (pool.Length == 0)
            {
                return;
            }

            // --------------------------------------------------
            // ランチャー更新
            // --------------------------------------------------
            foreach (var (launcherSettings, launcherState)
                in SystemAPI.Query<
                    RefRO<LauncherSettings>,
                    RefRW<LauncherState>>())
            {
                // 経過時間更新
                launcherState.ValueRW.ElapsedTime += SystemAPI.Time.DeltaTime;

                // 発射条件未達
                if (launcherState.ValueRO.ElapsedTime <
                    launcherSettings.ValueRO.Interval)
                {
                    continue;
                }

                // タイマーリセット
                launcherState.ValueRW.ElapsedTime = 0f;

                // --------------------------------------------------
                // 発射対象取得
                // --------------------------------------------------
                // プールが空の場合は処理なし
                if (pool.Length == 0)
                {
                    continue;
                }

                Entity entity = pool[pool.Length - 1].Entity;

                // 取得した要素をプールから削除
                pool.RemoveAt(pool.Length - 1);

                // ピンボール物理データ取得
                PinballPhysicsData physics =
                    SystemAPI.GetComponent<PinballPhysicsData>(entity);

                // --------------------------------------------------
                // ピンボールTransformを発射位置へ更新
                // --------------------------------------------------
                // ランチャーの Transform 取得
                LocalTransform launcherTransform =
                    SystemAPI.GetComponent<LocalTransform>(
                        launcherSettings.ValueRO.LauncherEntity);

                // 発射時にランチャー位置へ同期させる
                SystemAPI.SetComponent(entity, new LocalTransform
                {
                    Position = launcherTransform.Position,
                    Rotation = launcherTransform.Rotation,
                    Scale = physics.Scale
                });

                // --------------------------------------------------
                // 発射方向を計算
                // --------------------------------------------------
                // ランチャー前方ベクトルをワールド空間へ変換
                float3 direction =
                    math.mul(
                        launcherTransform.Rotation,
                        new float3(0f, 0f, 1f));

                // --------------------------------------------------
                // 物理状態の復元
                // --------------------------------------------------
                // PhysicsCollider を取得する
                PhysicsCollider physicsCollider =
                    SystemAPI.GetComponent<PhysicsCollider>(entity);

                // PhysicsMass を復元する
                SystemAPI.SetComponent(
                    entity,
                    PhysicsMass.CreateDynamic(
                        physicsCollider.MassProperties,
                        physics.Mass));

                // PhysicsDamping を復元する
                SystemAPI.SetComponent(entity, new PhysicsDamping
                {
                    Linear = physics.LinearDamping,
                    Angular = physics.AngularDamping
                });

                // --------------------------------------------------
                // 発射速度を付与
                // --------------------------------------------------
                // 発射方向へ発射速度を与える
                SystemAPI.SetComponent(entity, new PhysicsVelocity
                {
                    Linear = direction * launcherSettings.ValueRO.Speed,
                    Angular = float3.zero
                });

                // --------------------------------------------------
                // 状態設定
                // --------------------------------------------------
                // 状態をアクティブ状態に設定する
                SystemAPI.SetComponent(entity,
                    new PinballState
                    {
                        State = PinballStateType.Active
                    });
            }
        }
    }
}