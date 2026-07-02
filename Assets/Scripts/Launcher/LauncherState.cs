// ======================================================
// LauncherState.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-07-02
// 更新日時 : 2026-07-02
// 概要     : ランチャーの状態を管理する ECS コンポーネント
// ======================================================

using Unity.Entities;

namespace LauncherSystem
{
    /// <summary>
    /// ランチャーの状態を管理するコンポーネント
    /// Enable / Disable によりアクティブ状態を制御する
    /// </summary>
    public struct LauncherState : IComponentData, IEnableableComponent
    {
        /// <summary>
        /// ランチャーのアクティブ状態
        /// </summary>
        public bool IsActive;

        /// <summary>
        /// 前回発射からの経過時間
        /// 発射間隔の判定に使用する
        /// </summary>
        public float ElapsedTime;
    }
}