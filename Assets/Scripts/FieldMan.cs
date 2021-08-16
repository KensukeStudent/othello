using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// フィールド管理
/// </summary>
public class FieldMan : MonoBehaviour
{
    enum GameMode
    {
        Initialize,//初期化
        SetStoneCheak,//置ける石をチェック
        PutStone,//プレイヤーが石を置く
        GameOver,//ゲーム終了
    }

    /// <summary>
    /// ゲームモード
    /// </summary>
    GameMode gameMode = GameMode.Initialize;

    /// <summary>
    /// 現在のターン
    /// </summary>
    enum GoTrun
    {
        BlackTrun,//黒ターン
        WhiteTrun//白ターン
    }

    /// <summary>
    /// 基本黒ターン
    /// </summary>
    GoTrun trun = GoTrun.BlackTrun;

    /// <summary>
    /// 石
    /// </summary>
    [SerializeField] GameObject stoneObj;
    [SerializeField] GameObject cursorObj;

    /// <summary>
    /// フィールドの石状態
    /// </summary>
    Stone[,] fieldStone = new Stone[fieldW, fieldW];
    /// <summary>
    /// フィールドのカーソル状態
    /// </summary>
    CursorMark[,] fieldCursor = new CursorMark[fieldW, fieldW];
    /// <summary>
    /// 石配置可能位置
    /// </summary>
    int[,] canLocation = new int[fieldW, fieldW];

    /// <summary>
    /// 探索する座標方向
    /// </summary>
    Vector3[] dir =
    {
          new Vector3(  0 ,0, 1),  // up
          new Vector3(  1 ,0, 1),  // up-right
          new Vector3(  1 ,0, 0),  // right
          new Vector3(  1 ,0,-1),  // down-right
          new Vector3(  0 ,0,-1),  // down
          new Vector3( -1 ,0,-1),  // down-left
          new Vector3( -1 ,0, 0),  // left
          new Vector3( -1 ,0, 1),  // up-left
    };

    const int fieldW = 8;

    private void Start()
    {
        Initialize();
    }

    /// <summary>
    /// 石初期配置
    /// </summary>
    void Initialize()
    {
        for (int y = 0; y < fieldW; y++)
        {
            for (int x = 0; x < fieldW; x++)
            {
                var stone = Instantiate(stoneObj).GetComponent<Stone>();
                //石初期化
                stone.Initialize(x, y);
                //石をセット
                fieldStone[x, y] = stone;

                var cursor = Instantiate(cursorObj).GetComponent<CursorMark>();
                //カーソル初期化
                cursor.Initialize(x, y);
                //カーソルをセット
                fieldCursor[x, y] = cursor;
            }
        }

        //初めの４つの石
        fieldStone[3, 3].SetStone(Stone.StoneType.White);
        fieldStone[3, 4].SetStone(Stone.StoneType.Black);
        fieldStone[4, 3].SetStone(Stone.StoneType.Black);
        fieldStone[4, 4].SetStone(Stone.StoneType.White);
    }

    private void Update()
    {
        switch (gameMode)
        {
            //初期化
            case GameMode.Initialize:

                gameMode = GameMode.SetStoneCheak;
                break;
            
            //置ける石を判定
            case GameMode.SetStoneCheak:

                PutStoneCheak();
                break;
            
            //プレイヤーが石を置く
            case GameMode.PutStone:
                
                SetStone();
                break;
            
            //勝敗判定
            case GameMode.GameOver:
            
                break;
        }
    }

    /// <summary>
    /// 石を置く
    /// </summary>
    void SetStone()
    {
        var cursor = CursorMan.CursorPos;
        var x = (int)cursor.x;
        var y = (int)cursor.z;

        //マウスカーソルがフィールド内であれば処理
        if (OnArea(x, y) && Input.GetMouseButtonDown(0) && canLocation[x, y] == 1)
        {
            //石を置く
            fieldStone[x, y].SetStone(Stone.StoneType.Black);

            //敵AI起動

            gameMode = GameMode.SetStoneCheak;
            //カーソルマークを非表示
            CursorMarkHide();
        }
    }

    /// <summary>
    /// 石を置けるコマにカーソルを光らせる
    /// </summary>
    void PutStoneCheak()
    {
        //0,0～7,7まで探索
        for (int y = 0; y < fieldW; y++)
        {
            for (int x = 0; x < fieldW; x++)
            {
                //空のタイプが対象
                //反転できる石がある
                if (fieldStone[x, y].Type == Stone.StoneType.None && HitStone(x, y))
                {
                    //カーソル表示
                    fieldCursor[x, y].gameObject.SetActive(true);
                }
            }
        }

        gameMode = GameMode.PutStone;
    }

    /// <summary>
    /// 指定の座標から8方向に反転できる石があるかを判定
    /// </summary>
    bool HitStone(int x, int y)
    {
        //この座標から8方向に判定
        var from = new Vector3(x, 0.05f, y);
        var ret = false;

        for (int i = 0; i < dir.Length; i++)
        {
            //反転できる石があるかをチェック
            if (FindOtherTypeCheak(dir[i], from, false))
            {
                canLocation[x, y] = 1;
                ret = true;
            }
        }

        return ret;
    }

    /// <summary>
    /// 現在の座標から次の座標位置に反転させる石があるかを再帰処理
    /// </summary>
    /// <param name="dir">判定する座標方向</param>
    /// <param name="from">現在の判定座標</param>
    /// <param name="insert">別の石種類を挟んだかどうか</param>
    /// <returns></returns>
    bool FindOtherTypeCheak(Vector3 dir, Vector3 from, bool insert)
    {
        //次の石の座標
        var to = from + dir;
        var x = (int)to.x;
        var y = (int)to.z;

        var ret = false;

        //次の座標がフィールド内
        //次のコマ位置に石がある
        if (OnArea(x, y) && fieldStone[x, y].Type != Stone.StoneType.None)
        {
            //判定した石が自分のタイプと同じか
            if(SameTypeStone(x, y))
            {
                ret = insert ?
                    //間に反転できる石を挟んでいればtrue
                    true
                    : 
                    //挟まず同じタイプならfalse
                    false;
            }
            else
            {
                //敵オセロが見つかれば自分のオセロが見つかるまで再帰処理
               ret = FindOtherTypeCheak(dir, to, true);
            }
        }

        return ret;
    }

    /// <summary>
    /// 全てのカーソルマークを非表示
    /// </summary>
    void CursorMarkHide()
    {
        for (int y = 0; y < fieldW; y++)
        {
            for (int x = 0; x < fieldW; x++)
            {
                fieldCursor[x, y].Hide();
            }
        }     
    }

    /// <summary>
    /// カーソルがフィールド範囲ないかを判定
    /// </summary>
    bool OnArea(int x, int y)
    {
        //①
        //xが0以上
        //②
        //xがfiledW未満
        //③
        //yが0以上
        //④
        //yがfiledW未満
        return x >= 0 && x < fieldW && y >= 0 && y < fieldW;
    }

    /// <summary>
    /// 石の種類が同じかを判定
    /// </summary>
    bool SameTypeStone(int x,int y)
    {
       return (int)fieldStone[x, y].Type - 1 == (int)trun;
    }
}