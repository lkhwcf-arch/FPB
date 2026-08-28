using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{

    string name = "bird";

    bool isDead;
    int Score;



    // 사망시 처리 
    public void Dead()
    {
        if (isDead) return;

        isDead = true;
        RankManager.Instance.AddRank(name, Score);
    }
    public void AddScore()
    {
        if (isDead) return;
        
        Score++;
    }
}
