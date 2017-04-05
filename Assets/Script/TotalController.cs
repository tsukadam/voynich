using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TotalController : MonoBehaviour
//パズル以外の全てを制御する

{

    //各種画面
    public GameObject Menu;
    public GameObject Gacha;
    public GameObject Zukan;
    public GameObject ZukanPages;
    public GameObject Shop;
    public GameObject Puzzle;
    public GameObject Status;

    public GameObject GachaPages;
    public GameObject GachaPagesName;
    public GameObject GachaPagesGriff;
    public GameObject GachaPagesDescription;

    public GameObject PopUp;
    public GameObject PopUpText;
    public GameObject PopUpOK;


    //タップ切るための板
    public GameObject TapBlock;

    //ステータス保持オブジェクト
    public GameObject PlayerStatus;
    public GameObject GameStatus;

    //表示UI
    public GameObject GoldUI;
    public GameObject SuperGoldUI;

    // Use this for initialization
    //初期化作業
    void Start () {
        //Status以外は、同時には１つしかtrueにならない
        Menu.SetActive(true);

        Gacha.SetActive(false);
        Zukan.SetActive(false);
        ZukanPages.SetActive(false);
        Shop.SetActive(false);
        Puzzle.SetActive(false);
//StatusはPuzzle画面以外で表示
        Status.SetActive(true);
    }


    //ゴールド加算・表示
    public void PlusGold(int Amount)
    {
        PlayerStatus.GetComponent<PlayerStatus>().Gold += Amount;
        GoldUI.GetComponent<Text>().text = PlayerStatus.GetComponent<PlayerStatus>().Gold.ToString();
    }

    //スーパーゴールド加算・表示
    public void PlusSuperGold(int Amount)
    {
        PlayerStatus.GetComponent<PlayerStatus>().SuperGold += Amount;
        SuperGoldUI.GetComponent<Text>().text = PlayerStatus.GetComponent<PlayerStatus>().SuperGold.ToString();
    }



    //ガチャ

    //仮のレアリティ当選率
    //C 40
    //UC 30
    //R 20
    //SR 9
    //SSR 1

    public void NormalGacha()
    {
        StartCoroutine("NormalGachaCoroutine");
    }
    public IEnumerator NormalGachaCoroutine()
    {

        //仮のデータプール
        string[,] AllCard = new string[,]{
        { "0", "謎","ダミー","enigma","C"},
        { "1","ジョン・ディー","説明","jhon","SR"},
        { "2","植物","せつめい","kusa","U"},
        { "3","風呂","セツメイ","bath","R"},
        { "4","ビン","だみー","bin","UC"},
        { "5", "謎２","ダミー2","enigma2","C"}
    };
//配列をリストに変える
        int AllLength0 = AllCard.GetLength(0);
        int AllLength1 = AllCard.GetLength(1);
        List<string> PoolList = new List<string>();
        List<List<string>> AllList = new List<List<string>>();

        for (int i=0;i< AllLength0; i++)
        {
            PoolList = new List<string>();
            for(int m = 0; m < AllLength1; m++)
            {
                PoolList.Add(AllCard[i,m]);
            }
            AllList.Add(PoolList);
        }
        int AllListCount = AllList.Count;
        int PoolListCount = PoolList.Count;

        //ゴールド消費量定義
        int NormalPrice = 100;
        string KeyRarerity;
        if (PlayerStatus.GetComponent<PlayerStatus>().Gold >= NormalPrice) {
            //ゴールド消費
            PlusGold(-NormalPrice);
            int DecideRare = Random.Range(1, 101);
            if (DecideRare >= 1& DecideRare <= 40) { KeyRarerity = "C"; }
            else if (DecideRare > 40 & DecideRare <= 70) { KeyRarerity = "UC"; }
            else if (DecideRare > 70 & DecideRare <= 90) { KeyRarerity = "R"; }
            else if (DecideRare > 90 & DecideRare <= 99) { KeyRarerity = "SR"; }
            else { KeyRarerity = "SSR"; }

            List<string> ThisRarePoolList = new List<string>();
            List<List<string>> ThisRareAllList = new List<List<string>>();
            //オールカードをforにかけて特定のレアリティカードのみを抜き出しリストにする
            for (int i = 0; i < AllListCount; i++)
            {
                if (AllList[i][4] == KeyRarerity) {
                    ThisRarePoolList = new List<string>();
                        for (int m = 0; m < PoolListCount; m++)
                        {
                                ThisRarePoolList.Add(AllList[i][m]);
                    }
                    ThisRareAllList.Add(ThisRarePoolList);
                }
            }
            int ThisRareAllListCount = ThisRareAllList.Count;

            //その配列の中からランダムで決定
            int DecideCard = Random.Range(0, ThisRareAllListCount);
            string StringGetID = ThisRareAllList[DecideCard][0];
            int GetID = int.Parse(StringGetID);
            //    Debug.Log("Decide:"+ ThisRareAllListCount+"個の中から"+DecideCard + "番目 それのID:"+GetID);
            Debug.Log("Rare:"+KeyRarerity+" ID:"+GetID);

            //入手処理
            int GotCardLength = PlayerStatus.GetComponent<PlayerStatus>().GotCard.Count;
            List<string> PlayerGotCard = PlayerStatus.GetComponent<PlayerStatus>().GotCard;
            int GotFlag = 0;//既に持っているフラッグ
            for (int i = 0; i < GotCardLength; i++)
            {
         //       Debug.Log("Got:"+PlayerGotCard[i]);
                if (PlayerGotCard[i] ==StringGetID)
                {
                    GotFlag = 1;
                }
            }
            if (GotFlag == 1) {
                    GachaPagesName.GetComponent<Text>().text= ThisRareAllList[DecideCard][1];
                GachaPagesGriff.GetComponent<Text>().text = ThisRareAllList[DecideCard][3];
                GachaPagesDescription.GetComponent<Text>().text = ThisRareAllList[DecideCard][1]+"を入手した\n既に持っている";
                GachaPages.SetActive(true);
            }
            else
            {            //重複していない時だけ入手処理

                PlayerStatus.GetComponent<PlayerStatus>().GotCard.Add(StringGetID);
                    GachaPagesName.GetComponent<Text>().text= ThisRareAllList[DecideCard][1];
                GachaPagesGriff.GetComponent<Text>().text = ThisRareAllList[DecideCard][3];
                GachaPagesDescription.GetComponent<Text>().text = ThisRareAllList[DecideCard][1]+"を入手した";
                GachaPages.SetActive(true);
            }

        }
        else
        {
            //ゴールドが足りない旨表示
            PopUpOpen("お金が足りない。\n1000ゴールドめぐんであげよう");
            PlusGold(1000);
        }

        yield return null;
    }

    //図鑑
    //全カードのboxを作る boxはid、gotかどうか、アイコン画像の情報を持つ
    //boxのgot=0なら？ボックス
    //got=1なら各カードの画像
    //ボックスを押すとそのidの情報ポップアップを開く


    //汎用ポップアップ
    public void PopUpOpen(string Text)
    {
        PopUp.SetActive(true);
        PopUpText.GetComponent<Text>().text=Text;
        TapBlock.SetActive(true);
    }

    //ボタン用のタップブロック操作関数
    public void TapBlockFalse()
    {
        TapBlock.SetActive(false);
    }


    //画面遷移
    public void OnOffMenu(bool OnOff)
    {
        Menu.SetActive(OnOff);
    }
    public void OnOffGacha(bool OnOff)
    {
        Gacha.SetActive(OnOff);
    }
    public void OnOffZukan(bool OnOff)
    {
        Zukan.SetActive(OnOff);
    }
    public void OnOffZukanPages(bool OnOff)
    {
        ZukanPages.SetActive(OnOff);
    }
    public void OnOffPuzzle(bool OnOff)
    {
        Puzzle.SetActive(OnOff);
    }
    public void OnOffStatus(bool OnOff)
    {
        Status.SetActive(OnOff);
    }
    public void OnOffShop(bool OnOff)
    {
        Shop.SetActive(OnOff);
    }



    // Update is called once per frame
    void Update () {
		
	}
}
