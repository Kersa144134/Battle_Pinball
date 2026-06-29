// ======================================================
// PinballTagAuthoring.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-27
// 更新日時 : 2026-06-27
// 概要     : ピンボールタグを ECS コンポーネントへ変換する Authoring
// ======================================================

using Unity.Entities;
using UnityEngine;

namespace BallSystem
{
    /// <summary>
    /// ピンボールタグを付与するための Authoring クラス
    /// </summary>
    public sealed class PinballTagAuthoring : MonoBehaviour
    {
        // ======================================================
        // Baker
        // ======================================================

        /// <summary>
        /// Authoring の情報を ECS コンポーネントへ変換する Baker
        /// </summary>
        private sealed class Baker : Baker<PinballTagAuthoring>
        {
            // ======================================================
            // Baker 実装
            // ======================================================

            /// <summary>
            /// ピンボールタグを Entity に追加する
            /// </summary>
            /// <param name="authoring">変換元の Authoring</param>
            public override void Bake(PinballTagAuthoring authoring)
            {
                // Authoring を持つ Entity を取得する
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                // ピンボールタグを追加する
                AddComponent<PinballTag>(entity);
            }
        }
    }
}