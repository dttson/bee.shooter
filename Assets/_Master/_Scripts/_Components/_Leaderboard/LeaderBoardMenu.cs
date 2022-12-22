using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderBoardMenu : MonoBehaviour
{
    [SerializeField] private Transform m_LeaderboardContainer;
    
    public void onClose()
    {
        gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitUntil(() => FirebaseManager.Instance.CurrentUserData != null);

        FirebaseManager.Instance.loadLeaderboard(leaderDatas =>
        {
            foreach (LeaderboardUserData data in leaderDatas)
            {
                var leaderBoardItem =
                    LeaderboardItem.createItem(data, data.id == FirebaseManager.Instance.CurrentUserData.id);
                leaderBoardItem.transform.SetParent(m_LeaderboardContainer, false);
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
    }
}