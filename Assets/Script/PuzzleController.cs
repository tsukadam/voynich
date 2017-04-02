using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class PuzzleController : MonoBehaviour
//, IPointerEnterHandler, IPointerExitHandler
{
    //パズル部分を全て制御する

    //定義
    public GameObject GameSpace;//表示スペース
    public GameObject BlockPrefab;    //ブロックプレファブ
    public GameObject PlayerStatus;    //プレイヤーstatus
    public GameObject PuzzleStatus;    //パズルstatus
    public GameObject EventSystem;    //イベントシステムの取得（処理中に切る場合がある）


    //Clumnの位置指定。GameSpace内の位置を返す
    public float GetPositionColumn(int Column)
    {
        float FloatColumn = (float)Column;
        float PositionColumn = (FloatColumn * 7 / 10) - 3;
        return PositionColumn;
    }
    //Lineの位置指定。GameSpace内の位置を返す
    public float GetPositionLine(int Line)
    {
        float FloatLine = (float)Line;
        float PositionLine = (FloatLine * 7 / 10) - (5 / 2);
        return PositionLine;
    }


    //ブロック生成関数
    public void MakeBlock(int Column, int Line,
        string Name, string Image)
    {
        GameObject NewBlock = (GameObject)Instantiate(
            BlockPrefab,
            transform.position,
            Quaternion.identity);
        NewBlock.transform.parent = GameSpace.transform;
        float PositionColumn = GetPositionColumn(Column);
        float PositionLine = GetPositionLine(Line);

        NewBlock.transform.position = new Vector3(PositionColumn, PositionLine, -3);
//        NewBlock.transform.localScale= new Vector3(1,1,1);

        //Textの字がNameの字になる（本番では画像がImageの画像になる）
        NewBlock.GetComponent<TextMesh>().text = Name;

        //ブロックの情報をBlockStatusに書き込み
        NewBlock.GetComponent<BlockStatus>().Column = Column;
        NewBlock.GetComponent<BlockStatus>().Line = Line;
        NewBlock.GetComponent<BlockStatus>().Name = Name;
        NewBlock.GetComponent<BlockStatus>().Image = Image;
        string TagColumn = Column.ToString();
        string TagLine = Line.ToString();
        string TagName = "Block" + TagColumn + "-" + TagLine;
        NewBlock.tag = TagName;
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

                int NameRandom = Random.Range(1, 6);
                string Name = "Name";
                if (NameRandom == 1) { Name = "A"; }
                else if (NameRandom == 2) { Name = "B"; }
                else if (NameRandom == 3) { Name = "C"; }
                else if (NameRandom == 4) { Name = "D"; }
                else if (NameRandom == 5) { Name = "E"; }
                else { Name = "F"; }

                string Image = "Image";

                MakeBlock(CountLine, CountColumn, Name,Image);
                CountLine++;
            }
            CountColumn++;
        }
    }

    //全消し
    public void AllDestroy()
    {
        int CountColumn = 1;
        int CountLine = 1;

        GameObject CheckBlock;
        string FindTagC;
        string FindTagL;
        string FindTagName;

        while (CountColumn <= 8)
        {
            CountLine = 1;
            while (CountLine <= 8)
            {
                FindTagC = CountColumn.ToString();
                FindTagL = CountLine.ToString();
                FindTagName = "Block" + FindTagC + "-" + FindTagL;
                if (GameObject.FindGameObjectWithTag(FindTagName) == null) { break; }
                else {
                    CheckBlock = GameObject.FindGameObjectWithTag(FindTagName);
                    Destroy(CheckBlock);
                }

                CountLine++;
            }
            CountColumn++;
        }
    }


    //ブロックをまとめて動かす関数
    /*
    int Count;//動かす回数
    int ColumnOrLine;//0=Column方向,左右 1=Line方向,上下
    int IncOrDec;//移動方向。0=減少（下か左） 1=増加（上か右）
    int EndPoint//動きの起点
    int Stage//動く列または行指定
　　*/
    public void MoveBlock(int Count, int ColumnOrLine, int IncOrDec, int EndPoint, int Stage)
    {
        int MoveToColumn = 0;
        int MoveToLine = 0;
        float MoveToPositionColumn;
        float MoveToPositionLine;
        string TagColumn;
        string TagLine;
        string TagName;
        GameObject MovingBlock;
        string FindTagColumn = "";
        string FindTagLine = "";
        string FindTagName = "";
        int MoveOne;

        if (IncOrDec == 0) { MoveOne = 1; }//Dragginの動きが減少なら、他のブロックは上に上がる
        else { MoveOne = -1; }//Dragginの動きが増加なら、他のブロックは下に下がる

        int WhileCount = 0;
        while (WhileCount < Count)
        {
            //動くBlockを取得
            if (ColumnOrLine == 0)//左右の動きならばLineは固定
            {
                FindTagColumn = (EndPoint - (MoveOne * WhileCount)).ToString();
                FindTagLine = Stage.ToString();
                MoveToColumn = EndPoint - (MoveOne * (WhileCount + 1));
                MoveToLine = Stage;
            }
            else//上下の動きならばColumnは固定
            {
                FindTagColumn = Stage.ToString();
                FindTagLine = (EndPoint - (MoveOne * WhileCount)).ToString();
                MoveToColumn = Stage;
                MoveToLine = EndPoint - (MoveOne * (WhileCount + 1));

            }
            FindTagName = "Block" + FindTagColumn + "-" + FindTagLine;
                       Debug.Log(FindTagName);

            if ((EndPoint - (MoveOne * WhileCount)) < 0 | (EndPoint - (MoveOne * WhileCount)) > 8)
            {
                Debug.Log("0以下か9以上の行か列を動かそうとしている");
                break;
            }
            if (GameObject.FindGameObjectWithTag(FindTagName)==null)
            {
                Debug.Log("調べたタグで引きあたるオブジェクトがない");
                break;
            }

            MovingBlock = GameObject.FindGameObjectWithTag(FindTagName);
            MoveToPositionColumn = GetPositionColumn(MoveToColumn);
            MoveToPositionLine = GetPositionLine(MoveToLine);
            MovingBlock.transform.position = new Vector3(MoveToPositionColumn, MoveToPositionLine, -3);

            TagColumn = MoveToColumn.ToString();
            TagLine = MoveToLine.ToString();
            TagName = "Block" + TagColumn + "-" + TagLine;
            MovingBlock.tag = TagName;

            WhileCount++;
        }
    }


    //消去判定
    public void CheckDestroyBlock()
    {
        StartCoroutine("CheckDestroyBlockCoroutine");
    }
    IEnumerator CheckDestroyBlockCoroutine()
    {
        int CountColumn = 1;
        int CountLine = 1;
        int CountBig = 1;
        int CheckColumn1;
        int CheckColumn2;
        int CheckColumn3;
        GameObject CheckBlock1;
        GameObject CheckBlock2;
        GameObject CheckBlock3;
        string FindTagC1;
        string FindTagC2;
        string FindTagC3;
        string FindTagL1;
        string FindTagL2;
        string FindTagL3;
        string FindTagName1;
        string FindTagName2;
        string FindTagName3;
        string Griff1;
        string Griff2;
        string Griff3;

        while (CountBig <= 2)//一周目は横方向、二周目は縦方向の揃いをチェック
        {
            CountLine = 1;
            while (CountLine <= 8)
            {
                CountColumn = 1;
                while (CountColumn <= 6)
                {
                    CheckColumn1 = CountColumn;
                    CheckColumn2 = CountColumn + 1;
                    CheckColumn3 = CountColumn + 2;

                    FindTagC1 = CheckColumn1.ToString();
                    FindTagC2 = CheckColumn2.ToString();
                    FindTagC3 = CheckColumn3.ToString();
                    FindTagL1 = CountLine.ToString();
                    FindTagL2 = CountLine.ToString();
                    FindTagL3 = CountLine.ToString();
                    if (CountBig == 1) { 

                        FindTagName1 = "Block" + FindTagC1 + "-" + FindTagL1;
                        FindTagName2 = "Block" + FindTagC2 + "-" + FindTagL2;
                        FindTagName3 = "Block" + FindTagC3 + "-" + FindTagL3;
                   }
                    else{
                        FindTagName1 = "Block" + FindTagL1 + "-" + FindTagC1;
                        FindTagName2 = "Block" + FindTagL2 + "-" + FindTagC2;
                        FindTagName3 = "Block" + FindTagL3 + "-" + FindTagC3;

                        }

                    CheckBlock1 = GameObject.FindGameObjectWithTag(FindTagName1);
                    CheckBlock2 = GameObject.FindGameObjectWithTag(FindTagName2);
                    CheckBlock3 = GameObject.FindGameObjectWithTag(FindTagName3);
                    Griff1 = CheckBlock1.GetComponent<BlockStatus>().Name;
                    Griff2 = CheckBlock2.GetComponent<BlockStatus>().Name;
                    Griff3 = CheckBlock3.GetComponent<BlockStatus>().Name;
                    if (Griff1 == Griff2 & Griff2 == Griff3)
                    {
                        CheckBlock1.GetComponent<BlockStatus>().DestroyFlag=1;
                        CheckBlock2.GetComponent<BlockStatus>().DestroyFlag = 1;
                        CheckBlock3.GetComponent<BlockStatus>().DestroyFlag = 1;
                        CheckBlock1.GetComponent<TextMesh>().color = new Color(1.0f, 0, 0, 1.0f);
                        CheckBlock2.GetComponent<TextMesh>().color = new Color(1.0f, 0, 0, 1.0f);
                        CheckBlock3.GetComponent<TextMesh>().color = new Color(1.0f, 0, 0, 1.0f);
                    }
                    CountColumn++;
                }
                CountLine++;
            }
            CountBig++;
        }
        DestroyBlock();
        yield return null;
    }


    //消去処理
    public void DestroyBlock()
    {
        StartCoroutine("DestroyBlockCoroutine");
    }
    IEnumerator DestroyBlockCoroutine()
    {
        yield return new WaitForSeconds(0.5f);//遅延時間
        int CountColumn = 1;
        int CountLine = 1;

        GameObject CheckBlock;
        string FindTagC;
        string FindTagL;
        string FindTagName;
        int DestroyFlag =0;

        while (CountColumn <= 8)
        {
            CountLine = 1;
            while (CountLine <= 8)
            {
                FindTagC = CountColumn.ToString();
                FindTagL = CountLine.ToString();
                FindTagName = "Block" + FindTagC + "-" + FindTagL;
                CheckBlock= GameObject.FindGameObjectWithTag(FindTagName);
                DestroyFlag = CheckBlock.GetComponent<BlockStatus>().DestroyFlag;

                if (DestroyFlag == 1)
                {
                    Destroy(CheckBlock);
                }
                CountLine++;
            }
            CountColumn++;
        }
        SupplyBlock();
        yield return null;
    }

    //落下・補充処理
    public void SupplyBlock()
    {
        StartCoroutine("SupplyCoroutine");
    }
    IEnumerator SupplyCoroutine()
    {
        yield return new WaitForSeconds(0.5f);//遅延時間
        int CountColumn = 1;
        int CountLine = 1;
        GameObject CheckBlock;
        string FindTagC;
        string FindTagL;
        string FindTagName;

        int NullStartFlag = 0;
        int FallBlockStartFlag = 0;
        int FallGoFlag = 0;
        int FallEndFlag = 0;

        int CountNull = 0;
        int CountFallBlock = 0;
        int CountFalling = 0;

        int EndPoint=0;

        while (CountColumn <= 8)
        {
            //その列の全ての落下処理が終わるまで、ここからやり直す
            CountLine = 1;
            NullStartFlag = 0;
            FallBlockStartFlag = 0;
            CountNull = 0;
            CountFallBlock = 0;
            FallEndFlag = 0;
            FallGoFlag = 0;
            EndPoint = 0;
            while (CountLine <= 8)
            {
                FindTagC = CountColumn.ToString();
                FindTagL = CountLine.ToString();
                FindTagName = "Block" + FindTagC + "-" + FindTagL;
                Debug.Log(CountColumn + "-" + CountLine + FindTagName);

                if (CountLine<8) {
                    if (GameObject.FindGameObjectWithTag(FindTagName) == null)
                    {
                        if (NullStartFlag == 0 & FallBlockStartFlag == 0)//CountNullを動かすべき初回。NullStartFlagを立てる時。
                        {
                            NullStartFlag = 1;
                            CountNull++;
                        }
                        else if (NullStartFlag != 0 & FallBlockStartFlag != 0)//一サイクル数え終わったパターン。落下処理へ
                        {
                            FallGoFlag = 1;
                            EndPoint = CountLine-1;
                            Debug.Log(CountColumn + "-" + CountLine + "G");
                        }
                        else if (NullStartFlag != 0 & FallBlockStartFlag == 0)//CountNullを動かすべき時。
                        {
                            CountNull++;
                        }
                        else
                        {
                            Debug.Log("存在しないパターンのはず");
                        }

                    }
                    else {
                        if (NullStartFlag == 0 & FallBlockStartFlag == 0)//まだnullに一度もあたっていない時。カウント不要
                        {
                        }
                        else if (NullStartFlag != 0 & FallBlockStartFlag != 0)//２回目のNull待ち。CountFallBlockを動かすべき時
                        {
                            CountFallBlock++;
                        }
                        else if (NullStartFlag != 0 & FallBlockStartFlag == 0)//CountNullをもう数えずに終わらせ、CountFallBlockを動かすべき初回。FallBlockStartFlagを立てる時。
                        {
                            CountFallBlock++;
                            FallBlockStartFlag = 1;
                        }
                        else
                        {
                            Debug.Log("存在しないパターンのはず");
                        }
                    }
                }
                else//８列目の時
                {
                    if (GameObject.FindGameObjectWithTag(FindTagName) == null)
                    {
                        if (NullStartFlag == 0 & FallBlockStartFlag == 0)//一つだけ消えてるパターン。落下処理へ、しかし今のところ処理なし
                        {
                            FallEndFlag = 1;//今だけ
                            Debug.Log(CountColumn+"-"+CountLine+"A");
                        }
                        else if (NullStartFlag != 0 & FallBlockStartFlag != 0)//８行目で一サイクル数え終わったパターン。落下処理へ
                        {
                            EndPoint = CountLine - 1;
                            FallGoFlag = 1;
                            Debug.Log(CountColumn + "-" + CountLine + "B");
                        }
                        else if (NullStartFlag != 0 & FallBlockStartFlag == 0)//８行目まで消え続けているパターン。CountNullを動かして落下処理へ、しかし今のところ処理なし
                        {
                            CountNull++;
                            FallEndFlag = 1;//今だけ
                            Debug.Log(CountColumn + "-" + CountLine + "C");
                        }
                        else
                        {
                            Debug.Log("存在しないパターンのはず");
                        }

                    }
                    else {
                        if (NullStartFlag == 0&FallBlockStartFlag==0)//一つも消えていないか、落下処理が全て終わったパターン。FallEndFlagを立てて列のループ終了
                        {
                            FallEndFlag = 1;
                            Debug.Log(CountColumn + "-" + CountLine + "D");
                        }
                        else if (NullStartFlag != 0 & FallBlockStartFlag != 0)//２回目のNull待ちで８行目まできたパターン。CountFallBlockを動かして落下処理へ
                        {
                            CountFallBlock++;
                            FallGoFlag = 1;
                            Debug.Log(CountColumn + "-" + CountLine + "E");
                        }
                        else if(NullStartFlag != 0 & FallBlockStartFlag == 0)//CountNullをもう数えずに終わらせ、CountFallBlockを動かし、落下処理へ
                        {
                            CountFallBlock++;
                            FallGoFlag = 1;
                            Debug.Log(CountColumn + "-" + CountLine + "F");
                        }
                        else
                        {
                            Debug.Log("存在しないパターンのはず");
                        }
                    }
                   
                }
                //落下処理フラグが立っている時だけ落下処理
                if (FallGoFlag == 1)
                {
                    CountLine = 8;
                    if (EndPoint == 0) { EndPoint = 8; }//EndPointを検出したパターン以外はEndPoint=8になる
                    CountFalling = 0;
                    while (CountFalling < CountNull)
                    {
                        MoveBlock(CountFallBlock, 1, 0, EndPoint, CountColumn);
                        Debug.Log(CountColumn + "-" + CountLine + "Stone:" + CountFallBlock + " EndPoint:" + EndPoint + " Column:" + CountColumn);
                        EndPoint--;
                        CountFalling++;
                        yield return new WaitForSeconds(0.5f);//遅延時間
                    }
                }
                else { }
                CountLine++;
            }
//FallEndFlagが立っている時だけ次の列へ進む
            if (FallEndFlag == 1) { CountColumn++; }
            else { }
        }
        Debug.Log("処理終了");
        yield return null;
    }
        /*
        //追加処理
        public void SupplyBlock()
        {
            StartCoroutine("SupplyCoroutine");
        }
        IEnumerator SupplyCoroutine()
        {
            yield return new WaitForSeconds(0.5f);//遅延時間

            //落下
            int CountColumn = 1;
            int CountLine = 1;
            int CountDestroyed = 0;
            int CountMove = 0;
            int MaxDestroyed = 0;
            int CountBetween = 0;
            int WhileCount = 0;

            int Count;//動かす回数
            int ColumnOrLine;//0=Column方向,左右 1=Line方向,上下
            int IncOrDec;//移動方向。0=減少（下か左） 1=増加（上か右）
            int EndPoint;//動きの起点
            int Stage;//動く列または行指定

            GameObject CheckBlock;

            string FindTagC;
            string FindTagL;
            string FindTagName;

            while (CountColumn <= 8)
            {
                CountLine = 1;
                MaxDestroyed = 0;
    ///////////まずはDestroyedの個数を数える
                while (CountLine <= 8)
                {
                    FindTagC = CountColumn.ToString();
                    FindTagL = CountLine.ToString();
                    FindTagName = "Block" + FindTagC + "-" + FindTagL;
                    if (GameObject.FindGameObjectWithTag(FindTagName) == null) {
                        MaxDestroyed++;
    ///////////nullに当たった数だけMaxDestroyedが増える。0から。
                    }
                    CountLine++;
                }
    ///////////Lineの数ぶんループを回し終わる。
    ///////////CountLineを1に戻して、再び下から。
    ///////////MaxDestroyedの数にいたるまで回す
                CountDestroyed = 0;
                CountLine = 1;
                while (CountDestroyed <= MaxDestroyed)
                {
    ///////////Destoyedにあたるまで下から順にあげていく
                    while (CountLine <= 8)
                    {
                        FindTagC = CountColumn.ToString();
                        FindTagL = CountLine.ToString();
                        FindTagName = "Block" + FindTagC + "-" + FindTagL;
    ///////////Destroyedにあたったら、そこから次の非Destroyedにあたる又はライン上限に達するまで数える。
    ///////////その数ぶんだけ、上からスクロールしてこなければならない。
    ///////////ただし新規ブロックを追加しなければ足りない分は、追加処理を入れる必要があり、後回しにしたい。
                        if (GameObject.FindGameObjectWithTag(FindTagName) == null)
                        {
                            CountMove = 1;
                            CountLine++;//Destroyedの真上からスタート
                            while (CountLine <= 8)
                            {
                                FindTagC = CountColumn.ToString();
                                FindTagL = CountLine.ToString();
                                FindTagName = "Block" + FindTagC + "-" + FindTagL;
    ///////////Moveをカウント
                                if ((GameObject.FindGameObjectWithTag(FindTagName) == null) &(CountLine < 8))
                                {
                                    CountMove++;
                                }
                                else//非Destroyedにあたったらbreakしてループ再開
                                {
                                    break;
                                }
                            }
                            Debug.Log(CountColumn + "列目MoveCount数え終わり（"+CountMove+"）");

    ///////////////次のDestroyedにあたる又はライン上限に達するまで数える。
    //////////////その数が動くブロックの数である。

                            CountBetween = 0;
                            while (CountLine <= 8)
                            {
                                FindTagC = CountColumn.ToString();
                                FindTagL = CountLine.ToString();
                                FindTagName = "Block" + FindTagC + "-" + FindTagL;
                                if ((GameObject.FindGameObjectWithTag(FindTagName) != null)&(CountLine < 8))
    /////////////////非Destroyedの間、Betweenをカウント
                                {
                                    CountBetween++;
                                }
                                else
    /////////////////Destroyedにあたったら落下処理をして、二個目のDestroyedの一つ上からループ再開。またDestroyedにあたるまで
    /////////////////補充がいる時(Between=0)は落下処理をしない
                                {
                                    if (CountBetween == 0) {
                                        Debug.Log(CountColumn + "列目補充要、飛ばす");
                                        break;
                                    }
                                    Count = CountBetween;
                                    ColumnOrLine =1;
                                    IncOrDec = 0;
                                    EndPoint = CountLine;
                                    Stage =CountColumn;
                                    WhileCount = 0;
                                    Debug.Log(CountColumn + "列目落下処理前");
                                    while (WhileCount< CountMove) {
                                        Debug.Log(CountColumn + "列目Count:" + Count + " ColumnOrLine:" + ColumnOrLine + " IncOrDec:" + IncOrDec + " EndPoint:" + EndPoint + " Stage:" + Stage);
                                        MoveBlock(Count, ColumnOrLine, IncOrDec, EndPoint, Stage);
                                        EndPoint--;
                                        WhileCount++;
                                    }
                                   yield return new WaitForSeconds(0.2f);//遅延時間
                                    break;
                                }
                                CountLine++;
                                Debug.Log(CountColumn + "A");
                            }
                            //落下処理後のbreakはここに出る
                            CountLine++;
                            Debug.Log(CountColumn + "B");
                        }
                        Debug.Log(CountColumn + "C");
                        CountLine++;
                    }
                    Debug.Log(CountColumn + "D");
                    CountLine++;
                        CountDestroyed++;
                }
                Debug.Log(CountColumn + "E");
                Debug.Log(CountColumn+"列目処理終わり");
                CountColumn++;
            }
            Debug.Log("処理おわり");
            yield return null;
        }
        */

    }
