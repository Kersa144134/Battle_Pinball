// ======================================================
// PinballStateSaveData.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-27
// 更新日時 : 2026-06-27
// 概要     : ピンボールの状態保存・復元に使用する入力情報を保持するコンポーネント
// ======================================================

using Unity.Entities;
using UnityEngine.InputSystem;

namespace BallSystem
{
    /// <summary>
    /// ピンボールの状態保存・復元に使用する入力情報を保持するコンポーネント
    /// </summary>
    public sealed class PinballStateSaveData : IComponentData
    {
        // ======================================================
        // 入力設定
        // ======================================================

        /// <summary>
        /// 状態を保存する入力アクション
        /// </summary>
        public InputAction SaveAction;

        /// <summary>
        /// 保存した状態を復元する入力アクション
        /// </summary>
        public InputAction LoadAction;
    }
}