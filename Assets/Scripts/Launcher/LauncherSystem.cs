// ======================================================
// LauncherSystem.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-30
// 更新日時 : 2026-07-02
// 概要     : ピンボールをプール状態から発射する ECS システム
// ======================================================

using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using PinballSystem;

namespace LauncherSystem
{
    /// <summary>
    /// プール状態のピンボールを発射するシステム
    /// 各ランチャーの発射条件に応じてプールからボールを消費する
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct LauncherSystem : ISystem
    {
        // ======================================================
        // ISystem 実装
        // ======================================================

        public void OnUpdate(ref SystemState state)
        {
            // --------------------------------------------------
            // プール取得
            // --------------------------------------------------
            DynamicBuffer<PinballPoolEntry> pool =
                SystemAPI.GetSingletonBuffer<PinballPoolEntry>();

            // プールが空なら終了
            if (pool.Length == 0)
            {
                return;
            }

            // --------------------------------------------------
            // ランチャー更新
            // --------------------------------------------------
            foreach (var (launcherSettings, launcherState)
                in SystemAPI.Query<
                    RefRO<LauncherSettings>,
                    RefRW<LauncherState>>())
            {
                // 経過時間更新
                launcherState.ValueRW.ElapsedTime += SystemAPI.Time.DeltaTime;

                // 発射条件未達
                if (launcherState.ValueRO.ElapsedTime <
                    launcherSettings.ValueRO.Interval)
                {
                    continue;
                }

                // タイマーリセット
                launcherState.ValueRW.ElapsedTime = 0f;

                // --------------------------------------------------
                // プールから発射対象を取得
                // --------------------------------------------------
                Entity entity =
                    pool[pool.Length - 1].Entity;

                // 取得した要素をプールから削除
                pool.RemoveAt(pool.Length - 1);

                // --------------------------------------------------
                // 発射元ランチャーのTransform取得
                // --------------------------------------------------
                // ランチャー位置と回転を発射基準として使用
                LocalTransform launcherTransform =
                    SystemAPI.GetComponent<LocalTransform>(
                        launcherSettings.ValueRO.LauncherEntity);

                // --------------------------------------------------
                // ピンボール物理キャッシュ取得
                // --------------------------------------------------
                PinballPhysicsCache cache =
                    SystemAPI.GetComponent<PinballPhysicsCache>(entity);

                // --------------------------------------------------
                // ピンボールTransformを発射位置へ更新
                // --------------------------------------------------
                // 発射時にランチャー位置へ同期させる
                SystemAPI.SetComponent(entity, new LocalTransform
                {
                    Position = launcherTransform.Position,
                    Rotation = launcherTransform.Rotation,
                    Scale = cache.CachedScale
                });

                // --------------------------------------------------
                // 発射方向を計算
                // --------------------------------------------------
                // ランチャー前方ベクトルをワールド空間へ変換
                float3 direction =
                    math.mul(
                        launcherTransform.Rotation,
                        new float3(0f, 0f, 1f));

                // --------------------------------------------------
                // 物理状態の復元
                // --------------------------------------------------
                // プール中に無効化していた物理パラメータを復元
                SystemAPI.SetComponent(entity, cache.CachedMass);
                SystemAPI.SetComponent(entity, cache.CachedDamping);

                // --------------------------------------------------
                // 初速度を付与
                // --------------------------------------------------
                // 発射方向へ初速度を与えて飛翔挙動を生成
                SystemAPI.SetComponent(entity, new PhysicsVelocity
                {
                    Linear = direction * launcherSettings.ValueRO.Speed,
                    Angular = float3.zero
                });

                // --------------------------------------------------
                // 状態設定
                // --------------------------------------------------
                // ピンボール状態を取得する
                PinballState pinballState =
                    SystemAPI.GetComponent<PinballState>(entity);

                // 状態をアクティブ状態に設定する
                pinballState.State = PinballStateType.Active;

                // 更新した状態を書き戻す
                SystemAPI.SetComponent(entity, pinballState);
            }
        }
    }
}