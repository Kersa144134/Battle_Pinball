// ======================================================
// BumperCollisionSystem.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-30
// 更新日時 : 2026-06-30
// 概要     : バンパーと衝突したピンボールへ反発速度を与える ECS システム
// ======================================================

using BallSystem;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace BumperSystem
{
    /// <summary>
    /// バンパーとピンボールの Collision を処理するシステム
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
    public partial struct BumperCollisionSystem : ISystem
    {
        // ======================================================
        // ISystem 実装
        // ======================================================

        /// <summary>
        /// Trigger Job を実行する
        /// </summary>
        /// <param name="state">システム状態</param>
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Physics シミュレーションを取得する
            SimulationSingleton simulationSingleton =
                SystemAPI.GetSingleton<SimulationSingleton>();

            // Trigger Job を生成する
            BumperCollisionJob bumperCollisionJob =
                new BumperCollisionJob
                {
                    // バンパー設定を取得する
                    BumperSettingsLookup =
                        SystemAPI.GetComponentLookup<BumperCollisionSettings>(true),

                    // ピンボール状態を取得する
                    PinballStateLookup =
                        SystemAPI.GetComponentLookup<PinballState>(true),

                    // 座標情報を取得する
                    LocalTransformLookup =
                        SystemAPI.GetComponentLookup<LocalTransform>(true),

                    // 速度情報を取得する
                    PhysicsVelocityLookup =
                        SystemAPI.GetComponentLookup<PhysicsVelocity>()
                };

            // Job をスケジュールする
            state.Dependency =
                bumperCollisionJob.Schedule(
                    simulationSingleton,
                    state.Dependency);
        }
    }

    /// <summary>
    /// バンパー Trigger 処理
    /// </summary>
    [BurstCompile]
    public struct BumperCollisionJob : ITriggerEventsJob
    {
        // ======================================================
        // コンポーネント参照
        // ======================================================

        /// <summary>
        /// バンパー設定 Lookup
        /// </summary>
        [ReadOnly]
        public ComponentLookup<BumperCollisionSettings> BumperSettingsLookup;

        /// <summary>
        /// ピンボール状態 Lookup
        /// </summary>
        [ReadOnly]
        public ComponentLookup<PinballState> PinballStateLookup;

        /// <summary>
        /// 座標 Lookup
        /// </summary>
        [ReadOnly]
        public ComponentLookup<LocalTransform> LocalTransformLookup;

        /// <summary>
        /// 速度 Lookup
        /// </summary>
        public ComponentLookup<PhysicsVelocity> PhysicsVelocityLookup;

        // ======================================================
        // ICollisionEventsJob 実装
        // ======================================================

        /// <summary>
        /// Trigger 発生時の処理
        /// </summary>
        /// <param name="triggerEvent">Trigger 情報</param>
        [BurstCompile]
        public void Execute(TriggerEvent triggerEvent)
        {
            // Entity A がバンパーか判定する
            bool isEntityABumper =
                BumperSettingsLookup.HasComponent(triggerEvent.EntityA);

            // Entity B がバンパーか判定する
            bool isEntityBBumper =
                BumperSettingsLookup.HasComponent(triggerEvent.EntityB);

            // バンパーとピンボールの組み合わせ以外は処理しない
            if (!(isEntityABumper ^ isEntityBBumper))
            {
                return;
            }

            // バンパー Entity
            Entity bumperEntity;

            // ピンボール Entity
            Entity pinballEntity;

            // Entity A がバンパーの場合
            if (isEntityABumper)
            {
                // Entity B がピンボールでない場合は終了する
                if (!PinballStateLookup.HasComponent(triggerEvent.EntityB))
                {
                    return;
                }

                // Entity を設定する
                bumperEntity = triggerEvent.EntityA;
                pinballEntity = triggerEvent.EntityB;
            }
            else
            {
                // Entity A がピンボールでない場合は終了する
                if (!PinballStateLookup.HasComponent(triggerEvent.EntityA))
                {
                    return;
                }

                // Entity を設定する
                bumperEntity = triggerEvent.EntityB;
                pinballEntity = triggerEvent.EntityA;
            }

            // バンパー設定を取得する
            BumperCollisionSettings bumperSettings =
                BumperSettingsLookup[bumperEntity];

            // バンパー座標を取得する
            LocalTransform bumperTransform =
                LocalTransformLookup[bumperEntity];

            // ピンボール座標を取得する
            LocalTransform pinballTransform =
                LocalTransformLookup[pinballEntity];

            // バンパー中心からピンボールへの法線を算出する
            float3 normal =
                math.normalize(
                    pinballTransform.Position -
                    bumperTransform.Position);

            // ピンボール速度を取得する
            RefRW<PhysicsVelocity> physicsVelocity =
                PhysicsVelocityLookup.GetRefRW(pinballEntity);

            // バンパー中心から外向きの方向を正規化する
            float3 direction =
                math.normalize(
                    pinballTransform.Position -
                    bumperTransform.Position);

            // 現在速度を完全に上書きする
            physicsVelocity.ValueRW.Linear =
                direction *
                bumperSettings.Power;
        }
    }
}