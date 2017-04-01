using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PuzzleController : MonoBehaviour
{
    //パズル部分を全て制御する

    //定義
    public GameObject GameSpace;//表示スペース
    public GameObject BlockPrefab;    //ブロックプレファブ
    public GameObject PlayerStatus;    //プレイヤーstatus
    public GameObject PuzzleStatus;    //パズルstatus
    public GameObject EventSystem;    //イベントシステムの取得（処理中に切る場合がある）


    //ブロック生成関数
    public void MakeBlock(int Column, int Line,
        string Name, string Image)
    {
        GameObject NewBlock = (GameObject)Instantiate(
            BlockPrefab,
            transform.position,
            Quaternion.identity);
        NewBlock.transform.parent = GameSpace.transform;
        int PositionLine = Line;
        int PositionColumn = Column;
        NewBlock.transform.position = new Vector3(PositionLine, PositionColumn, 0);

        //ブロックの情報をBlockStatusに書き込み
        NewBlock.GetComponent<BlockStatus>().Line = Line;
        NewBlock.GetComponent<BlockStatus>().Column = Column;
        NewBlock.GetComponent<BlockStatus>().Name = Name;
        NewBlock.GetComponent<BlockStatus>().Image = Image;

        Debug.Log("Make");
    }


    //ゲーム開始時の生成

    public void StartMakeBlock()
    {
        int CountColumn = 1;
        int CountLine = 1;
        while (CountColumn <= 8)
        {
            CountLine = 1;
            while (CountLine <= 8)
            {

                string Name = "Name";
                string Image = "Image";

                MakeBlock(CountLine, CountColumn, Name,Image);
                CountLine++;
            }
            CountColumn++;
        }
    }
    
}
