using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;


public class ButtonHider : MonoBehaviour {
    //ボタンが自分自身を消すためのスクリプト
    public GameObject Myself;

    public void SelfHide()
    {
        Myself.SetActive(false);

    }

}
