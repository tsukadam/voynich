using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlockStatus : MonoBehaviour
{
    //ブロックごとのステータスを保持する
    //cloneして使う

    //ステータスの定義
    public string Name = "name";//名前、ブロックの種類を定義
    public string Image = "image";//イメージ名、ブロックの画像を定義
    public int Column = 0;//列
    public int Line = 0;//行

    //ステータス表示オブジェクトの取得
    public GameObject Block;//自分自身

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
