// ======================================================
// PinballPhysicsCache.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-30
// 更新日時 : 2026-06-30
// 概要     : ピンボールの物理状態を退避・復元するためのキャッシュコンポーネント
// ======================================================

using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace PinballSystem
{
    /// <summary>
    /// ピンボールの物理状態を一時保存するコンポーネント
    /// プール時に状態を退避し、発射時に復元する用途で使用する
    /// </summary>
    public struct PinballPhysicsCache : IComponentData
    {
        // ======================================================
        // Transform 情報
        // ======================================================

        /// <summary>
        /// プール移動前の位置
        /// 発射時の初期位置補正やデバッグ用途で使用する
        /// </summary>
        public float3 CachedPosition;

        /// <summary>
        /// プール移動前の回転
        /// 発射方向基準の補助として使用する
        /// </summary>
        public quaternion CachedRotation;

        /// <summary>
        /// プール移動前のスケール
        /// 発射時に元の見た目を復元するために使用する
        /// </summary>
        public float CachedScale;
        
        // ======================================================
        // 速度情報
        // ======================================================

        /// <summary>
        /// 発射前の直線速度
        /// 物理停止前の移動量を保持し、復元や補正に利用する
        /// </summary>
        public float3 CachedLinearVelocity;

        /// <summary>
        /// 発射前の角速度
        /// 回転運動の保持・復元に使用する
        /// </summary>
        public float3 CachedAngularVelocity;

        // ======================================================
        // 物理情報
        // ======================================================

        /// <summary>
        /// 質量情報
        /// プール時に無効化した物理を復元するために使用する
        /// </summary>
        public PhysicsMass CachedMass;

        /// <summary>
        /// 減衰設定
        /// プール解除時に元状態へ戻すために使用する
        /// </summary>
        public PhysicsDamping CachedDamping;
    }
}