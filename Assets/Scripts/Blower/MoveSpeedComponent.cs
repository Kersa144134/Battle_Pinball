// ======================================================
// MoveSpeedComponent.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-27
// 更新日時 : 2026-06-27
// 概要     : ブロワーの移動速度を保持する ECS コンポーネント
// ======================================================

using Unity.Entities;

namespace BlowerSystem
{
    /// <summary>
    /// 移動速度データ
    /// </summary>
    public struct MoveSpeedComponent : IComponentData
    {
        /// <summary>
        /// 移動速度
        /// </summary>
        public float Value;
    }
}