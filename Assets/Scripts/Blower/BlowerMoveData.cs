// ======================================================
// BlowerMoveData.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-27
// 更新日時 : 2026-06-27
// 概要     : ブロワーの移動入力を保持する ECS コンポーネント
// ======================================================

using Unity.Entities;

namespace BlowerSystem
{
    /// <summary>
    /// 外部入力から渡される Z 軸方向の移動データ
    /// </summary>
    public struct BlowerMoveData : IComponentData
    {
        /// <summary>
        /// Z 軸方向の移動入力
        /// 正方向と負方向で移動を制御する
        /// </summary>
        public float Direction;
    }
}