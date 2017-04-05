using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class CSVDataReader : MonoBehaviour
{

    //敵データ格納用の配列データ(とりあえず初期値はnull値)
    public string[,] stageMapDatas = null;

    //読み込めたか確認の表示用の変数
    public int height = 0;    //行数
    public int width = 0;    //列数

    //CSVデータ読み込み関数
    //引数：データパス
    public string[,] readCSVData(string path)
    {
        //返り値の２次元配列
        string[,] readToIntData;

        //ストリームリーダーsrに読み込む
        //※Application.dataPathはプロジェクトデータのAssetフォルダまでのアクセスパスのこと,
        StreamReader sr = new StreamReader(Application.dataPath + path);
        //ストリームリーダーをstringに変換
        string strStream = sr.ReadToEnd();

    //    Debug.Log(strStream);


        //StringSplitOptionを設定(要はカンマとカンマに何もなかったら格納しないことにする)
        System.StringSplitOptions option = StringSplitOptions.RemoveEmptyEntries;

        //行に分ける
        string[] lines = strStream.Split(new char[] { '\r', '\n' }, option);

        //カンマ分けの準備(区分けする文字を設定する)
        char[] spliter = new char[1] { ',' };

        //行数設定
        int heightLength = lines.Length;
        //列数設定
        int widthLength = lines[0].Split(spliter, option).Length;

        //返り値の2次元配列の要素数を設定
        readToIntData = new string[heightLength, widthLength];

        //カンマ分けをしてデータを完全分割
        for (int i = 0; i < heightLength; i++)
        {
            for (int j = 0; j < widthLength; j++)
            {
                //カンマ分け
      //          Debug.Log(lines[i]);

                string[] readStrData = lines[i].Split(spliter, option);
                //型変換（…はせずにstring型の配列へ）
                readToIntData[i, j] = readStrData[j];
            }
        }

        //確認表示用の変数(行数、列数)を格納する
        this.height = heightLength;    //行数   
        this.width = widthLength;     //列数

        //返り値
        return readToIntData;
    }

    //確認表示用の関数
    //引数：2次元配列データ,行数,列数
    public void WriteMapDatas(string[,] arrays, int hgt, int wid)
    {
        for (int i = 0; i < hgt; i++)
        {

            for (int j = 0; j < wid; j++)
            {
                //行番号-列番号:データ値 と表示される
                Debug.Log(i + "-" + j + ":" + arrays[i, j]);
            }
        }
    }

    void Start()
    {
        /*
        //データパスを設定
        //このデータパスは、Assetフォルダ以下の位置を書くので/で階層を区切り、CSVデータ名まで書かないと読み込んでくれない
        string path = "/CsvData/DataCustomer.csv";
        //データを読み込む(引数：データパス)
        this.stageMapDatas = readCSVData(path);

        //WriteMapDatas(this.stageMapDatas, this.height, this.width);
        */
    }

    void UpDate()
    {

    }
}
