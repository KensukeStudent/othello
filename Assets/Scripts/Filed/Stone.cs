using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 石(白、クロ)
/// </summary>
public class Stone : MonoBehaviour
{
    /// <summary>
    /// 石のサイズ
    /// </summary>
    const float size = 1;

    /// <summary>
    /// ひっくり返しフラグ
    /// </summary>
    public bool TrunFlag { private set; get; } = false;
    /// <summary>
    /// 0.5秒かけて回転する
    /// </summary>
    readonly float rotSecond = 180.0f / 0.2f; 
    /// <summary>
    /// 回転が始まるラグ
    /// </summary>
    float rotTimeLag = 0.0f;
    /// <summary>
    /// 計測
    /// </summary>
    float timer = 0.0f;
    /// <summary>
    /// 回転角度制限
    /// </summary>
    float limitRot = 0.0f;

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
        const float y = 0.5f;
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
    /// 反転情報セット
    /// </summary>
    public void SetTrun(float rotTimeLag , Vector2 rotDirection)
    {
        //回転開始フラグ
        TrunFlag = true;
        //回転開始タイムラグ
        this.rotTimeLag = rotTimeLag;
        //回転前の向き方向
        transform.localEulerAngles = new Vector3(rotDirection.x, rotDirection.y, transform.localEulerAngles.z);
        //回転角度制限 ---> 猶予値として５設ける
        limitRot = Type == StoneType.Black ? 360.0f + rotSecond : 180.0f + rotSecond;
    }

    /// <summary>
    /// 回転アニメーション再生
    /// </summary>
    public void TrunAnimation()
    {
        //どの方向に回転するか
        if (!TrunFlag) return;

        timer += Time.deltaTime;

        if(timer >= rotTimeLag)
        {
            var rot = transform.localEulerAngles;
            rot.z += Time.deltaTime * rotSecond;

            //余分な５の値を引く
            if(rot.z % limitRot > limitRot - rotSecond)
            {
                TrunFlag = false;
                timer = 0.0f;
                rot.z = Type == StoneType.Black ? 0.0f : 180.0f;
                Type = Type == StoneType.Black ? StoneType.White : StoneType.Black;
            }

            transform.localEulerAngles = rot;
        }
    }
}