// ======================================================
// PinballPhysicsData.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-30
// 更新日時 : 2026-07-06
// 概要     : ピンボールの物理状態を退避・復元するためのデータコンポーネント
// ======================================================

using Unity.Entities;
using Unity.Mathematics;

namespace PinballSystem
{
    /// <summary>
    /// ピンボールの初期物理状態を保持するデータコンポーネント
    /// プール解除時に物理設定を復元するために使用する
    /// </summary>
    public struct PinballPhysicsData : IComponentData
    {
        // --------------------------------------------------
        // Transform 情報
        // --------------------------------------------------
        /// <summary>
        /// 初期ローカル座標
        /// プール解除時に元の位置へ復元するために使用する
        /// </summary>
        public float3 Position;

        /// <summary>
        /// 初期回転
        /// プール解除時に元の姿勢へ復元するために使用する
        /// </summary>
        public quaternion Rotation;

        /// <summary>
        /// 初期スケール
        /// プール解除時に元の見た目を維持するために使用する
        /// </summary>
        public float Scale;

        // --------------------------------------------------
        // 速度情報
        // --------------------------------------------------
        /// <summary>
        /// 初期直線速度
        /// 発射時の物理速度を復元するために使用する
        /// </summary>
        public float3 LinearVelocity;

        /// <summary>
        /// 初期角速度
        /// 発射時の回転速度を復元するために使用する
        /// </summary>
        public float3 AngularVelocity;

        // --------------------------------------------------
        // Rigidbody 情報
        // --------------------------------------------------
        /// <summary>
        /// Rigidbody の質量
        /// PhysicsMass を再構築するために使用する
        /// </summary>
        public float Mass;

        /// <summary>
        /// Rigidbody の直線減衰
        /// プール解除時に減衰設定を復元するために使用する
        /// </summary>
        public float LinearDamping;

        /// <summary>
        /// Rigidbody の角減衰
        /// プール解除時に減衰設定を復元するために使用する
        /// </summary>
        public float AngularDamping;
    }
}