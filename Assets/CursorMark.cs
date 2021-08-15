using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カーソル点滅処理
/// </summary>
public class CursorMark : MonoBehaviour
{
    [SerializeField] SpriteRenderer cursorMark;

    /// <summary>
    /// 点滅モード
    /// </summary>
    public enum FadeSequenceType
    {
        In,//明るくなる
        Out//暗くなる
    }

    FadeSequenceType fadeSequenceType = FadeSequenceType.In;

    /// <summary>
    /// 速度
    /// </summary>
    float fadeSpeed = 1;

    /// <summary>
    /// アルファ値最大
    /// </summary>
    float maxAlpha = 1;
    /// <summary>
    /// アルファ値最小
    /// </summary>
    float minAlpha = 0.2f;

    /// <summary>
    /// 変更するアルファ値
    /// </summary>
    float alpha = 1.0f;

    /// <summary>
    /// 座標セット
    /// </summary>
    public void Initialize(float x, float z)
    {
        const float size = 1;
        const float offsetY = 0.05f;
        var pos = Vector3.zero;

        pos.x = x + size / 2;
        pos.y = offsetY;
        pos.z = z + size / 2;

        transform.position = pos;
        
        //非表示
        Hide();
    }

    /// <summary>
    /// 非表示
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);

        //アルファを初期化
        alpha = 1;
        SetAlpha();
    }

    private void Update()
    {
        FadeUpdate();
    }

    /// <summary>
    /// フェイド処理
    /// </summary>
    void FadeUpdate()
    {
        switch (fadeSequenceType)
        {
            //フェイドイン処理
            case FadeSequenceType.In:

                UpdateIn();

                if (EndIn())
                {
                    fadeSequenceType = FadeSequenceType.Out;
                }

                break;

            //フェイドアウト処理
            case FadeSequenceType.Out:

                UpdateOut();

                if (EndOut())
                {
                    fadeSequenceType = FadeSequenceType.In;
                }

                break;
        }

        //透過度をセット
        SetAlpha();
    }

    /// <summary>
    /// フェイドイン処理(透明化)
    /// </summary>
    void UpdateIn()
    {
        alpha -= Time.deltaTime * fadeSpeed;
        alpha = Mathf.Clamp(alpha, minAlpha, maxAlpha);
    }

    /// <summary>
    /// フェイドアウト処理(暗化)
    /// </summary>
    void UpdateOut()
    {
        alpha += Time.deltaTime * fadeSpeed;
        alpha = Mathf.Clamp(alpha, minAlpha, maxAlpha);
    }

    /// <summary>
    /// フェイドイン終了
    /// </summary>
    bool EndIn()
    {
        return alpha == minAlpha;
    }

    /// <summary>
    /// フェイドアウト終了
    /// </summary>
    bool EndOut()
    {
        return alpha == maxAlpha;
    }

    /// <summary>
    /// α値を代入
    /// </summary>
    void SetAlpha()
    {
        var color = cursorMark.color;

        color.a = alpha;
        cursorMark.color = color;
    }
}
