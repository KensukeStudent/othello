using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// フィールド管理
/// </summary>
public class FieldMan : MonoBehaviour
{
    /// <summary>
    /// 石
    /// </summary>
    [SerializeField] GameObject stoneObj;
    [SerializeField] GameObject cursorObj;

    /// <summary>
    /// フィールドの石状態
    /// </summary>
    Stone[,] fieldStone;
    /// <summary>
    /// フィールドのカーソル状態
    /// </summary>
    CursorMark[,] fieldCursor;

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
        //フィールド初期化
        fieldStone = new Stone[fieldW, fieldW];
        fieldCursor = new CursorMark[fieldW, fieldW];

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
}
