// ======================================================
// PinballRequestPool.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-07-06
// 更新日時 : 2026-07-06
// 概要     : ピンボールをプール状態へ遷移させる要求を表すコンポーネント
// ======================================================

using Unity.Entities;

namespace PinballSystem
{
    /// <summary>
    /// ピンボールをプール状態へ遷移させる要求を表すコンポーネント
    /// PinballPoolSystem が検出し、プール処理完了後に削除する
    /// </summary>
    public struct PinballRequestPool : IComponentData
    {
    }
}