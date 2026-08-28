using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class RankData
{
    public string name;
    public int rank;
    public int point;
    public string dateTime;

    public RankData(string _name,int _point, DateTime dateTime)
    {
        this.name = _name;
        this.rank = 0;
        this.point = _point;
        this.dateTime = dateTime.ToString("yyyy-MM-ddTHH:mm:ss");
    }
}