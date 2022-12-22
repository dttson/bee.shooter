using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardItem : MonoBehaviour
{
    [SerializeField] private Image m_Icon;
    [SerializeField] private TMP_Text m_TextOrderTop3;
    [SerializeField] private TMP_Text m_TextOrder;
    [SerializeField] private TMP_Text m_TextName;
    [SerializeField] private Text m_TextScore;
    [SerializeField] private Image m_Background;
    [SerializeField] private Sprite m_NormalRowBackground;
    [SerializeField] private Sprite m_SelfRowBackground;

    private static readonly Color[] TOP3_COLORS =
    {
        new Color(251f/255, 209f/255, 1f/255),
        new Color(74f/255, 245f/255, 255f/255),
        new Color(248f/255, 131f/255, 2f/255),
    };

    public static LeaderboardItem createItem(LeaderboardUserData data, bool isSelfItem)
    {
        if (data == null)
        {
            return null;
        }

        var item = Instantiate(Resources.Load<LeaderboardItem>("_UI/LeaderboardItem"));
        if (isSelfItem)
        {
            item.m_Background.sprite = item.m_SelfRowBackground;
            
            var size =((RectTransform) item.transform).sizeDelta;
            size.y = 100f;
            ((RectTransform) item.transform).sizeDelta = size;
        }
        else
        {
            item.m_Background.sprite = item.m_NormalRowBackground;
        }
        
        if (data.rank < 4)
        {
            item.m_Icon.gameObject.SetActive(true);
            item.m_Icon.color = TOP3_COLORS[data.rank - 1];
            item.m_TextOrderTop3.gameObject.SetActive(true);
            item.m_TextOrder.gameObject.SetActive(false);
            item.m_TextOrderTop3.text = data.rank.ToString();
        }
        else
        {
            item.m_Icon.gameObject.SetActive(false);
            item.m_TextOrderTop3.gameObject.SetActive(false);
            item.m_TextOrder.gameObject.SetActive(true);
            item.m_TextOrder.text = data.rank.ToString();
        }

        item.m_TextName.text = data.name;
        item.m_TextScore.text = data.score.ToString();
        return item;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}