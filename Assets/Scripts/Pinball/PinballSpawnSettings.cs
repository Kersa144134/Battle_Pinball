// ======================================================
// PinballSpawnSettings.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-27
// 更新日時 : 2026-07-03
// 概要     : ピンボール生成設定コンポーネント
// ======================================================

using Unity.Entities;

namespace PinballSystem
{
    /// <summary>
    /// ピンボール生成時に使用する設定を保持するコンポーネント
    /// </summary>
    public struct PinballSpawnSettings : IComponentData
    {
        /// <summary>
        /// 生成するピンボールのPrefabエンティティ
        /// </summary>
        public Entity Prefab;

        /// <summary>
        /// プールに生成するピンボールの総数
        /// </summary>
        public int SpawnCount;

        /// <summary>
        /// ランダム生成に使用するシード値
        /// </summary>
        public uint RandomSeed;
    }
}