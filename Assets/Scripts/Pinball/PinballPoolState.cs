// ======================================================
// PinballPoolState.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-30
// 更新日時 : 2026-06-30
// 概要     : ピンボールのプール状態を管理する ECS コンポーネント
// ======================================================

using Unity.Entities;

namespace BallSystem
{
    /// <summary>
    /// ピンボールのプール状態を管理するコンポーネント
    /// Enable / Disable によりアクティブ状態を制御する
    /// </summary>
    public struct PinballPoolState : IComponentData, IEnableableComponent
    {
        /// <summary>
        /// アクティブ状態フラグ
        /// </summary>
        public bool IsActive;
    }
}