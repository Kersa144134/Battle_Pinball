// ======================================================
// PinballDataAuthoring.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-27
// 更新日時 : 2026-07-02
// 概要     : ピンボール状態を ECS コンポーネントへ変換する Authoring
// ======================================================

using Unity.Entities;
using UnityEngine;

namespace BallSystem
{
    /// <summary>
    /// ピンボール状態を付与するための Authoring クラス
    /// </summary>
    public sealed class PinballStateAuthoring : MonoBehaviour
    {
        // ======================================================
        // Baker
        // ======================================================

        /// <summary>
        /// Authoring の情報を ECS コンポーネントへ変換する Baker
        /// </summary>
        private sealed class Baker : Baker<PinballStateAuthoring>
        {
            // ======================================================
            // Baker 実装
            // ======================================================

            /// <summary>
            /// ピンボール状態を Entity に追加する
            /// </summary>
            /// <param name="authoring">変換元の Authoring</param>
            public override void Bake(PinballStateAuthoring authoring)
            {
                // Authoring を持つ Entity を取得する
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                // ピンボール状態を追加する
                AddComponent(entity, new PinballState
                {
                    // 初期値は未割り当て
                    Id = -1
                });
            }
        }
    }
}