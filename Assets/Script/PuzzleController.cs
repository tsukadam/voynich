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
    public GameObject GameStatus;    //ゲームstatus
    public GameObject EventSystem;    //イベントシステムの取得（処理中に切る場合がある）

    public int GameSize;//行と列の大きさ。とりあえず８だが、スタート時にGameStatusから代入して変えられるように

    public GameObject PointUI;    //ポイント表記
    public GameObject DestroyUI;    //消した数表記

    //タップ切るための板
    public GameObject TapBlock;

    public GameObject DebugText;//デバッグ用
    public int DestroyAmount=0;//直前に消した数。０のとき消去判定を止めるために使う

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
        float FloatGameSize = (float)GameSize;
        float Scale = 8/ FloatGameSize*2/3;
        NewBlock.transform.localScale= new Vector3(Scale, Scale, 1);
//↑盤面全体をScaleする仕様のほうがいい

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

    //どのブロックが作られるか決める関数
    //ゆくゆくは、消した総数が一定に達するなどのタイミングでGameStatusに登場ブロックを格納させ、
    //その格納種類に応じてランダムで発生させる
    //データベースにアクセスするのはこいつ
    //Nameを決めて、それ以外はMakeBlockに横流す
    public void DecideBlock(int Column, int Line)
    {
        string Name="";
        string Image="";
        int NameRandom = Random.Range(1, 7);
        //bdesqn
        if (NameRandom == 1) { Name = "b"; }
        else if (NameRandom == 2) { Name = "d"; }
        else if (NameRandom == 3) { Name = "e"; }
        else if (NameRandom == 4) { Name = "s"; }
        else if (NameRandom == 5) { Name = "q"; }
        else if (NameRandom == 6) { Name = "n"; }
        else { Name = "w"; }
        MakeBlock( Column, Line, Name, Image);
    }


    //ゲーム開始時の生成
    public void StartMakeBlock()
    {
        StartCoroutine("StartMakeBlockCoroutine");
    }
    public IEnumerator StartMakeBlockCoroutine()
    {
        GameSize = 8;//ココで最初のGamzeSizeを定義。ゆくゆくは関数作って戻り値で定義する
            TapBlock.SetActive(true);//操作不能
        int CountColumn = 1;
        int CountLine = 1;
        while (CountLine <= GameSize)
        {
            CountColumn = 1;
            while (CountColumn <= GameSize)
            {
                DecideBlock(CountColumn,CountLine);
           //     Debug.Log(CountColumn+"-"+GameSize);
                yield return null;
                CountColumn++;
            }
            CountLine++;
        }
        TapBlock.SetActive(false);//操作不能
        yield return null;
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

        while (CountColumn <= GameSize)
        {
            CountLine = 1;
            while (CountLine <= GameSize)
            {
                FindTagC = CountColumn.ToString();
                FindTagL = CountLine.ToString();
                FindTagName = "Block" + FindTagC + "-" + FindTagL;
                if (GameObject.FindGameObjectWithTag(FindTagName) == null) { }
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
    int Count;//一度に動かすブロックの数
    int ColumnOrLine;//0=Column方向,左右 1=Line方向,上下
    int IncOrDec;//移動方向。0=減少（下か左） 1=増加（上か右）
    int EndPoint//動きの起点。このPointからブロックを退かせる動きになる
    int Stage//動く列または行指定
    int Amount//一度に動かす量
　　*/
    public void MoveBlock(int Count, int ColumnOrLine, int IncOrDec, int EndPoint, int Stage,int Amount)
    {
        StartCoroutine(MoveBlockCoroutine(Count, ColumnOrLine, IncOrDec, EndPoint, Stage, Amount));
    }
    public IEnumerator MoveBlockCoroutine(int Count, int ColumnOrLine, int IncOrDec, int EndPoint, int Stage, int Amount)
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

        if (IncOrDec == 0) { MoveOne = 1;Amount = Amount - 1; }//Dragginの動きが減少なら、他のブロックは上に上がる
        else { MoveOne = -1 ; Amount = (Amount-1) * -1; }//Dragginの動きが増加なら、他のブロックは下に下がる

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
                MoveToLine = EndPoint - (MoveOne * (WhileCount + 1))-Amount;

            }
            FindTagName = "Block" + FindTagColumn + "-" + FindTagLine;
  //                     Debug.Log(FindTagName);

            if ((EndPoint - (MoveOne * WhileCount)) < 0 | (EndPoint - (MoveOne * WhileCount)) > 16)
            {
                Debug.Log("0以下か16以上の行か列を動かそうとしている");
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

            MovingBlock.GetComponent<BlockStatus>().Column = MoveToColumn;
            MovingBlock.GetComponent<BlockStatus>().Line = MoveToLine;
            TagColumn = MoveToColumn.ToString();
            TagLine = MoveToLine.ToString();
            TagName = "Block" + TagColumn + "-" + TagLine;
            MovingBlock.tag = TagName;

            WhileCount++;
        }
        yield return null;
    }

    //ドラッグ終了時の挙動。BlockStatusからのみ呼び出し可
    public IEnumerator OnEndDragCoroutine(
        PointerEventData pointerEventData,
        GameObject DraggingBlock,
        GameObject DraggedBlock,
        int StartColumn,
        int StartLine,
        int NowOnColumn,
        int NowOnLine,
        int PrevOnColumn,
        int PrevOnLine,
        int EndColumn,
        int EndLine,
        Vector3 ScreenPoint,
        Vector3 MousePoint,
        Vector3 Offset,
        int DraggingColumn,
        int DraggingLine,
        RaycastHit2D hit
       )
    {
        TapBlock.SetActive(true);//操作不能
        //ヒットがない時、ドラッグされている対象がスタート地点から上下左右の時は、スタート地点をエンド地点とする
        if (!hit)
        {
            EndColumn = StartColumn;
            EndLine = StartLine;
        }
        else
        {
            if ((DraggingColumn != DraggedBlock.GetComponent<BlockStatus>().Column) & (DraggingLine != DraggedBlock.GetComponent<BlockStatus>().Line))
            {
                EndColumn = StartColumn;
                EndLine = StartLine;
            }
            else {
                //エンド地点の取得
                EndColumn = DraggedBlock.GetComponent<BlockStatus>().Column;
                EndLine = DraggedBlock.GetComponent<BlockStatus>().Line;
            }
        }
        Debug.Log(EndColumn + "-" + EndLine + ":DragEnd");

        //スタート地点とエンド地点を比較
        //同じならDraggingをスタート地点に戻す
        //違うなら
        //スタート地点とエンド地点の間にあるブロックは、スタート地点方向に１ずつ動く
        //Draggingはエンド地点に動く

        float PositionColumn;
        float PositionLine;
        if (StartColumn == EndColumn & StartLine == EndLine)
        {
            PositionColumn = GetPositionColumn(StartColumn);
            PositionLine = GetPositionLine(StartLine);
            DraggingBlock.transform.position = new Vector3(PositionColumn, PositionLine, -3);
            Debug.Log("Start=End");
            TapBlock.SetActive(false);//操作不能
        }
        else {
            int Count;//動かす回数
            int Stage;//動く行または列番号
            int EndPoint;//数える起点、エンド地点のLineまたはColumn
            int ColumnOrLine;//0=Column方向,左右 1=Line方向,上下
            int IncOrDec;//移動方向。0=減少（下か左） 1=増加（上か右）

            if (StartColumn == EndColumn)//Line方向の場合
            {
                ColumnOrLine = 1;
                EndPoint = EndLine;
                Stage = EndColumn;
                Count = Mathf.Abs(StartLine - EndLine);
                if (EndLine - StartLine < 0)//上方向の場合
                {
                    IncOrDec = 1;
                }
                else//下方向の場合
                {
                    IncOrDec = 0;
                }
            }
            else//Column方向の場合
            {
                ColumnOrLine = 0;
                EndPoint = EndColumn;
                Stage = EndLine;
                Count = Mathf.Abs(StartColumn - EndColumn);
                if (EndColumn - StartColumn < 0)//右方向の場合
                {
                    IncOrDec = 1;
                }
                else//左方向の場合
                {
                    IncOrDec = 0;
                }
            }

            //          Debug.Log("Count:" + Count + " ColumnOrLine:" + ColumnOrLine + " IncOrDec:" + IncOrDec + " EndPoint:" + EndPoint + " Stage:" + Stage);

            yield return StartCoroutine(MoveBlockCoroutine(Count, ColumnOrLine, IncOrDec, EndPoint, Stage, 1));
            //     Scripter.MoveBlock(Count,  ColumnOrLine,  IncOrDec, EndPoint, Stage,1);

            PositionColumn = GetPositionColumn(EndColumn);
            PositionLine = GetPositionLine(EndLine);
            DraggingBlock.transform.position = new Vector3(PositionColumn, PositionLine, -3);
            DraggingBlock.GetComponent<BlockStatus>().Column = EndColumn;
            DraggingBlock.GetComponent<BlockStatus>().Line = EndLine;

            string TagColumn = EndColumn.ToString();
            string TagLine = EndLine.ToString();
            string TagName = "Block" + TagColumn + "-" + TagLine;
            DraggingBlock.tag = TagName;

            yield return null;
            //動きが発生している時は消去判定を走らせる
            AllCheck();
        }
        DraggingBlock.GetComponent<Collider2D>().isTrigger = true;//ドラッグしているブロックのトリガーを戻す
        DraggingColumn = 0;
        DraggingLine = 0;
        NowOnColumn = 0;
        NowOnLine = 0;
        PrevOnColumn = 0;
        PrevOnLine = 0;
        StartColumn = 0;
        StartLine = 0;

        DraggingBlock = null;//ドラッグ終了

        yield return null;
    }


    //単語合致消去判定
    public void CheckDestroyWord()
    {
        StartCoroutine("CheckDestroyWordCoroutine");
    }
    IEnumerator CheckDestroyWordCoroutine()
    {

        int CountColumn = 1;
        int CountLine = GameSize;
        string OneText = "";
        string AllText = "";
        GameObject CheckBlock;
        string FindTagC;
        string FindTagL;
        string FindTagName;

        //全ブロックを一つの文字列にする
        //nullは#、改行は-に
        while (CountLine > 0)
        {
            CountColumn = 1;
            while (CountColumn <= GameSize)
            {
                FindTagC = CountColumn.ToString();
                FindTagL = CountLine.ToString();
                FindTagName = "Block" + FindTagC + "-" + FindTagL;
                if (GameObject.FindGameObjectWithTag(FindTagName) == null) { Debug.Log("タグで引き当てたオブジェクトがnullです");
                    OneText = "#";
                }
                else {
                    CheckBlock = GameObject.FindGameObjectWithTag(FindTagName);
                    OneText = CheckBlock.GetComponent<BlockStatus>().Name;
                }
                AllText = AllText + OneText;
                //                Debug.Log(CountLine + "-" + CountColumn);
                CountColumn++;
            }
            AllText = AllText + "-";
            CountLine--;
        }
        //辞書の単語と比較
        //bdesqn
        string[] Words = { "bdes","sqn","dde","bsn","dees" };
        int WordsMax = Words.Length;
        int WordsCount = 0;
        string Word = "";

        while (WordsCount < WordsMax)
        {
            Word = Words[WordsCount];
            //IndexOfが-1を返すまで繰り返す
            int StartPoint;
            int WordAmount = Word.Length;
            while (AllText.IndexOf(Word) != -1)
            {

                StartPoint = AllText.IndexOf(Word);

                //SPを行列に変換
                int SPColumn;
                int SPLine;
                float FloatSPLine;
                FloatSPLine = StartPoint / (GameSize+1);
                SPLine = Mathf.FloorToInt(FloatSPLine) + 1;
                SPColumn = StartPoint - ((SPLine - 1) * (GameSize + 1)) + 1;
                SPLine = GameSize - (SPLine - 1);
                //その行列の文字に消去フラグを立てる
                int WhileCount = 0;
                while (WhileCount < WordAmount)
                {
                    FindTagC = (SPColumn + WhileCount).ToString();
                    FindTagL = SPLine.ToString();
                    FindTagName = "Block" + FindTagC + "-" + FindTagL;
                    CheckBlock = GameObject.FindGameObjectWithTag(FindTagName);
                    CheckBlock.GetComponent<BlockStatus>().DestroyFlag = 1;
                    CheckBlock.GetComponent<TextMesh>().color = new Color(0, 0, 1.0f, 1.0f);
                    WhileCount++;
                }


                //引き当てた文字列を=に置換（次から引きあたらない）
                AllText = AllText.Remove(StartPoint, WordAmount);
                int ChangeCount = 0;
                while (ChangeCount < WordAmount)
                {
                    AllText = AllText.Insert(StartPoint + ChangeCount, "=");
                    ChangeCount++;
                }
         //       DebugText.GetComponent<Text>().text = AllText;
                Debug.Log(Word+" S P:" + StartPoint + " Line:" + SPLine + " Column:" + SPColumn + " Length:" + WordAmount);
            }
            WordsCount++;
        }
        yield return null;
    }


    //（揃い消去判定～単語消去判定～消去～補充）←消えなくなるまで繰り返し
    public void AllCheck()
    {
        StartCoroutine("AllCheckCoroutine");
    }
    public IEnumerator AllCheckCoroutine()
    {

        yield return StartCoroutine("CheckDestroyBlockCoroutine");
        yield return new WaitForSeconds(0.2f);//遅延時間
        yield return StartCoroutine("CheckDestroyWordCoroutine");
        yield return new WaitForSeconds(0.2f);//遅延時間
        yield return StartCoroutine("DestroyBlockCoroutine");
        yield return StartCoroutine(GetPointCoroutine(DestroyAmount));
        yield return StartCoroutine("FallBlockCoroutine");
        while (DestroyAmount > 0)
            {
                for (int i = 1; i <= GameSize; i++)
                {
                yield return StartCoroutine(SupplyBlockCoroutine(i));
                }
            yield return StartCoroutine("CheckDestroyBlockCoroutine");
            yield return new WaitForSeconds(0.2f);//遅延時間
            yield return StartCoroutine("CheckDestroyWordCoroutine");
            yield return new WaitForSeconds(0.2f);//遅延時間
            yield return StartCoroutine("DestroyBlockCoroutine");
            yield return StartCoroutine(GetPointCoroutine(DestroyAmount));
            yield return StartCoroutine("FallBlockCoroutine");
        }
        Debug.Log("処理終了");
        TapBlock.SetActive(false);//操作不能
        yield return null;
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
            while (CountLine <= GameSize)
            {
                CountColumn = 1;
                while (CountColumn <= GameSize-2)
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
  //      DestroyBlock();
        yield return null;
    }

    //得点処理

    public void GetPoint(int DestroyAmount)
    {
        StartCoroutine(GetPointCoroutine(DestroyAmount));
    }
    IEnumerator GetPointCoroutine(int DestroyAmount)
    {
        int GetPoint;
        int PoolPoint = GameStatus.GetComponent<GameStatus>().Point;
        int PoolDestroy = GameStatus.GetComponent<GameStatus>().DestroyPoint;
        GetPoint = DestroyAmount * (100+ DestroyAmount);
        GameStatus.GetComponent<GameStatus>().Point+=GetPoint;
        GameStatus.GetComponent<GameStatus>().DestroyPoint+= DestroyAmount;

        PointUI.GetComponent<Text>().text = GameStatus.GetComponent<GameStatus>().Point.ToString();
        DestroyUI.GetComponent<Text>().text = GameStatus.GetComponent<GameStatus>().DestroyPoint.ToString();

        yield return null;
    }

    //消去処理
    public void DestroyBlock()
    {
        StartCoroutine("DestroyBlockCoroutine");
    }
    IEnumerator DestroyBlockCoroutine()
    {
        int CountColumn = 1;
        int CountLine = 1;
        int CountDestroy = 0;

        GameObject CheckBlock;
        string FindTagC;
        string FindTagL;
        string FindTagName;
        int DestroyFlag =0;

        while (CountColumn <= GameSize)
        {
            CountLine = 1;
            while (CountLine <= GameSize)
            {
                FindTagC = CountColumn.ToString();
                FindTagL = CountLine.ToString();
                FindTagName = "Block" + FindTagC + "-" + FindTagL;
                CheckBlock= GameObject.FindGameObjectWithTag(FindTagName);
                DestroyFlag = CheckBlock.GetComponent<BlockStatus>().DestroyFlag;
                if (DestroyFlag == 1)
                {
                    Destroy(CheckBlock);
                    CountDestroy++;
                }
                CountLine++;
            }
            CountColumn++;
        }

        DestroyAmount = CountDestroy;
        yield return null;
    }

    //その列の落下処理をやる
    public void FallColumn(int Column)
    {
        StartCoroutine(FallColumnCoroutine(Column));
    }
    IEnumerator FallColumnCoroutine(int CountColumn)
    {
        int CountLine = 1;
        string FindTagC;
        string FindTagL;
        string FindTagName;

        int NullStartFlag = 0;
        int FallBlockStartFlag = 0;
        int FallGoFlag = 0;
        int FallEndFlag = 0;
        int UnDestroyedLine = 0;

        int CountNull = 0;
        int CountFallBlock = 0;

        int EndPoint = 0;
        while (FallEndFlag != 1)
        {
            //その列の全ての落下処理が終わるまで、ここからやり直す
            //ただし、非destroyedだとわかっているラインまでは飛ばす
            CountLine = UnDestroyedLine+1;
            NullStartFlag = 0;
            FallBlockStartFlag = 0;
            CountNull = 0;
            CountFallBlock = 0;
            FallEndFlag = 0;
            FallGoFlag = 0;
            EndPoint = 0;
//            Debug.Log(CountColumn+"列 飛ばす："+UnDestroyedLine+"まで");
            while (CountLine <= GameSize)
            {
                FindTagC = CountColumn.ToString();
                FindTagL = CountLine.ToString();
                FindTagName = "Block" + FindTagC + "-" + FindTagL;
  //              Debug.Log(CountColumn + "-" + CountLine + FindTagName);

                if (CountLine < GameSize)
                {
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
                            EndPoint = CountLine - 1;
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
                            UnDestroyedLine++;
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
                        if (NullStartFlag == 0 & FallBlockStartFlag == 0)//一つだけ消えてるパターン。ループ終了
                        {
                            FallEndFlag = 1;//今だけ
                       }
                        else if (NullStartFlag != 0 & FallBlockStartFlag != 0)//８行目で一サイクル数え終わったパターン。落下処理へ
                        {
                            EndPoint = CountLine - 1;
                            FallGoFlag = 1;
                        }
                        else if (NullStartFlag != 0 & FallBlockStartFlag == 0)//８行目まで消え続けているパターン。CountNullを動かしてループ終了
                        {
                            CountNull++;
                            FallEndFlag = 1;//今だけ
                        }
                        else
                        {
                            Debug.Log("存在しないパターンのはず");
                        }

                    }
                    else {
                        if (NullStartFlag == 0 & FallBlockStartFlag == 0)//一つも消えていないか、落下処理が全て終わったパターン。FallEndFlagを立てて列のループ終了
                        {
                            FallEndFlag = 1;
                        }
                        else if (NullStartFlag != 0 & FallBlockStartFlag != 0)//２回目のNull待ちで８行目まできたパターン。CountFallBlockを動かして落下処理へ
                        {
                            CountFallBlock++;
                            FallGoFlag = 1;
                        }
                        else if (NullStartFlag != 0 & FallBlockStartFlag == 0)//CountNullをもう数えずに終わらせ、CountFallBlockを動かし、落下処理へ
                        {
                            CountFallBlock++;
                            FallGoFlag = 1;
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
                    CountLine = GameSize;
                    if (EndPoint == 0) { EndPoint = GameSize; }//EndPointを検出したパターン以外はEndPoint=8になる
                    StartCoroutine(MoveBlockCoroutine(CountFallBlock, 1, 0, EndPoint, CountColumn, CountNull));
 //                   Debug.Log(CountColumn + "-" + CountLine + "Stone:" + CountFallBlock + " EndPoint:" + EndPoint + " Column:" + CountColumn + " Null:" + CountNull);
                    // yield return new WaitForSeconds(0.2f);//遅延時間
                }
                else { }
                CountLine++;
            }
        }
        //FallEndFlagが立っている時だけ処理終了
        yield return null;
    }

    //落下処理
    public void FallBlock()
    {
        StartCoroutine("FallBlockCoroutine");
    }
    IEnumerator FallBlockCoroutine()
    {
        int CountColumn = 1;
        while (CountColumn <= GameSize)
        {
           StartCoroutine(FallColumnCoroutine(CountColumn));
            yield return null;
            CountColumn++;
        }
   //     Debug.Log("処理終了");

        yield return null;
    }

    //補充処理
    public void SupplyBlock(int Column)
    {
        StartCoroutine(SupplyBlockCoroutine(Column));
    }
    IEnumerator SupplyBlockCoroutine(int Column)
    {

        //その行で、足りていないブロックの数を数えて発生させる
        int CountLine = 1;
        int CountNull = 0;
        int CountWhile = 0;
        int StartLine = 0;
        string FindTagC;
        string FindTagL;
        string FindTagName;

        while (CountLine <= GameSize)
        {
            FindTagC = Column.ToString();
            FindTagL = CountLine.ToString();
            FindTagName = "Block" + FindTagC + "-" + FindTagL;
            //            Debug.Log(CountLine + FindTagName);
            if (GameObject.FindGameObjectWithTag(FindTagName) == null)
            { CountNull++; }
            CountLine++;
        }
        StartLine = GameSize-CountNull+1;
        while (CountWhile < CountNull)
        {
            DecideBlock(Column, StartLine);
            StartLine++;
            CountWhile++;
        }
        yield return null;
    }



}
