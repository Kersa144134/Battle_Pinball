// ======================================================
// PinballSpawnSystem.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-27
// 更新日時 : 2026-06-27
// 概要     : ピンボールを生成する ECS システム
// ======================================================

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PinballSystem
{
    /// <summary>
    /// ピンボールの生成処理を行うシステム
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(PinballPoolEntrySystem))]
    public partial struct PinballSpawnSystem : ISystem
    {
        // ======================================================
        // ISystem 実装
        // ======================================================

        /// <summary>
        /// システム生成時の初期化を行う
        /// </summary>
        /// <param name="state">システムの状態</param>
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PinballSpawnSettings>();
        }

        /// <summary>
        /// ピンボールを生成する
        /// </summary>
        /// <param name="state">システムの状態</param>
        public void OnUpdate(ref SystemState state)
        {
            // 生成設定を取得する
            PinballSpawnSettings settings =
                SystemAPI.GetSingleton<PinballSpawnSettings>();

            // プレハブが存在しない場合は終了する
            if (!state.EntityManager.Exists(settings.Prefab))
            {
                // システムを停止する
                state.Enabled = false;
                return;
            }

            // 指定数のピンボールを一括生成する
            NativeArray<Entity> entities =
                state.EntityManager.Instantiate(
                    settings.Prefab,
                    settings.SpawnCount,
                    Allocator.Temp);

            // 現在のプール配置番号を取得する
            int nextPoolIndex =
                SystemAPI.GetSingleton<PinballPoolInfo>().NextPoolIndex;

            // --------------------------------------------------
            // プール要求付与
            // --------------------------------------------------
            for (int i = 0; i < entities.Length; i++)
            {
                // 対象 Entity を取得する
                Entity entity =
                    entities[i];

                // プール要求コンポーネントを追加する
                state.EntityManager.AddComponent<PinballRequestPool>(entity);
            }

            // 構造変更後にプール管理情報を再取得する
            RefRW<PinballPoolInfo> poolInfo =
                SystemAPI.GetSingletonRW<PinballPoolInfo>();

            // 次回使用する配置番号を保存する
            poolInfo.ValueRW.NextPoolIndex =
                nextPoolIndex;

            // 一時生成した配列を解放する
            entities.Dispose();

            // 初回生成のみ実行するためシステムを停止する
            state.Enabled = false;
        }
    }
}