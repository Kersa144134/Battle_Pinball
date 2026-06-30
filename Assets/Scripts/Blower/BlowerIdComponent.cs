// ======================================================
// BlowerIdComponent.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-27
// 更新日時 : 2026-06-27
// 概要     : ブロワー識別用 ID コンポーネント
// ======================================================

using Unity.Entities;

namespace BlowerSystem
{
    /// <summary>
    /// ブロワーを識別するための ID
    /// </summary>
    public struct BlowerIdComponent : IComponentData
    {
        /// <summary>
        /// ブロワー ID 番号
        /// </summary>
        public int Value;
    }
}