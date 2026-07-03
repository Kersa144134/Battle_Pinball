// ======================================================
// PinballSpawnSettingsAuthoring.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-27
// 更新日時 : 2026-07-03
// 概要     : ピンボール生成設定を ECS コンポーネントへ変換する Authoring
// ======================================================

using Unity.Entities;
using UnityEngine;

namespace PinballSystem
{
    /// <summary>
    /// ピンボール生成設定をインスペクタ上で編集するための Authoring クラス
    /// </summary>
    public sealed class PinballSpawnSettingsAuthoring : MonoBehaviour
    {
        // ======================================================
        // インスペクタ設定
        // ======================================================

        /// <summary>
        /// 生成するピンボールのプレハブ
        /// </summary>
        [SerializeField]
        private GameObject _prefab = null;

        /// <summary>
        /// プールに生成するピンボールの総数
        /// </summary>
        [SerializeField]
        [Min(1)]
        private int _spawnCount = 10;

        /// <summary>
        /// ランダム生成に使用するシード値
        /// </summary>
        [SerializeField]
        private uint _randomSeed = 12345;

        // ======================================================
        // Baker
        // ======================================================

        /// <summary>
        /// Authoring の情報を ECS コンポーネントへ変換する Baker
        /// </summary>
        private sealed class Baker : Baker<PinballSpawnSettingsAuthoring>
        {
            /// <summary>
            /// Authoring の設定を ECS コンポーネントとして登録する
            /// </summary>
            /// <param name="authoring">変換元の Authoring</param>
            public override void Bake(PinballSpawnSettingsAuthoring authoring)
            {
                // プレハブが設定されていない場合は変換しない
                if (authoring._prefab == null)
                {
                    Debug.LogError(
                        $"{nameof(PinballSpawnSettingsAuthoring)} の Prefab が設定されていません。",
                        authoring);

                    return;
                }

                // --------------------------------------------------
                // 生成設定コンポーネント作成
                // --------------------------------------------------
                PinballSpawnSettings settings = new PinballSpawnSettings
                {
                    Prefab = GetEntity(authoring._prefab, TransformUsageFlags.Dynamic),
                    SpawnCount = authoring._spawnCount,
                    RandomSeed = authoring._randomSeed
                };

                // --------------------------------------------------
                // Entity取得
                // --------------------------------------------------
                Entity entity =
                    GetEntity(TransformUsageFlags.None);

                AddComponent(entity, settings);
            }
        }
    }
}