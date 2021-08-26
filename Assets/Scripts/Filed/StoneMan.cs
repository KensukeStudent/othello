using System.Collections.Generic;
using System;
using UnityEngine;

public enum DirType
{
    UP,
    UP_RIGHT,
    RIGHT,
    DOWN_RIGHT,
    DONW,
    DOWN_LEFT,
    LEFT,
    UP_LEFT
}

/// <summary>
/// 石の情報クラス
/// </summary>
public class StoneMan
{
    /// <summary>
    /// 石の回転する向きの変更
    /// </summary>
    Dictionary<DirType, Vector2> rotInfo = new Dictionary<DirType, Vector2>();
    /// <summary>
    /// 回転アニメーションさせる石を格納
    /// </summary>
    List<Stone> stones = new List<Stone>();

    /// <summary>
    /// コンストラクター
    /// </summary>
    public StoneMan()
    {
        Initilaize();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    void Initilaize()
    {
        //石の軸を回転
        Vector2[] dirRot =
        {
            new Vector2(0,90),
            new Vector2(0,135),
            new Vector2(0,180),
            new Vector2(0,225),
            new Vector2(0,270),
            new Vector2(0,315),
            new Vector2(0,0),
            new Vector2(0,45),
        };

        for (int i = 0; i < 8; i++)
        {
            //列挙名を取得
            DirType dirType = (DirType)Enum.ToObject(typeof(DirType), i);

            //向き名と向き方向をセット
            rotInfo.Add(dirType, dirRot[i]);
        }
    }

    /// <summary>
    /// 回転向きを取得
    /// </summary>
    public Vector2 GetDirection(int dirTypeNo)
    {
        rotInfo.TryGetValue((DirType)Enum.ToObject(typeof(DirType), dirTypeNo), out Vector2 rotDirection);
        return rotDirection;
    }

    /// <summary>
    /// 回転する石を格納
    /// </summary>
    public void SetTrunStone(Stone stone)
    {
        stones.Add(stone);
    }

    /// <summary>
    /// 石　回転更新
    /// </summary>
    public bool TrunStones()
    {
        var stoneCount = 0;

        for (int i = 0; i < stones.Count; i++)
        {
            var stone = stones[i];

            if (stone.TrunFlag)
            {
                stone.TrunAnimation();
            }
            else stoneCount++;
        }

        //全ての回転アニメーションが終わったらリストをクリア
        if(stoneCount == stones.Count)
        {
            stones.Clear();
            return true;
        }

        return false;
    }
}
