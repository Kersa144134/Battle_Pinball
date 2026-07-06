// ======================================================
// PinballPoolZoneAuthoring.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-07-06
// 更新日時 : 2026-07-06
// 概要     : ピンボールプールゾーンを ECS タグへ変換する Authoring
// ======================================================

using Unity.Entities;
using UnityEngine;

namespace PinballSystem
{
    /// <summary>
    /// ピンボールのプールゾーンを識別するための Authoring クラス
    /// シーン上の配置オブジェクトに付与し、ECS 側でゾーン判定に使用する
    /// </summary>
    public sealed class PinballPoolZoneAuthoring : MonoBehaviour
    {
        // ======================================================
        // Baker
        // ======================================================

        /// <summary>
        /// Authoring を ECS コンポーネントへ変換する Baker
        /// </summary>
        private sealed class Baker : Baker<PinballPoolZoneAuthoring>
        {
            // ======================================================
            // Baker 実装
            // ======================================================

            /// <summary>
            /// プールゾーンタグを Entity に付与する
            /// </summary>
            /// <param name="authoring">変換元 Authoring</param>
            public override void Bake(PinballPoolZoneAuthoring authoring)
            {
                // Authoring がアタッチされている Entity を取得する
                Entity entity =
                    GetEntity(TransformUsageFlags.Dynamic);

                // プールゾーン識別タグを付与する
                AddComponent<PinballPoolZoneTag>(entity);
            }
        }
    }
}