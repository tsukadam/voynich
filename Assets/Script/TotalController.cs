using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TotalController : MonoBehaviour
//パズル以外の全てを制御する

{

    //自分自身
    public GameObject Myself;

    //各種画面
    public GameObject Menu;
    public GameObject Gacha;
    public GameObject Zukan;

    public GameObject Shop;
    public GameObject Puzzle;
    public GameObject Status;

    public GameObject GachaPages;
    public GameObject GachaPagesName;
    public GameObject GachaPagesGriff;
    public GameObject GachaPagesDescription;

    public GameObject ZukanIconPrefab;
    public GameObject ZukanIconSpace;
    public int ZukanColumnCount = 5;//図鑑アイコンの一行の表示数

    public GameObject ZukanPages;
    public GameObject ZukanPagesName;
    public GameObject ZukanPagesGriff;
    public GameObject ZukanPagesDescription;

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

    //単語データ処理用、Startで中身を入れる
    public int AllListCount;
    public int PoolListCount;
    public string[,] AllCard;
    public  List<string> PoolList;
    public List<List<string>> AllList;
    public string[] Words;

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

        //単語データを読み込み
        CSVDataReader CSVScripter = Myself.GetComponent<CSVDataReader>();
        string path = "/CSVData/CSVWords.csv";
        AllCard = CSVScripter.readCSVData(path);
        PlayerStatus.GetComponent<PlayerStatus>().AllCard = AllCard;

        //配列をリストに変える
        int AllLength0 = AllCard.GetLength(0);
        int AllLength1 = AllCard.GetLength(1);
        PoolList = new List<string>();
        AllList = new List<List<string>>();

        for (int i = 0; i < AllLength0; i++)
        {
            PoolList = new List<string>();
            for (int m = 0; m < AllLength1; m++)
            {
                PoolList.Add(AllCard[i, m]);
            //    Debug.Log(AllCard[i, m]);
            }
            AllList.Add(PoolList);
        }
        AllListCount = AllList.Count;
        PoolListCount = PoolList.Count;


        //単語リスト
        int AllWordsLength0 = AllCard.GetLength(0);
        int AllWordsLength1 = AllCard.GetLength(1);
        string[] Words = new string[AllWordsLength0];

        for (int i = 0; i < AllWordsLength0; i++)
        {
            Words[i] = AllCard[i, 3];
        }
        PlayerStatus.GetComponent<PlayerStatus>().Words = Words;

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
            PopUpOpen("お金が足りない。\n3000ゴールドめぐんであげよう");
            PlusGold(3000);
        }

        yield return null;
    }
    //図鑑個別ページ開いた時
    public void ZukanPagesOpen(string KeyId)
    {

        List<string> KeyCard = new List<string>();

        //オールカードをforにかけてKyeIDのカードのみを抜き出す
        for (int i = 0; i < AllListCount; i++)
        {
            if (AllList[i][0] == KeyId)
            {
                KeyCard = new List<string>();
                for (int m = 0; m < PoolListCount; m++)
                {
                    KeyCard.Add(AllList[i][m]);
                }
            }
        }


        ZukanPagesName.GetComponent<Text>().text = KeyCard[1];
        ZukanPagesGriff.GetComponent<Text>().text = KeyCard[3];
        ZukanPagesDescription.GetComponent<Text>().text = KeyCard[2];
        ZukanPages.SetActive(true);
}

    //デバッグ用
    //手持ちに6体加える
    public void ZukanDebugGet()
    {
        string StringGetID;
        for (int i = 0; i < 6; i++)
        {
            StringGetID = i.ToString();
            PlayerStatus.GetComponent<PlayerStatus>().GotCard.Add(StringGetID);
        }
    }
    //図鑑
    //全カードのboxを作る boxはid、gotかどうか、アイコン画像の情報を持つ
    //boxのgot=0なら？ボックス
    //got=1なら各カードの画像
    //ボックスを押すとそのidの情報ポップアップを開く

    public void ZukanIconMake(int Column,int Line,string id)
    {
        Debug.Log("MakeIcon");

        GameObject NewIcon = (GameObject)Instantiate(
                  ZukanIconPrefab,
                  transform.position,
                  Quaternion.identity);
        NewIcon.transform.SetParent(ZukanIconSpace.transform, false);

        float PositionColumn = GetIconColumn(Column);
        float PositionLine = GetIconLine(Line);
        NewIcon.transform.position = new Vector3(PositionColumn, PositionLine, -7);
        GameObject IdName= NewIcon.transform.FindChild("Text").gameObject;
        IdName.GetComponent<Text>().text =id;
        NewIcon.tag = "ZukanIcon";
        NewIcon.SetActive(true);
    }

    //図鑑アイコンを全て消す
    public void ZukanIconAllDestroy()
    {
        StartCoroutine("ZukanIconAllDestroyCoroutine");
    }
    public IEnumerator ZukanIconAllDestroyCoroutine()
    {
        GameObject[] IconToDestroy = GameObject.FindGameObjectsWithTag("ZukanIcon");
foreach(GameObject obs in IconToDestroy)
        {
            Destroy(obs);
            Debug.Log("Destroy");
        }
        yield return null;
    }
    //図鑑を開いた時
    public void ZukanOpen()
    {
        StartCoroutine("ZukanOpenCoroutine");
    }
    public IEnumerator ZukanOpenCoroutine()
    {
        yield return StartCoroutine("ZukanIconAllDestroyCoroutine");

        int IconCount = PlayerStatus.GetComponent<PlayerStatus>().GotCard.Count;
        if (IconCount != 0)
        {
            List<string> PlayerGotCard = PlayerStatus.GetComponent<PlayerStatus>().GotCard;
            //あとでレアリティごとにソートしたい
            int ColumnCount = ZukanColumnCount;
//            Debug.Log("Icon:" + IconCount);
            float FloatIconCount = (float)IconCount;
            float FloatColumnCount = (float)ColumnCount;
            float FloatLineCount = FloatIconCount / FloatColumnCount;
            int LineCount = Mathf.CeilToInt(FloatLineCount);
//            Debug.Log("Line:"+LineCount);
            int IdNo = 0;
            int i = 0;
            while (i < LineCount&IdNo<IconCount)
                {
                int m = 0;
                while (m < ColumnCount & IdNo < IconCount)
                {
                    string IconId = PlayerGotCard[IdNo];
                    ZukanIconMake(m, i, IconId);

                    m++;
                    IdNo++;
                }
                i++;
            }
        }
        yield return null;
    }

    //図鑑アイコンのClumn位置指定
    public float GetIconColumn(int Column)
    {
        float FloatColumn = (float)Column;
        float PositionColumn = (FloatColumn*4/3 ) - (7 / 2);
        return PositionColumn;
    }
    //図鑑アイコンのLine位置指定
    public float GetIconLine(int Line)
    {
        float FloatLine = (float)Line;
        float PositionLine = -(FloatLine*3/2 ) - (7 / 2)+(73/10);
        return PositionLine;
    }

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
