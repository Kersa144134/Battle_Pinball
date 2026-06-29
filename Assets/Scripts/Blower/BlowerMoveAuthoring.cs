// ======================================================
// BlowerMoveAuthoring.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-27
// 更新日時 : 2026-06-27
// 概要     : PlayerIdComponent・MoveInputData・MoveSpeedComponentをEntity生成時に付与するAuthoring
// ======================================================

using Unity.Entities;
using UnityEngine;

namespace BlowerSystem
{
    /// <summary>
    /// ブロワー Entity に識別 ID・入力・速度を付与する Authoring
    /// </summary>
    public sealed class BlowerMoveAuthoring : MonoBehaviour
    {
        // ======================================================
        // インスペクタ設定
        // ======================================================

        /// <summary>
        /// プレイヤー ID
        /// </summary>
        [SerializeField]
        private int _playerId = 0;

        /// <summary>
        /// 移動速度
        /// </summary>
        [SerializeField]
        private float _speed = 5f;

        // ======================================================
        // Baker
        // ======================================================

        /// <summary>
        /// MonoBehaviour → ECS 変換処理
        /// </summary>
        private sealed class Baker : Baker<BlowerMoveAuthoring>
        {
            /// <summary>
            /// Entityへ必要なコンポーネントを付与する
            /// </summary>
            /// <param name="authoring">Authoring参照</param>
            public override void Bake(BlowerMoveAuthoring authoring)
            {
                // Entity取得
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                // 初期方向生成
                float initialDirection = 0f;

                // PlayerIdComponent付与
                AddComponent(entity, new PlayerIdComponent
                {
                    Value = authoring._playerId
                });

                // MoveInputData付与
                AddComponent(entity, new BlowerMoveData
                {
                    Direction = initialDirection
                });

                // MoveSpeedComponent付与
                AddComponent(entity, new MoveSpeedComponent
                {
                    Value = authoring._speed
                });
            }
        }
    }
}