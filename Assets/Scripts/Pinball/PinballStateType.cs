// ======================================================
// PinballStateType.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-07-02
// 更新日時 : 2026-07-03
// 概要     : ピンボール状態値の定数定義
// ======================================================

namespace PinballSystem
{
    /// <summary>
    /// ピンボールの状態を表す定数定義
    /// </summary>
    public static class PinballStateType
    {
        /// <summary>
        /// 初期化状態
        /// 生成直後で、初期化処理が完了していない状態
        /// </summary>
        public const byte Initialize = 0;

        /// <summary>
        /// プール待機状態
        /// 発射待ちとしてプールに存在している状態
        /// </summary>
        public const byte Pool = 1;

        /// <summary>
        /// アクティブ状態
        /// 発射され物理シミュレーション対象になっている状態
        /// </summary>
        public const byte Active = 2;

        /// <summary>
        /// スリープ状態
        /// ポーズ・一時停止などで更新を止める状態
        /// </summary>
        public const byte Sleeping = 3;
    }
}