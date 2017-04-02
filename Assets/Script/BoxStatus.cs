using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class BoxStatus : MonoBehaviour
{
    //ボックスごとのステータスを保持する
    //cloneして使う

    //ステータスの定義
    public int Column = 0;//列
    public int Line = 0;//行

    //ステータス表示オブジェクトの取得
    public GameObject Box;//自分自身
    public GameObject Entered;

    // Update is called once per frame
    void Update()
    {

    }

}