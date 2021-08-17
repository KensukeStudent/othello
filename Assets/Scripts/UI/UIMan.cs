using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// UI管理
/// </summary>
public class UIMan : MonoBehaviour
{
    /// <summary>
    /// 黒の数
    /// </summary>
    [SerializeField] Text blackCount;
    /// <summary>
    /// 白の数
    /// </summary>
    [SerializeField] Text whiteCount;

    /// <summary>
    /// ゲームオーバーテキスト
    /// </summary>
    [SerializeField] GameObject gameOverObj;
    /// <summary>
    /// 勝敗テキスト
    /// </summary>
    [SerializeField] Text winlose;

    /// <summary>
    /// 各カウントをセット
    /// </summary>
    public void SetCount(int black,int white)
    {
        blackCount.text = black.ToString();
        whiteCount.text = white.ToString();
    }

    /// <summary>
    /// ゲームオーバーUIを表示
    /// </summary>
    public void SetGameOver()
    {
        gameOverObj.SetActive(true);

        if(int.Parse(blackCount.text) == int.Parse(whiteCount.text))
        {
            winlose.text = "引き分けです！";
            return;
        }

        winlose.text = int.Parse(blackCount.text) > int.Parse(whiteCount.text) ?
            "あなたの勝ちです！"
            :
            "あなたの負けです！";
    }

    /// <summary>
    /// 続けるボタン
    /// </summary>
    public void ContinueButton()
    {
        SceneManager.LoadScene("Main");
    }

    /// <summary>
    /// 終わりボタン
    /// </summary>
    public void EndButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}