using System.Collections;
using System;
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
        PlayerTurn,//置ける石をチェック
        PutStone,//プレイヤーが石を置く
        EnemyTrun,//敵のターン
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
    /// 石オブジェクト
    /// </summary>
    [SerializeField] GameObject stoneObj;
    /// <summary>
    /// カーソルオブジェクト
    /// </summary>
    [SerializeField] GameObject cursorObj;
    
    /// <summary>
    /// フィールド範囲
    /// </summary>
    const int fieldW = 8;

    /// <summary>
    /// フィールドの石状態
    /// </summary>
    Stone[,] fieldStone = new Stone[fieldW, fieldW];
    /// <summary>
    /// フィールドのカーソル状態
    /// </summary>
    CursorMark[,] fieldCursor = new CursorMark[fieldW, fieldW];

    /// <summary>
    /// 探索する座標方向
    /// </summary>
    Vector2[] dir =
    {
          new Vector2(  0 , 1),  // up
          new Vector2(  1 , 1),  // up-right
          new Vector2(  1 , 0),  // right
          new Vector2(  1 ,-1),  // down-right
          new Vector2(  0 ,-1),  // down
          new Vector2( -1 ,-1),  // down-left
          new Vector2( -1 , 0),  // left
          new Vector2( -1 ,1),  // up-left
    };

    /// <summary>
    /// 石配置可能位置
    /// </summary>
    CanLocation[,] canLocation = new CanLocation[fieldW, fieldW];

    /// <summary>
    /// 石の情報があるクラス
    /// </summary>
    StoneMan stoneMan = new StoneMan();
    /// <summary>
    /// 石回転アニメーション中
    /// </summary>
    bool trunStones = false;

    /// <summary>
    /// このターンにパスした回数
    /// </summary>
    int passCount = 0;
    /// <summary>
    /// 黒タイプの石の数
    /// </summary>
    int blackStoneCount = 0;
    /// <summary>
    /// 白タイプの石の数
    /// </summary>
    int whiteStoneCount = 0;

    /// <summary>
    /// UI管理
    /// </summary>
    UIMan uiMan;

    private void Start()
    {
        uiMan = GameObject.FindWithTag("UIMan").GetComponent<UIMan>();
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

                canLocation[x, y] = new CanLocation();
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
        if (gameMode == GameMode.GameOver) return;

        if (trunStones)
        {
            //現在アニメーション中
            if(stoneMan.TrunStones())
            {
                trunStones = false;

                //個数のチェック
                StoneCountCheak();
            }

            return;
        }

        //操作可能
        InputMode();
    }

    /// <summary>
    /// 操作モード
    /// </summary>
    void InputMode()
    {
        switch (gameMode)
        {
            //初期化
            case GameMode.Initialize:

                gameMode = GameMode.PlayerTurn;
                break;

            //置ける石を判定
            case GameMode.PlayerTurn:

                //石を置ける個数が1つ以上ならプレイヤーターン
                if (PlayerPutCheak() == 0)
                {
                    gameMode = GameMode.EnemyTrun;
                    passCount++;
                }
                else
                {
                    gameMode = GameMode.PutStone;
                }
                break;

            //プレイヤーが石を置く
            case GameMode.PutStone:

                PlayerPutStone();
                break;

            //敵のターン
            case GameMode.EnemyTrun:

                //石をセット
                AIPutStone(AIPutCheak());
                break;
        }
    }

    #region プレイヤー処理

    /// <summary>
    /// 石を置けるコマをチェック
    /// </summary>
    int PlayerPutCheak()
    {
        //コマを置ける個数
        var putCount = 0;

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
                    //配置可能
                    canLocation[x, y].SetPut(true);

                    putCount++;
                }
            }
        }

        return putCount;
    }

    /// <summary>
    /// 指定の座標から8方向に反転できる石があるかを判定
    /// </summary>
    bool HitStone(int x, int y)
    {
        //この座標から8方向に判定
        var from = new Vector3(x, y);
        var ret = false;

        for (int i = 0; i < dir.Length; i++)
        {
            //反転できる石があるかをチェック
            var turnCheak = FindOtherTypeCheak(dir[i], from, false);
            //初期化
            canLocation[x, y].SetTrunDirection(i, turnCheak);

            if (turnCheak != Vector2.zero)
            {
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
    Vector2 FindOtherTypeCheak(Vector2 dir, Vector2 from, bool insert)
    {
        //次の石の座標
        var to = from + dir;
        var x = (int)to.x;
        var y = (int)to.y;

        //次の座標がフィールド内
        //次のコマ位置に石がある
        if (OnArea(x, y) && fieldStone[x, y].Type != Stone.StoneType.None)
        {
            //判定した石が自分のタイプと同じか
            if (SameTypeStone(x, y))
            {
                //間に反転できる石を挟んでいればtrue
                if (insert)
                {
                    return to;
                }
                //挟まず同じタイプならfalse
                else
                {
                    return Vector3.zero;
                }
            }
            else
            {
                //敵オセロが見つかれば自分のオセロが見つかるまで再帰処理
               return FindOtherTypeCheak(dir, to, true);
            }
        }

        return Vector3.zero;
    }

    /// <summary>
    /// 石を置く
    /// </summary>
    void PlayerPutStone()
    {
        var location = CursorMan.CursorPos;
        var x = (int)location.x;
        var y = (int)location.z;

        //マウスカーソルがフィールド内であれば処理
        if (OnArea(x, y) && Input.GetMouseButtonDown(0) && canLocation[x, y].Put)
        {
            PutProsses(x, y);

            //カーソルマークを非表示
            CursorMarkHide();
        }
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
                canLocation[x, y].SetPut(false);
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
    bool SameTypeStone(int x, int y)
    {
        return (int)fieldStone[x, y].Type - 1 == (int)trun;
    }

    #endregion

    #region 敵AI処理

    /// <summary>
    /// 敵の動き
    /// </summary>
    List<Vector2> AIPutCheak()
    {
        //置ける座標を保存
        var putStore = new List<Vector2>();

        //0,0～7,7まで探索
        for (int y = 0; y < fieldW; y++)
        {
            for (int x = 0; x < fieldW; x++)
            {
                //空のタイプが対象
                //反転できる石がある
                if (fieldStone[x, y].Type == Stone.StoneType.None && HitStone(x, y))
                {
                    //座標を保存
                    putStore.Add(new Vector2(x, y));
                }
            }
        }

        return putStore;
    }

    /// <summary>
    /// 敵が石を置く
    /// </summary>
    void AIPutStone(List<Vector2> putStore)
    {
        if (putStore.Count != 0)
        {
            //ランダムで置く位置を選択
            var maxCount = putStore.Count;

            var putLcation = putStore[UnityEngine.Random.Range(0, maxCount)];

            PutProsses((int)putLcation.x, (int)putLcation.y);
        }
        else
        {
            gameMode = GameMode.PlayerTurn;
            trun = GoTrun.BlackTrun;
            passCount++;

            if(passCount == 2)
            {
                //ゲームオーバー処理
                GameOver();
            }

            passCount = 0;
        }
    }

    #endregion

    /// <summary>
    /// タイプ別の石をひっくり返す処理
    /// </summary>
    void TrunStone(int x, int y, Stone.StoneType stoneType)
    {
        var from = new Vector2(x, y);
        var trunLag = 0.025f;

        for (int i = 0; i < dir.Length; i++)
        {
            //座標があるなら
            if (canLocation[x, y].TrunDirection[i] != Vector2.zero)
            {
                //この座標までひっくり返し
                var to = canLocation[x, y].TrunDirection[i];
                //toまでの距離
                var distanceTo = 0;

                for (Vector2 location = from + dir[i]; location != to; location += dir[i])
                {
                    var stone = fieldStone[(int)location.x, (int)location.y];
                    //石の回転情報をセット
                    stone.SetTrun(trunLag * distanceTo, stoneMan.GetDirection(i));

                    //回転する石を管理庫にセット
                    stoneMan.SetTrunStone(stone);
                    
                    distanceTo++;
                }
            }
        }

        trunStones = true;
    }

    /// <summary>
    /// 石配置処理コルーチン
    /// </summary>
    void PutProsses(int x, int y)
    {
        //ターン別でストーンの種類を変更
        var stoneType = trun == GoTrun.BlackTrun ?
            Stone.StoneType.Black
            :
            Stone.StoneType.White;

        //石を置く
        fieldStone[x, y].SetStone(stoneType);

        //石をひっくり返し
        TrunStone(x, y, stoneType);

        //配置処理退場処理
        PutExit(stoneType);
    }

    /// <summary>
    /// 現在の各石の色の個数をチェック
    /// </summary>
    void StoneCountCheak()
    {
        //フィールド上の石の個数
        var fliedNoneCount = 0;

        //0,0～7,7まで探索
        for (int y = 0; y < fieldW; y++)
        {
            for (int x = 0; x < fieldW; x++)
            {
                if (fieldStone[x, y].Type == Stone.StoneType.Black)
                {
                    blackStoneCount++;
                }
                else if (fieldStone[x, y].Type == Stone.StoneType.White)
                {
                    whiteStoneCount++;
                }
                else
                {
                    fliedNoneCount++;
                }
            }
        }

        //表示カウントセット
        uiMan.SetCount(blackStoneCount, whiteStoneCount);

        //フィールド全てに石が埋まった
        //どちらかの石が０個
        if(fliedNoneCount == 0 || blackStoneCount == 0 || whiteStoneCount == 0)
        {
            GameOver();
            return;
        }

        //個数確認後初期化
        blackStoneCount = 0;
        whiteStoneCount = 0;
    }

    /// <summary>
    /// 石配置処理後
    /// </summary>
    void PutExit(Stone.StoneType stoneType)
    {
        switch (stoneType)
        {
            case Stone.StoneType.Black:
                
                //敵のターン
                gameMode = GameMode.EnemyTrun;
                trun = GoTrun.WhiteTrun;

                break;
            
            case Stone.StoneType.White:

                //プレイヤーのターン
                gameMode = GameMode.PlayerTurn;
                trun = GoTrun.BlackTrun;
                break;
        }
    }

    /// <summary>
    /// ゲームオーバー処理
    /// </summary>
    void GameOver()
    {
        //ゲーム終了
        gameMode = GameMode.GameOver;
        uiMan.SetGameOver();
    }
}