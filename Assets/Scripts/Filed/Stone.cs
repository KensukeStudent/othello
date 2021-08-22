using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 石(白、クロ)
/// </summary>
public class Stone : MonoBehaviour
{
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

    DirType aa = DirType.DONW;

    /// <summary>
    /// 石のサイズ
    /// </summary>
    const float size = 1;

    /// <summary>
    /// ひっくり返しフラグ
    /// </summary>
    bool trunFlag = false;
    /// <summary>
    /// 0.5秒かけて回転する
    /// </summary>
    const float rotSecond = 180.0f / 0.5f; 
    /// <summary>
    /// 回転が始まるラグ
    /// </summary>
    float rotTimeLag = 0.0f;

    /// <summary>
    /// 回転方向
    /// </summary>
    Vector2 rotDirection;

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

    /// <summary>
    /// 反転フラグセット
    /// </summary>
    public void TrunSet(float rotTimeLag , DirType dirType)
    {
        trunFlag = true;
        this.rotTimeLag = rotTimeLag;

        int x = 0;

        //回転方向を指定
        switch (dirType)
        {
            //(  0 , 1),
            case DirType.UP:
                
                break;

            //(  1 , 1)
            case DirType.UP_RIGHT:
                
                break;

            //(  1 , 0)
            case DirType.RIGHT:
                
                break;

            //(  1 ,-1)
            case DirType.DOWN_RIGHT:
                
                break;

            //(  0 ,-1)
            case DirType.DONW:
                
                break;

            //( -1 ,-1)
            case DirType.DOWN_LEFT:
                
                break;

            //( -1 , 0)
            case DirType.LEFT:
                
                break;

            //(-1, 1)
            case DirType.UP_LEFT:

                break;
        }
    }

    /// <summary>
    /// 回転アニメーション再生
    /// </summary>
    public void TrunAnimation()
    {
        //どの方向に回転するか

    }
}
