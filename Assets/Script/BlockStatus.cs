using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class BlockStatus : MonoBehaviour
    ,
        IBeginDragHandler, IDragHandler, IEndDragHandler,
    IDropHandler
{
    //ブロックごとのステータスを保持する
    //cloneして使う

    //ステータスの定義
    public string Name = "name";//名前、ブロックの種類を定義
    public string Image = "image";//イメージ名、ブロックの画像を定義
    public int Column = 0;//列
    public int Line = 0;//行

    public int DestroyFlag = 0;//消去フラグ

    // Update is called once per frame
    void Update()
    {

    }


    //ドラッグ処理
    private Transform CanvasTran;
    private GameObject DraggingBlock;
    private GameObject DraggedBlock;
    private Vector3 ScreenPoint;
    private Vector3 MousePoint;
    private Vector3 Offset;
    public int StartColumn=0;
    public int StartLine=0;
    public int NowOnColumn=0;
    public int NowOnLine=0;
    public int PrevOnColumn=0;
    public int PrevOnLine=0;
    public int EndColumn=0;
    public int EndLine=0;


    public int DraggingColumn = 0;//ドラッグ動作中の列。そこ以外は無反応
    public int DraggingLine = 0;//ドラッグ動作中の行。そこ以外は無反応


    void Awake()
    {
        CanvasTran = transform.parent;
    }

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

 

    public void OnMouseDown()
    {
        Debug.Log("MouseDown");
        // このオブジェクトの位置(transform.position)をスクリーン座標に変換。
        ScreenPoint = Camera.main.WorldToScreenPoint(transform.position);
        //マウスカーソルの位置
        MousePoint= Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, ScreenPoint.z));
        // ワールド座標上の、マウスカーソルと、対象の位置の差分。
        Offset = transform.position - MousePoint;

        //マウスの位置にオブジェクトがあれば、それをドラッグ対象とする
        RaycastHit2D hit = Physics2D.Raycast(MousePoint, new Vector3(0, 0, 10));

        if (hit)
        {
     //       Debug.Log("hit");
            DraggingBlock = hit.collider.gameObject;//ドラッグ開始
        }
    }

    public void OnBeginDrag(PointerEventData pointerEventData)
    {
        if (DraggingBlock == null) return;//ドラッグ中のみ判定

        DraggingBlock.GetComponent<Collider2D>().isTrigger = false;//ドラッグされている側のトリガーは切る
//スタート地点の取得
        StartColumn = DraggingBlock.GetComponent<BlockStatus>().Column;
        StartLine = DraggingBlock.GetComponent<BlockStatus>().Line;
        DraggingColumn = StartColumn;
        DraggingLine = StartLine;
        Debug.Log(StartColumn + "-" + StartLine + ":DragStart");
    }

    public void OnDrag(PointerEventData pointerEventData)
    {
        if (DraggingBlock == null) return;//ドラッグ中のみ判定

        //ドラッグによる移動
        //最終的には、ドラッグの方向を感知して一定だけ動かす

        Vector3 currentScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, ScreenPoint.z);
        Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenPoint) + Offset;
        DraggingBlock.transform.position = currentPosition;
    }

    public void OnEndDrag(PointerEventData pointerEventData)
    {
        if (DraggingBlock == null) return;//ドラッグ中のみ判定

        DraggingBlock.layer = LayerMask.NameToLayer("Ignore Raycast");//ドラッグしているブロックはレイキャストから外す
        //マウスの位置にオブジェクトがあれば、それをドラッグされている対象とする
        MousePoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, ScreenPoint.z));
        RaycastHit2D hit = Physics2D.Raycast(MousePoint, new Vector3(0, 0, 10));
        DraggingBlock.layer = LayerMask.NameToLayer("Default");

        //ヒットがない時、ドラッグされている対象がスタート地点から上下左右の時は、スタート地点をエンド地点とする
        if (!hit)
        {
            EndColumn = StartColumn;
            EndLine = StartLine;
        }
        else {
            DraggedBlock = hit.collider.gameObject;
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
        if (StartColumn == EndColumn& StartLine == EndLine)
        {
            PositionColumn = GetPositionColumn(StartColumn);
            PositionLine = GetPositionLine(StartLine);
            DraggingBlock.transform.position = new Vector3(PositionColumn, PositionLine, -3);
            Debug.Log("Start=End");
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

            //パズルコントローラーの関数を使うので取得
            GameObject PuzzleController;
            PuzzleController = GameObject.FindGameObjectWithTag("PuzzleController");
            PuzzleController Scripter = PuzzleController.GetComponent<PuzzleController>();

            Scripter.MoveBlock(Count,  ColumnOrLine,  IncOrDec, EndPoint, Stage);

            PositionColumn = GetPositionColumn(EndColumn);
            PositionLine = GetPositionLine(EndLine);
            DraggingBlock.transform.position = new Vector3(PositionColumn, PositionLine, -3);
            DraggingBlock.GetComponent<BlockStatus>().Column = EndColumn;
            DraggingBlock.GetComponent<BlockStatus>().Line = EndLine;

            string TagColumn = EndColumn.ToString();
            string TagLine = EndLine.ToString();
            string TagName = "Block" + TagColumn + "-" + TagLine;
            DraggingBlock.tag = TagName;

            //動きが発生している時は消去判定を走らせる
            Scripter.CheckDestroyBlock();
//            Scripter.DestroyBlock();
 //           Scripter.SupplyBlock();
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

    }


    //ドロップ処理
    void Start()
    { }

    public void OnTriggerEnter2D(Collider2D collider)
    {

    }

    public void OnTriggerExit2D(Collider2D collider)
    {
  
    }

    public void OnDrop(PointerEventData pointerEventData)
    {

    }

}