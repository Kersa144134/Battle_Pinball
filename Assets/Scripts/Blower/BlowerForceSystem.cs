// ======================================================
// BlowerSystem.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-27
// 更新日時 : 2026-06-27
// 概要     : ブロワーによる力の付与を行う ECS システム
// ======================================================

using BallSystem;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;

namespace BlowerSystem
{
    /// <summary>
    /// ブロワーと接触したオブジェクトへ力を加えるシステム
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
    public partial struct BlowerForceSystem : ISystem
    {
        // ======================================================
        // ISystem 実装
        // ======================================================

        /// <summary>
        /// ブロワーの力を適用する Job を実行する
        /// </summary>
        /// <param name="state">システムの状態</param>
        public void OnUpdate(ref SystemState state)
        {
            // Physics シミュレーションを取得する
            var simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();

            // ブロワー処理を実行する Job を生成する
            BlowerJob blowerJob = new BlowerJob()
            {
                // ブロワー設定を取得するための Lookup を設定する
                BlowerSettingsLookup = SystemAPI.GetComponentLookup<BlowerForceSettings>(true),

                // ピンボール状態を取得するための Lookup を設定する
                PinballStateLookup = SystemAPI.GetComponentLookup<PinballState>(true),

                // PhysicsMass を取得するための Lookup を設定する
                PhysicsMassLookup = SystemAPI.GetComponentLookup<PhysicsMass>(true),

                // PhysicsVelocity を取得するための Lookup を設定する
                PhysicsVelocityLookup = SystemAPI.GetComponentLookup<PhysicsVelocity>(),

                // 現在フレームの DeltaTime を設定する
                DeltaTime = SystemAPI.Time.DeltaTime
            };

            // Job をスケジュールする
            state.Dependency = blowerJob.Schedule(
                simulationSingleton,
                state.Dependency);
        }
    }

    /// <summary>
    /// Trigger に接触したオブジェクトへブロワーの力を加える Job
    /// </summary>
    [BurstCompile]
    public struct BlowerJob : ITriggerEventsJob
    {
        // ======================================================
        // コンポーネント参照
        // ======================================================

        /// <summary>
        /// ブロワー設定を取得する Lookup
        /// </summary>
        [ReadOnly]
        public ComponentLookup<BlowerForceSettings> BlowerSettingsLookup;

        /// <summary>
        /// ピンボール状態を取得する Lookup
        /// </summary>
        [ReadOnly]
        public ComponentLookup<PinballState> PinballStateLookup;

        /// <summary>
        /// PhysicsMass を取得する Lookup
        /// </summary>
        [ReadOnly]
        public ComponentLookup<PhysicsMass> PhysicsMassLookup;

        /// <summary>
        /// PhysicsVelocity を取得する Lookup
        /// </summary>
        public ComponentLookup<PhysicsVelocity> PhysicsVelocityLookup;

        /// <summary>
        /// フレーム時間
        /// </summary>
        public float DeltaTime;

        // ======================================================
        // ITriggerEventsJob 実装
        // ======================================================

        /// <summary>
        /// Trigger 接触時の処理を実行する
        /// </summary>
        /// <param name="triggerEvent">Trigger イベント</param>
        public void Execute(TriggerEvent triggerEvent)
        {
            // EntityA がブロワーか判定する
            bool isEntityABlower = BlowerSettingsLookup.HasComponent(triggerEvent.EntityA);

            // EntityB がブロワーか判定する
            bool isEntityBBlower = BlowerSettingsLookup.HasComponent(triggerEvent.EntityB);

            // ブロワーとピンボールの組み合わせ以外は処理しない
            if (!(isEntityABlower ^ isEntityBBlower))
            {
                return;
            }

            // ブロワー Entity を取得する
            Entity blowerEntity;

            // ピンボール Entity を取得する
            Entity pinballEntity;

            // EntityA がブロワーの場合
            if (isEntityABlower)
            {
                // EntityB がピンボールでない場合は処理しない
                if (!PinballStateLookup.HasComponent(triggerEvent.EntityB))
                {
                    return;
                }


                // Entity を設定する
                blowerEntity = triggerEvent.EntityA;
                pinballEntity = triggerEvent.EntityB;
            }
            // EntityB がブロワーの場合
            else
            {
                // EntityA がピンボールでない場合は処理しない
                if (!PinballStateLookup.HasComponent(triggerEvent.EntityA))
                {
                    return;
                }

                // Entity を設定する
                blowerEntity = triggerEvent.EntityB;
                pinballEntity = triggerEvent.EntityA;
            }

            // ブロワー設定を取得する
            BlowerForceSettings blowerSettings = BlowerSettingsLookup[blowerEntity];

            // ピンボールの質量を取得する
            PhysicsMass physicsMass = PhysicsMassLookup[pinballEntity];

            // ピンボールの速度コンポーネントを取得する
            var physicsVelocity = PhysicsVelocityLookup.GetRefRW(pinballEntity);

            // ブロワーの力をピンボールへ加える
            physicsVelocity.ValueRW.ApplyLinearImpulse(
                physicsMass,
                blowerSettings.Force * DeltaTime);
        }
    }
}