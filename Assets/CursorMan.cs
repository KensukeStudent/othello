using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// マウスカーソル
/// </summary>
public class CursorMan : MonoBehaviour
{
    /// <summary>
    /// カーソル座標
    /// </summary>
    public Vector3 CursorPos { private set; get; }

    private void Update()
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //整数化
        pos.x = Mathf.FloorToInt(pos.x);
        pos.y = 0.1f;
        pos.z = Mathf.FloorToInt(pos.z);

        CursorPos = pos;
    }
}
