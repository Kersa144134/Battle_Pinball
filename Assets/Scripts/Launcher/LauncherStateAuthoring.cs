// ======================================================
// LauncherStateAuthoring.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-07-02
// 更新日時 : 2026-07-02
// 概要     : ピンボール発射状態を ECS コンポーネントへ変換する Authoring
// ======================================================

using Unity.Entities;
using UnityEngine;

namespace LauncherSystem
{
    /// <summary>
    /// ピンボール発射状態をインスペクタで保持するための Authoring クラス
    /// </summary>
    public sealed class LauncherStateAuthoring : MonoBehaviour
    {
        // ======================================================
        // インスペクタ設定
        // ======================================================

        /// <summary>
        /// 経過時間
        /// </summary>
        [SerializeField]
        private float _elapsedTime = 0f;

        // ======================================================
        // Baker
        // ======================================================

        /// <summary>
        /// ECS コンポーネントへ変換する Baker
        /// </summary>
        private sealed class Baker : Baker<LauncherStateAuthoring>
        {
            // ======================================================
            // Baker 実装
            // ======================================================

            /// <summary>
            /// Authoring を ECS コンポーネントへ変換する
            /// </summary>
            /// <param name="authoring">変換元 Authoring</param>
            public override void Bake(LauncherStateAuthoring authoring)
            {
                // --------------------------------------------------
                // Entity 取得
                // --------------------------------------------------
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                // --------------------------------------------------
                // 状態コンポーネント作成
                // --------------------------------------------------
                LauncherState state = new LauncherState
                {
                    ElapsedTime = authoring._elapsedTime
                };

                // --------------------------------------------------
                // コンポーネント登録
                // --------------------------------------------------
                AddComponent(entity, state);
            }
        }
    }
}