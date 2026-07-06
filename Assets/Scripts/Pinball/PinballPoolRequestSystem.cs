// ======================================================
// PinballPoolRequestSystem.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-07-06
// 更新日時 : 2026-07-06
// 概要     : プール領域へ接触したピンボールへプール要求を付与する ECS システム
// ======================================================

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

namespace PinballSystem
{
    /// <summary>
    /// プール領域へ接触したピンボールへ
    /// プール要求を付与するシステム
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
    public partial struct PinballPoolRequestSystem : ISystem
    {
        // ======================================================
        // ISystem 実装
        // ======================================================

        /// <summary>
        /// Trigger 判定 Job を実行する
        /// </summary>
        /// <param name="state">システム状態</param>
        public void OnUpdate(ref SystemState state)
        {
            // Physics シミュレーションを取得する
            SimulationSingleton simulationSingleton =
                SystemAPI.GetSingleton<SimulationSingleton>();

            // EntityCommandBuffer を生成する
            EntityCommandBuffer ecb =
                new EntityCommandBuffer(state.WorldUpdateAllocator);

            // Trigger 判定 Job を生成する
            PinballPoolRequestJob job =
                new PinballPoolRequestJob
                {
                    // プール領域タグ参照を設定する
                    PoolZoneLookup =
                        SystemAPI.GetComponentLookup<PinballPoolZoneTag>(true),

                    // ピンボール状態参照を設定する
                    PinballStateLookup =
                        SystemAPI.GetComponentLookup<PinballState>(true),

                    // EntityCommandBuffer を設定する
                    ECB =
                        ecb.AsParallelWriter()
                };

            // Job をスケジュールする
            state.Dependency =
                job.Schedule(
                    simulationSingleton,
                    state.Dependency);

            // Job 完了を待機する
            state.Dependency.Complete();

            // 構造変更を反映する
            ecb.Playback(state.EntityManager);
        }
    }

    /// <summary>
    /// Trigger 接触時にプール要求を付与する Job
    /// </summary>
    [BurstCompile]
    public struct PinballPoolRequestJob : ITriggerEventsJob
    {
        // ======================================================
        // コンポーネント参照
        // ======================================================

        /// <summary>
        /// プール領域タグ参照
        /// </summary>
        [ReadOnly]
        public ComponentLookup<PinballPoolZoneTag> PoolZoneLookup;

        /// <summary>
        /// ピンボール状態参照
        /// </summary>
        [ReadOnly]
        public ComponentLookup<PinballState> PinballStateLookup;

        /// <summary>
        /// 構造変更用 EntityCommandBuffer
        /// </summary>
        public EntityCommandBuffer.ParallelWriter ECB;

        // ======================================================
        // ITriggerEventsJob 実装
        // ======================================================

        /// <summary>
        /// Trigger 接触時の処理
        /// </summary>
        /// <param name="triggerEvent">Trigger イベント</param>
        public void Execute(TriggerEvent triggerEvent)
        {
            // EntityA がプール領域か判定する
            bool isEntityAPool =
                PoolZoneLookup.HasComponent(triggerEvent.EntityA);

            // EntityB がプール領域か判定する
            bool isEntityBPool =
                PoolZoneLookup.HasComponent(triggerEvent.EntityB);

            // プール領域とピンボール以外の組み合わせは処理しない
            if (!(isEntityAPool ^ isEntityBPool))
            {
                return;
            }

            // プールへ戻す対象 Entity
            Entity pinballEntity;

            // EntityA がプール領域の場合
            if (isEntityAPool)
            {
                // EntityB がピンボールでなければ終了する
                if (!PinballStateLookup.HasComponent(triggerEvent.EntityB))
                {
                    return;
                }

                // ピンボール Entity を設定する
                pinballEntity =
                    triggerEvent.EntityB;
            }
            // EntityB がプール領域の場合
            else
            {
                // EntityA がピンボールでなければ終了する
                if (!PinballStateLookup.HasComponent(triggerEvent.EntityA))
                {
                    return;
                }

                // ピンボール Entity を設定する
                pinballEntity =
                    triggerEvent.EntityA;
            }

            // 既にプール状態の場合は処理しない
            if (PinballStateLookup[pinballEntity].State ==
                PinballStateType.Pool)
            {
                return;
            }

            // プール要求を付与する
            ECB.AddComponent(
                0,
                pinballEntity,
                new PinballRequestPool());
        }
    }
}