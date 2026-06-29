// ======================================================
// BlowerForceSettings.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-27
// 更新日時 : 2026-06-27
// 概要     : ブロワーの設定を保持する ECS コンポーネント
// ======================================================

using Unity.Entities;
using Unity.Mathematics;

namespace BlowerSystem
{
    /// <summary>
    /// ブロワーの設定を保持するコンポーネント
    /// </summary>
    public struct BlowerForceSettings : IComponentData
    {
        // ======================================================
        // 設定
        // ======================================================

        /// <summary>
        /// ブロワーがオブジェクトへ加える力
        /// </summary>
        public float3 Force;
    }
}