// ======================================================
// PinballLaunchSettings.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-30
// 更新日時 : 2026-06-30
// 概要     : ピンボール発射パラメータ設定コンポーネント
// ======================================================

using Unity.Entities;

namespace BallSystem
{
    /// <summary>
    /// ピンボール発射に関する設定
    /// </summary>
    public struct PinballLaunchSettings : IComponentData
    {
        /// <summary>
        /// 発射元エンティティ
        /// </summary>
        public Entity LauncherEntity;
        
        /// <summary>
        /// /// 発射間隔秒
        /// /// </summary>
        public float Interval;

        /// <summary>
        /// 発射速度
        /// </summary>
        public float Speed;
    }
}