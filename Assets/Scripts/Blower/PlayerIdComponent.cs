// ======================================================
// PlayerIdComponent.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-27
// 更新日時 : 2026-06-27
// 概要     : プレイヤー識別用 ID コンポーネント
// ======================================================

using Unity.Entities;

namespace BlowerSystem
{
    /// <summary>
    /// プレイヤーを識別するための ID
    /// </summary>
    public struct PlayerIdComponent : IComponentData
    {
        /// <summary>
        /// プレイヤー ID 番号
        /// </summary>
        public int Value;
    }
}