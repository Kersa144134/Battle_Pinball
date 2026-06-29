// ======================================================
// BlowerForceSettingsAuthoring.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-27
// 更新日時 : 2026-06-27
// 概要     : ブロワー設定を ECS コンポーネントへ変換する Authoring
// ======================================================

using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace BlowerSystem
{
    /// <summary>
    /// ブロワー設定をインスペクタ上で編集するための Authoring クラス
    /// </summary>
    public sealed class BlowerForceSettingsAuthoring : MonoBehaviour
    {
        // ======================================================
        // インスペクタ設定
        // ======================================================

        /// <summary>
        /// ブロワーがオブジェクトへ加える力
        /// </summary>
        [SerializeField]
        private float3 _force = math.float3(0.0f, 100.0f, 0.0f);

        // ======================================================
        // Baker
        // ======================================================

        /// <summary>
        /// Authoring の情報を ECS コンポーネントへ変換する Baker
        /// </summary>
        private sealed class Baker : Baker<BlowerForceSettingsAuthoring>
        {
            /// <summary>
            /// Authoring の設定を ECS コンポーネントとして登録する
            /// </summary>
            /// <param name="authoring">変換元の Authoring</param>
            public override void Bake(BlowerForceSettingsAuthoring authoring)
            {
                // ブロワー設定を格納するコンポーネントを作成する
                BlowerForceSettings blowerSettings = new BlowerForceSettings()
                {
                    // オブジェクトへ加える力を設定する
                    Force = authoring._force
                };

                // Authoring を持つ Entity を取得する
                Entity entity = GetEntity(TransformUsageFlags.None);

                // Entity にブロワー設定コンポーネントを追加する
                AddComponent(entity, blowerSettings);
            }
        }
    }
}