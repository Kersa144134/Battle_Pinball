// ======================================================
// BumperCollisionSettings.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-06-30
// 更新日時 : 2026-06-30
// 概要     : バンパーの反発設定を保持する ECS コンポーネント
// ======================================================

using Unity.Entities;

namespace BumperSystem
{
    /// <summary>
    /// バンパーの反発設定を保持するコンポーネント
    /// </summary>
    public struct BumperCollisionSettings : IComponentData
    {
        /// <summary>
        /// 反射時に加える基本反射力
        /// </summary>
        public float Power;
    }
}