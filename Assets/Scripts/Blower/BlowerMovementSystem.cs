// ======================================================
// BlowerMovementSystem.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-27
// 更新日時 : 2026-06-27
// 概要     : ブロワーの入力方向に応じて移動を適用する ECS システム
// ======================================================

using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace BlowerSystem
{
    /// <summary>
    /// MoveInputData と MoveSpeedComponent を基に Z 軸移動のみを行う
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct BlowerMovementSystem : ISystem
    {
        // ======================================================
        // 定数
        // ======================================================

        /// <summary>
        /// 最低保証速度
        /// </summary>
        private const float DEFAULT_SPEED = 1.0f;

        /// <summary>
        /// 移動範囲制限 最小値
        /// </summary>
        private const float MOVE_LIMIT_MIN = -2.75f;

        /// <summary>
        /// 移動範囲制限 最大値
        /// </summary>
        private const float MOVE_LIMIT_MAX = 2.75f;

        // ======================================================
        // ISystem 実装
        // ======================================================

        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;

            // --------------------------------------------------
            // ECS 走査
            // --------------------------------------------------
            foreach (var (transform, input, speed) in
                SystemAPI.Query<
                    RefRW<LocalTransform>,
                    RefRO<BlowerMoveData>,
                    RefRO<MoveSpeedComponent>>())
            {
                // Z 軸入力のみ取得する
                float moveInputZ = input.ValueRO.Direction;

                // 速度取得
                float moveSpeed = math.max(speed.ValueRO.Value, DEFAULT_SPEED);

                // 移動量計算
                float deltaZ = moveInputZ * moveSpeed * deltaTime;

                // 現在位置取得
                float3 position = transform.ValueRW.Position;

                // Z 軸更新のみ実行
                position.z += deltaZ;

                // 移動範囲制限
                position.z = math.clamp(position.z, MOVE_LIMIT_MIN, MOVE_LIMIT_MAX);

                // 位置反映
                transform.ValueRW.Position = position;
            }
        }
    }
}