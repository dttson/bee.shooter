using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using System.Linq;
using UnityEngine.Events;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }
    public bool IsInitialized { get; private set; } = false;
    
    //Firebase variables
    [Header("Firebase")] public DependencyStatus dependencyStatus;
    public DatabaseReference dBreference;

    public string m_UserId = "test-user";

    public LeaderboardUserData CurrentUserData { get; private set; } = default;
    public List<LeaderboardUserData> LeaderboardData { get; private set; } = new List<LeaderboardUserData>();

    void Awake()
    {
        Instance = this;
        
        initializeUserInfo();

        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => dBreference != null);

        loadUserData();
    }

    private void initializeUserInfo()
    {
        //TODO: get user id from game center
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        dBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void loadUserData()
    {
        if (string.IsNullOrEmpty(m_UserId))
        {
            Debug.LogError("User id is invalid!");
            return;
        }

        IEnumerator coroutineLoadUserData()
        {
            //Set the currently logged in user xp
            var DBTask = dBreference.Child("users").Child(m_UserId).GetValueAsync();
            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            }
            else if (DBTask.Result.Value == null)
            {
                //No data exists yet
                CurrentUserData = new LeaderboardUserData {id = m_UserId, name = m_UserId, score = 0};
                updateUserData();
            }
            else
            {
                //Data has been retrieved
                DataSnapshot snapshot = DBTask.Result;

                string username = snapshot.Child("name").Value.ToString();
                int score = int.Parse(snapshot.Child("score").Value.ToString());
                CurrentUserData = new LeaderboardUserData {id = m_UserId, name = username, score = score};
            }
        }

        StartCoroutine(coroutineLoadUserData());
    }

    private void updateUserData()
    {
        if (CurrentUserData.Equals(default(LeaderboardUserData)))
        {
            Debug.LogError("Cannot update user data");
            return;
        }

        IEnumerator coroutineUpdateUserData()
        {
            // Get raw JSON of current user data
            var jsonData = JsonUtility.ToJson(CurrentUserData);
            var DBTask = dBreference.Child("users").Child(m_UserId).SetRawJsonValueAsync(jsonData);
            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

            if (DBTask.Exception != null)
            {
                Debug.LogError(message: $"Failed to update user data {DBTask.Exception}");
            }
            else
            {
                Debug.Log(message: "User data is updated successfully");
            }
        }

        StartCoroutine(coroutineUpdateUserData());
    }

    public void updateScore(int score)
    {
        if (string.IsNullOrEmpty(m_UserId))
        {
            Debug.LogError("User id is invalid!");
            return;
        }

        IEnumerator coroutineGetScore()
        {
            //Set the currently logged in user xp
            var DBTask = dBreference.Child("users").Child(m_UserId).Child("score").SetValueAsync(score);
            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

            if (DBTask.Exception != null)
            {
                Debug.LogError(message: $"Failed to register task with {DBTask.Exception}");
            }
            else
            {
                //Xp is now updated
            }
        }

        StartCoroutine(coroutineGetScore());
    }

    public void loadLeaderboard(UnityAction<List<LeaderboardUserData>> onFinish)
    {
        if (string.IsNullOrEmpty(m_UserId))
        {
            Debug.LogError("User id is invalid!");
            onFinish?.Invoke(null);
            return;
        }

        IEnumerator coroutineLoadLeaderboard()
        {
            //Get all the users data ordered by kills amount
            var DBTask = dBreference.Child("users").OrderByChild("score").GetValueAsync();

            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

            if (DBTask.Exception != null)
            {
                Debug.LogError(message: $"Failed to register task with {DBTask.Exception}");
                onFinish?.Invoke(null);
            }
            else
            {
                //Data has been retrieved
                DataSnapshot snapshot = DBTask.Result;

                var listUserData = new List<LeaderboardUserData>();
                int rank = 1;
                //Loop through every users UID
                foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
                {
                    string userId = childSnapshot.Child("id").Value.ToString();
                    string username = childSnapshot.Child("name").Value.ToString();
                    int score = int.Parse(childSnapshot.Child("score").Value.ToString());

                    listUserData.Add(new LeaderboardUserData
                        {id = userId, name = username, score = score, rank = rank});
                    rank++;
                }

                LeaderboardData = listUserData;

                onFinish?.Invoke(listUserData);
            }
        }

        StartCoroutine(coroutineLoadLeaderboard());
    }
}