// ======================================================
// PinballPoolEntrySystem.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-07-02
// 更新日時 : 2026-07-02
// 概要     : ピンボールプール管理 Entity を生成する ECS システム
// ======================================================

using Unity.Burst;
using Unity.Entities;

namespace PinballSystem
{
    /// <summary>
    /// ピンボールプールを管理する Entity を生成するシステム
    /// ゲーム開始時に一度だけ実行される
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct PinballPoolEntrySystem : ISystem
    {
        // ======================================================
        // ISystem 実装
        // ======================================================

        /// <summary>
        /// プール管理 Entity を生成する
        /// 初回のみ実行され、その後システムは停止する
        /// </summary>
        /// <param name="state">システムの状態</param>
        public void OnUpdate(ref SystemState state)
        {
            // プール管理 Entity 生成
            Entity poolEntity = state.EntityManager.CreateEntity();

            // プール識別タグ追加
            state.EntityManager.AddComponent<PinballPoolTag>(poolEntity);

            // プールデータバッファ追加
            state.EntityManager.AddBuffer<PinballPoolEntry>(poolEntity);

            // システム停止
            state.Enabled = false;
        }
    }
}