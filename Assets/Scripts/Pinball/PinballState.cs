// ======================================================
// PinballState.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-07-02
// 更新日時 : 2026-07-02
// 概要     : ピンボールの状態を保持する ECS コンポーネント
// ======================================================

using Unity.Entities;

namespace PinballSystem
{
    /// <summary>
    /// ピンボールの状態を保持するコンポーネント
    /// プール状態・発射状態・停止状態などを数値で管理する
    /// </summary>
    public struct PinballState : IComponentData
    {
        /// <summary>
        /// ピンボールの状態値
        /// </summary>
        public byte State;
    }
}