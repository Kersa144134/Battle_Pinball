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
using Unity.Mathematics;
using Unity.Transforms;
using Klak.Math;

namespace BallSystem
{
    /// <summary>
    /// ピンボールの生成処理を行うシステム
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
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
            // PinballSpawnSettings が存在する場合のみシステムを実行する
            state.RequireForUpdate<PinballSpawnSettings>();
        }

        /// <summary>
        /// ピンボールを生成する
        /// </summary>
        /// <param name="state">システムの状態</param>
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // ピンボール生成設定を取得する
            var pinballSpawnSettings = SystemAPI.GetSingleton<PinballSpawnSettings>();

            // プレハブ Entity が存在しない場合は生成しない
            if (!state.EntityManager.Exists(pinballSpawnSettings.Prefab))
            {
                state.Enabled = false;
                return;
            }

            // 指定数のピンボールを生成する
            NativeArray<Entity> spawnedEntities = state.EntityManager.Instantiate(
                pinballSpawnSettings.Prefab,
                pinballSpawnSettings.SpawnCount,
                Allocator.Temp);

            // ランダム生成用の乱数を初期化する
            Random random = new Random(pinballSpawnSettings.RandomSeed);

            // 生成したすべてのピンボールに対して初期位置を設定する
            foreach (Entity entity in spawnedEntities)
            {
                // スポーン半径内のランダムな位置を取得する
                float3 position = random.NextFloat3InSphere() * pinballSpawnSettings.SpawnRadius;

                // LocalTransform コンポーネントを取得する
                var localTransform = SystemAPI.GetComponentRW<LocalTransform>(entity);

                // ピンボールの初期位置を設定する
                localTransform.ValueRW.Position = position;
            }

            // NativeArray を破棄する
            spawnedEntities.Dispose();

            // 初回生成のみ実行するためシステムを停止する
            state.Enabled = false;
        }
    }
}