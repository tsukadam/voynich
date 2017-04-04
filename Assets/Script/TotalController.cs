using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;


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


    //ガチャ
    //仮のデータプール
    string[,] AllCard = new string[,]{
        { "0", "謎","ダミー","enigma","N"},
        { "1","ジョン・ディー","説明","jhon","SR"},
        { "2","植物","せつめい","kusa","U"},
        { "3","風呂","セツメイ","bath","R"},
        { "4","ビン","だみー","bin","UC"}
    };
//仮のレアリティ当選率
//C　40
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
        Debug.Log(AllCard[2,2]);
        //トークン消費
        int NormalPrice = 100;
        string KeyRarerity;
        if (PlayerStatus.GetComponent<PlayerStatus>().Gold < NormalPrice) {
            int GetGacha = Random.Range(1, 101);
            if(GetGacha>=1& GetGacha <= 40) { KeyRarerity = "C"; }
            else if (GetGacha > 40 & GetGacha <= 70) { KeyRarerity = "UC"; }
            else if (GetGacha > 70 & GetGacha <= 90) { KeyRarerity = "R"; }
            else if (GetGacha > 90 & GetGacha <= 99) { KeyRarerity = "SR"; }
            else { KeyRarerity = "SSR"; }
            string[,] RaerityCard;
            //オールカードをforにかけて特定のレアリティカードのみを抜き出し配列にする
            //その配列の中からランダムで決定

            PlayerStatus.GetComponent<PlayerStatus>().Gold -= NormalPrice;
        }
        else
        {
            //ゴールドが足りない旨表示

        }

        yield return null;
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
