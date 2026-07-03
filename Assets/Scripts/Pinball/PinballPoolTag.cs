// ======================================================
// PinballPoolTag.cs
// 作成者   : 高橋一翔
// 作成日時 : 2026-07-02
// 更新日時 : 2026-07-02
// 概要     : ピンボールプール管理 Entity を識別するタグコンポーネント
// ======================================================

using Unity.Entities;

namespace PinballSystem
{
    /// <summary>
    /// ピンボールプールを管理する Entity を識別するタグコンポーネント
    /// プール管理用 Entity はシーン内に 1 つだけ存在することを想定する
    /// </summary>
    public struct PinballPoolTag : IComponentData
    {
    }
}