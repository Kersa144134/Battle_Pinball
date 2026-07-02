// ======================================================
// LauncherSettingsAuthoring.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-30
// 更新日時 : 2026-06-30
// 概要     : ピンボール発射設定を ECS コンポーネントへ変換する Authoring
// ======================================================

using Unity.Entities;
using UnityEngine;

namespace LauncherSystem
{
    /// <summary>
    /// ピンボール発射設定をインスペクタで編集するための Authoring クラス
    /// </summary>
    public sealed class LauncherSettingsAuthoring : MonoBehaviour
    {
        // ======================================================
        // インスペクタ設定
        // ======================================================

        /// <summary>
        /// 発射間隔秒
        /// </summary>
        [SerializeField]
        private float _interval = 1.0f;

        /// <summary>
        /// 発射速度
        /// </summary>
        [SerializeField]
        private float _speed = 10f;

        // ======================================================
        // Baker
        // ======================================================

        /// <summary>
        /// Authoring 情報を ECS コンポーネントへ変換する Baker
        /// </summary>
        private sealed class Baker : Baker<LauncherSettingsAuthoring>
        {
            // ======================================================
            // Baker 実装
            // ======================================================

            /// <summary>
            /// ECS コンポーネントへ変換する
            /// </summary>
            /// <param name="authoring">変換元 Authoring</param>
            public override void Bake(LauncherSettingsAuthoring authoring)
            {
                // --------------------------------------------------
                // Entity 取得
                // --------------------------------------------------
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                // --------------------------------------------------
                // 生成設定コンポーネント作成
                // --------------------------------------------------
                LauncherSettings settings = new LauncherSettings
                {
                    LauncherEntity = entity,
                    Interval = authoring._interval,
                    Speed = authoring._speed
                };

                // --------------------------------------------------
                // コンポーネント登録
                // --------------------------------------------------
                AddComponent(entity, settings);
            }
        }
    }
}