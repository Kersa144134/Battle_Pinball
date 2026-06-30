// ======================================================
// PinballPoolStateAuthoring.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-27
// 更新日時 : 2026-06-27
// 概要     : ピンボールプール状態を ECS コンポーネントへ変換する Authoring
// ======================================================

using Unity.Entities;
using UnityEngine;

namespace BallSystem
{
    /// <summary>
    /// ピンボールのプール状態を付与するための Authoring クラス
    /// 生成直後の初期状態として使用する
    /// </summary>
    public sealed class PinballPoolStateAuthoring : MonoBehaviour
    {
        // ======================================================
        // Baker
        // ======================================================

        /// <summary>
        /// Authoring の情報を ECS コンポーネントへ変換する Baker
        /// </summary>
        private sealed class Baker : Baker<PinballPoolStateAuthoring>
        {
            // ======================================================
            // Baker 実装
            // ======================================================

            /// <summary>
            /// ピンボールプール状態を Entity に追加する
            /// </summary>
            /// <param name="authoring">変換元の Authoring</param>
            public override void Bake(PinballPoolStateAuthoring authoring)
            {
                // Authoring を持つ Entity を取得する
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                // ピンボールプール状態コンポーネントを追加する
                AddComponent<PinballPoolState>(entity);

                // 初期状態は非アクティブ
                SetComponentEnabled<PinballPoolState>(entity, false);
            }
        }
    }
}