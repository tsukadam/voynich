using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;


public class GameStatus : MonoBehaviour
//ワンゲームごとのステータスを保持する

{
    //宣言
    //ゲームごとのステータスを保持する
    //ゲームプレイの間持ちつづける
    public int Point;
    public int Level;

    //レベルアップ時に操作
    public float Time;
    public int GoalPoint;
    public int DestroyPoint;
    public int GameSize;

    public string[] UseGriff;

    // Update is called once per frame
    void Update()
    {

    }


}