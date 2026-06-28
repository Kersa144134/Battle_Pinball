// ======================================================
// PinballSpawnSettingsAuthoring.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-27
// 更新日時 : 2026-06-27
// 概要     : ピンボール生成設定を ECS コンポーネントへ変換する Authoring
// ======================================================

using Unity.Entities;
using UnityEngine;

namespace BallSystem
{
    /// <summary>
    /// ピンボール生成設定を Inspector 上で編集するための Authoring クラス
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
        /// 生成するピンボール数
        /// </summary>
        [SerializeField]
        private int _spawnCount = 10;

        /// <summary>
        /// ピンボールを生成する半径
        /// </summary>
        [SerializeField]
        private float _spawnRadius = 1.0f;

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
            // ======================================================
            // Baker 実装
            // ======================================================

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
                
                // 生成設定を格納するコンポーネントを作成する
                PinballSpawnSettings pinballSpawnSettings = new PinballSpawnSettings()
                {
                    // プレハブを Entity として取得する
                    Prefab = GetEntity(authoring._prefab, TransformUsageFlags.Dynamic),

                    // 生成数を設定する
                    SpawnCount = authoring._spawnCount,

                    // スポーン半径を設定する
                    SpawnRadius = authoring._spawnRadius,

                    // ランダムシードを設定する
                    RandomSeed = authoring._randomSeed
                };

                // Authoring を持つ Entity を取得する
                Entity entity = GetEntity(TransformUsageFlags.None);

                // Entity に生成設定コンポーネントを追加する
                AddComponent(entity, pinballSpawnSettings);
            }
        }
    }
}