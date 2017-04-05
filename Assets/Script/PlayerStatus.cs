using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PlayerStatus : MonoBehaviour
//ゲームプレイを超えたプレイヤー固有のステータスを保持
//セーブするデータはここに記録。
//ここにないデータは何もセーブされない
//図鑑所持キャラや課金関連処理もここ
{
    //宣言
    public int Gold;
    public int SuperGold;
    public int AdBlock;
    public int HightScorePoint1;
    public int HightScorePoint2;
    public int HightScorePoint3;
    public int HightScoreDestroy1;
    public int HightScoreDestroy2;
    public int HightScoreDestroy3;

    //所持カード
    public List<string> GotCard;
    //PoolCard = {"1","2","5","9"}
    //のように所持カードをＩＤで持つ。ぜんぶstringで、処理時にキャスト

    //ガチャカードのデータ形式
    //ID、名前、説明文、綴り、レアリティC,U,R,SR,SSR
    //レアリティはラベル。確率はガチャ部分で決定する
    //まずレアリティ、次にキャラが決まる
    //枚数概念ないので、持っているかもっていないかをIDで引き当てればおｋ

    // Update is called once per frame
    void Update()
    {

    }


}