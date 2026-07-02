// ======================================================
// PinballPhysicsCacheAuthoring.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-07-02
// 更新日時 : 2026-07-02
// 概要     : ピンボール物理キャッシュを ECS コンポーネントへ変換する Authoring
// ======================================================

using Unity.Entities;
using UnityEngine;

namespace BallSystem
{
    /// <summary>
    /// ピンボールの物理状態キャッシュを付与するための Authoring クラス
    /// </summary>
    public sealed class PinballPhysicsCacheAuthoring : MonoBehaviour
    {
        // ======================================================
        // Baker
        // ======================================================

        /// <summary>
        /// Authoring の情報を ECS コンポーネントへ変換する Baker
        /// </summary>
        private sealed class Baker : Baker<PinballPhysicsCacheAuthoring>
        {
            // ======================================================
            // Baker 実装
            // ======================================================

            /// <summary>
            /// ピンボール物理キャッシュを Entity に追加する
            /// </summary>
            /// <param name="authoring">変換元の Authoring</param>
            public override void Bake(PinballPhysicsCacheAuthoring authoring)
            {
                // Authoring を持つ Entity を取得する
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                // ピンボール物理キャッシュコンポーネントを追加する
                AddComponent<PinballPhysicsCache>(entity);
            }
        }
    }
}