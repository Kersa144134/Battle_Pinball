// ======================================================
// PinballPhysicsDataAuthoring.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-07-02
// 更新日時 : 2026-07-06
// 概要     : ピンボール物理データを ECS コンポーネントへ変換する Authoring
// ======================================================

using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace PinballSystem
{
    /// <summary>
    /// ピンボールの初期物理情報を ECS コンポーネントへ変換するための Authoring クラス
    /// </summary>
    public sealed class PinballPhysicsDataAuthoring : MonoBehaviour
    {
        // ======================================================
        // Baker
        // ======================================================

        /// <summary>
        /// Authoring の情報を ECS コンポーネントへ変換する Baker
        /// </summary>
        private sealed class Baker : Baker<PinballPhysicsDataAuthoring>
        {
            // ======================================================
            // Baker 実装
            // ======================================================

            /// <summary>
            /// ピンボールの初期物理情報を Entity に追加する
            /// </summary>
            /// <param name="authoring">変換元の Authoring</param>
            public override void Bake(PinballPhysicsDataAuthoring authoring)
            {
                // Authoring を持つ Entity を取得する
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                // Rigidbody を取得する
                Rigidbody rigidbody = authoring.GetComponent<Rigidbody>();

                // Transform を取得する
                Transform transform = authoring.transform;

                // ピンボール物理データコンポーネントを追加する
                AddComponent(entity, new PinballPhysicsData
                {
                    Position = (float3)transform.localPosition,
                    Rotation = (quaternion)transform.localRotation,
                    Scale = transform.localScale.x,

                    // 初期速度、初期角速度はゼロとする
                    LinearVelocity = float3.zero,
                    AngularVelocity = float3.zero,

                    Mass = rigidbody.mass,
                    LinearDamping = rigidbody.linearDamping,
                    AngularDamping = rigidbody.angularDamping
                });
            }
        }
    }
}