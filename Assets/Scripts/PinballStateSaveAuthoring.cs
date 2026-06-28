// ======================================================
// PinballStateSaveAuthoring.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-27
// 更新日時 : 2026-06-27
// 概要     : ピンボール状態保存設定を ECS コンポーネントへ変換する Authoring
// ======================================================

using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BallSystem
{
    /// <summary>
    /// ピンボールの状態保存・復元に使用する入力設定を行う Authoring クラス
    /// </summary>
    public sealed class PinballStateSaveAuthoring : MonoBehaviour
    {
        // ======================================================
        // インスペクタ設定
        // ======================================================

        /// <summary>
        /// 状態を保存する入力アクション
        /// </summary>
        [SerializeField]
        private InputAction _saveAction = null;

        /// <summary>
        /// 保存した状態を復元する入力アクション
        /// </summary>
        [SerializeField]
        private InputAction _loadAction = null;

        // ======================================================
        // Baker
        // ======================================================

        /// <summary>
        /// Authoring の設定を ECS コンポーネントへ変換する Baker
        /// </summary>
        private sealed class Baker : Baker<PinballStateSaveAuthoring>
        {
            // ======================================================
            // ECS コンポーネント変換
            // ======================================================

            /// <summary>
            /// Authoring の設定を ECS コンポーネントとして登録する
            /// </summary>
            /// <param name="authoring">変換元の Authoring</param>
            public override void Bake(PinballStateSaveAuthoring authoring)
            {
                // 状態保存設定コンポーネントを生成する
                PinballStateSaveData pinballStateSaveData = new PinballStateSaveData()
                {
                    // 状態保存入力を設定する
                    SaveAction = authoring._saveAction,

                    // 状態復元入力を設定する
                    LoadAction = authoring._loadAction
                };

                // Authoring に対応する Entity を取得する
                Entity entity = GetEntity(TransformUsageFlags.None);

                // マネージドコンポーネントとして Entity に追加する
                AddComponentObject(entity, pinballStateSaveData);
            }
        }
    }
}