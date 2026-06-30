// ======================================================
// PinballPoolSystem.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-30
// 更新日時 : 2026-06-30
// 概要     : ピンボールをプール状態へ戻し非アクティブ化する ECS システム
// ======================================================

using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

namespace BallSystem
{
    /// <summary>
    /// ピンボールをプール状態へ戻すシステム
    /// 発射待機状態として物理および位置を初期化する
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct PinballPoolSystem : ISystem
    {
        // ======================================================
        // 定数
        // ======================================================

        /// <summary>
        /// プール待機位置
        /// Y 軸上空に退避させることでゲーム空間から除外する
        /// </summary>
        private static readonly float3 POOL_POSITION =
            new float3(0f, 100f, 0f);

        // ======================================================
        // ISystem 実装
        // ======================================================

        /// <summary>
        /// プール対象のピンボールを初期化状態へ戻す
        /// </summary>
        /// <param name="state">システム状態</param>
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // プール対象のみを走査する
            foreach (var (transform, velocity)
                     in SystemAPI.Query<
                         RefRW<LocalTransform>,
                         RefRW<PhysicsVelocity>>()
                         .WithAll<PinballPoolState>())
            {
                // 位置をプール位置へ固定
                transform.ValueRW.Position = POOL_POSITION;

                // 回転は維持しつつ速度のみ初期化
                velocity.ValueRW.Linear = float3.zero;
                velocity.ValueRW.Angular = float3.zero;
            }
        }
    }
}