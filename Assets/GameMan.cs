using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 現在のターン
/// </summary>
enum GoTrun
{
    BlackTrun,//黒ターン
    WhiteTrun//白ターン
}

/// <summary>
/// ゲーム管理
/// </summary>
public class GameMan : MonoBehaviour
{
    /// <summary>
    /// 基本黒ターン
    /// </summary>
    GoTrun trun = GoTrun.BlackTrun;
}
