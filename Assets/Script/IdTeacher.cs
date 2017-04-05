using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class IdTeacher : MonoBehaviour
{
    //図鑑アイコンが押されたとき、そのアイコンが持つidをTotalControllerに教えるためのスクリプト
    public GameObject MyselfText;
    public GameObject TotalController;

    public void IdTeach()
    {
        TotalController Scripter = TotalController.GetComponent<TotalController>();
        string MyId = MyselfText.GetComponent<Text>().text;
        Scripter.ZukanPagesOpen(MyId);
    }
}
