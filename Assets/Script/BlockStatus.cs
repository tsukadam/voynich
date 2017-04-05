using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class BlockStatus : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler
//ブロックの種類を保持する
//ブロックへの操作を受けた時は、ここを起点にPuzzleCOntrollerを呼び出す
//cloneして使う
{

    //宣言
    public string Name = "name";//名前、ブロックの種類を定義
    public string Image = "image";//イメージ名、ブロックの画像を定義
    public int Column = 0;//列
    public int Line = 0;//行
    public int DestroyFlag = 0;//消去フラグ

    // Update is called once per frame
    void Update()
    {

    }

    //ドラッグ処理関連オブジェクト・宣言
    public GameObject DraggingBlock;
    public GameObject DraggedBlock;
    public Vector3 ScreenPoint;
    public Vector3 MousePoint;
    public Vector3 Offset;
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




 
    public void OnMouseDown()
    {
        if (DraggingBlock != null) return;//非ドラッグ中のみ判定
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
        if (hit){
            DraggedBlock = hit.collider.gameObject;                     
        }
        GameObject PuzzleController;
        PuzzleController = GameObject.FindGameObjectWithTag("PuzzleController");
        PuzzleController Scripter = PuzzleController.GetComponent<PuzzleController>();

        StartCoroutine(Scripter.OnEndDragCoroutine(
            pointerEventData, 
            DraggingBlock,
            DraggedBlock,
            StartColumn,
            StartLine,
            NowOnColumn,
            NowOnLine,
            PrevOnColumn,
            PrevOnLine,
            EndColumn,
            EndLine,
            ScreenPoint,
            MousePoint,
            Offset,
            DraggingColumn,
            DraggingLine,
            hit
        ));
       }

}