using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanLocation
{
    /// <summary>
    /// 配置できる
    /// </summary>
    public bool Put { private set; get; } = false;
    /// <summary>
    /// 相手の石をひっくり返す座標方向
    /// </summary>
    public Vector2[] TrunDirection { private set; get; } = new Vector2[8];

    /// <summary>
    /// 置けるかの判定をセット
    /// </summary>
    public void SetPut(bool condition)
    {
        Put = condition;
    }

    /// <summary>
    /// 石のひっくり返す方向をセット
    /// </summary>
    public void SetTrunDirection(int directionNo,Vector2 toDirection)
    {
        TrunDirection[directionNo] = toDirection;
    }

    public void Reset()
    {
        for (int i = 0; i < TrunDirection.Length; i++)
        {
            TrunDirection[i] = Vector2.zero;
        }
    }
}