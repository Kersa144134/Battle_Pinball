// ======================================================
// PinballState.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-07-02
// 更新日時 : 2026-07-02
// 概要     : ピンボールの状態を保持する ECS コンポーネント
// ======================================================

using Unity.Entities;

namespace BallSystem
{
    /// <summary>
    /// ピンボールの状態を保持するコンポーネント
    /// Enable / Disable により発射状態を制御する
    /// </summary>
    public struct PinballState : IComponentData, IEnableableComponent
    {
        /// <summary>
        /// ピンボールを識別する ID
        /// 将来的な検索やデバッグ用途で使用する
        /// </summary>
        public int Id;
    }
}