// ======================================================
// PinballPoolEntry.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-07-02
// 更新日時 : 2026-07-02
// 概要     : ピンボールプールの登録情報を保持する DynamicBuffer 要素
// ======================================================

using Unity.Entities;

namespace BallSystem
{
    /// <summary>
    /// ピンボールプールへ登録されている Entity を保持する
    /// DynamicBuffer の要素
    /// </summary>
    public struct PinballPoolEntry : IBufferElementData
    {
        /// <summary>
        /// プール対象のピンボール Entity
        /// </summary>
        public Entity Entity;
    }
}