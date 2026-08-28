using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class RankItem : MonoBehaviour
{
    [SerializeField]
    private TMP_Text rankText;

    [SerializeField]
    private TMP_Text pointText;

    [SerializeField]
    private TMP_Text dateText;

    public void SetData(RankData data)
    {
        rankText.text = data.rank.ToString();

        pointText.text = data.point.ToString();

        if (DateTime.TryParse(
            data.dateTime,
            out DateTime dateTime))
        {
            dateText.text = dateTime.ToString("yyyy-MM-dd");
        }
        else
        {
            dateText.text = "-";
        }

        gameObject.SetActive(true);
    }

    public void Clear()
    {
        gameObject.SetActive(false);
    }
}
