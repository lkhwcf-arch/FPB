using UnityEngine;
using TMPro;
public class RankItem : MonoBehaviour
{
    [SerializeField]
    private TMP_Text rankText;

    [SerializeField]
    private TMP_Text pointText;

    [SerializeField]
    private TMP_Text nicknameText;

    public void SetData(RankData data)
    {
        rankText.text = data.rank.ToString();

        pointText.text = data.point.ToString();
        nicknameText.text = data.name;

        gameObject.SetActive(true);
    }

    public void Clear()
    {
        gameObject.SetActive(false);
    }
}
