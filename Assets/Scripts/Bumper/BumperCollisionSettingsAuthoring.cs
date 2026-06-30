// ======================================================
// BumperCollisionSettingsAuthoring.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-30
// 更新日時 : 2026-06-30
// 概要     : バンパー設定を ECS コンポーネントへ変換する Authoring
// ======================================================

using Unity.Entities;
using UnityEngine;

namespace BumperSystem
{
    /// <summary>
    /// バンパー設定をインスペクタ上で編集するための Authoring クラス
    /// </summary>
    public sealed class BumperCollisionSettingsAuthoring : MonoBehaviour
    {
        // ======================================================
        // インスペクタ設定
        // ======================================================

        /// <summary>
        /// 反射時に加える力
        /// </summary>
        [SerializeField]
        private float _power = 30.0f;

        // ======================================================
        // Baker
        // ======================================================

        /// <summary>
        /// Authoring の情報を ECS コンポーネントへ変換する Baker
        /// </summary>
        private sealed class Baker : Baker<BumperCollisionSettingsAuthoring>
        {
            /// <summary>
            /// Authoring の設定を ECS コンポーネントとして登録する
            /// </summary>
            /// <param name="authoring">変換元の Authoring</param>
            public override void Bake(BumperCollisionSettingsAuthoring authoring)
            {
                // バンパー設定を格納するコンポーネントを作成する
                BumperCollisionSettings bumperSettings = new BumperCollisionSettings()
                {
                    Power = authoring._power,
                };

                // Authoring を持つ Entity を取得する
                Entity entity = GetEntity(TransformUsageFlags.None);

                // Entity にバンパー設定を追加する
                AddComponent(entity, bumperSettings);
            }
        }
    }
}