using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 石(白、クロ)
/// </summary>
public class Stone : MonoBehaviour
{
    /// <summary>
    /// 石色変更
    /// </summary>
    bool changeColor = false;
    const float size = 1;

    /// <summary>
    /// 石のタイプ基盤
    /// </summary>
    public enum StoneType
    {
        None,//生成無し
        Black,//黒
        White//白
    }

    /// <summary>
    /// 石タイプ
    /// </summary>
    public StoneType Type { set; get; } = StoneType.None;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize(float x, float z)
    {
        gameObject.SetActive(false);
        Type = StoneType.None;
        const float y = 0.1f;
        transform.position = new Vector3(x + size / 2, y, z + size / 2);
    }

    /// <summary>
    /// 石タイプ変更
    /// </summary>
    public void SetStone(StoneType type)
    {
        Type = type;
        gameObject.SetActive(true);

        if (type == StoneType.Black)
        {
            var rot = transform.localEulerAngles;
            rot.z = 180;
            transform.localEulerAngles = rot;
        }
    }

    /// <summary>
    /// 石の反転処理
    /// </summary>
    public void TrunStone(StoneType type)
    {
        Type = type;

        var rot = transform.localEulerAngles;
        //タイプごとに反転
        rot.z = type == StoneType.Black ? 180 : 0;
        transform.localEulerAngles = rot;
    }
}
