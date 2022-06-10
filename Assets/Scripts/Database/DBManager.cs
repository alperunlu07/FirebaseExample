using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

namespace DB
{


    public class DBManager : MonoSingleton<DBManager>
    {
        //FirebaseDatabase database = FirebaseDatabase.DefaultInstance;
        private DatabaseReference database;// = FirebaseDatabase.DefaultInstance.RootReference;
        int userId = 0;

        private void Awake()
        {
            database = FirebaseDatabase.DefaultInstance.RootReference;
        }
        void Start()
        {
            //DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
            //database.GetReference("test").ValueChanged += HandleValueChanged;

            FirebaseDatabase.DefaultInstance
               .GetReference("users")
               .ValueChanged += HandleValueChanged;
        
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                //database.Child("users").Child("1").Child("username").SetValueAsync(name);
                writeNewUser(userId++.ToString(), "TestName", "test@test.com");
                //database.RootReference("test")
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                //database.Child("users").Child("1").Child("username").SetValueAsync(name);
                WriteNewScore(userId++.ToString(), Random.Range(0,100));
                //database.RootReference("test")
            }
        }

        public void SendUserInfo()
        {

        }
        void HandleValueChanged(object sender, ValueChangedEventArgs args)
        { 
            if (args.DatabaseError == null)
            {
                Debug.Log(args.ToString());
            }
            Debug.Log(args.Snapshot);

        }

        private void writeNewUser(string userId, string name, string email)
        {
            User user = new User(name, email);
            string json = JsonUtility.ToJson(user);

            database.Child("users").Child(userId).SetRawJsonValueAsync(json);
        }
        private void WriteNewScore(string userId, int score)
        {
            // Create new entry at /user-scores/$userid/$scoreid and at
            // /leaderboard/$scoreid simultaneously
            string key = database.Child("scores").Push().Key;
            LeaderBoardEntry entry = new LeaderBoardEntry(userId, score);
            Dictionary<string, System.Object> entryValues = entry.ToDictionary();

            Dictionary<string, System.Object> childUpdates = new Dictionary<string, System.Object>(); 
            childUpdates["/scores/" + key] = entryValues;

            childUpdates["/user-scores/" + userId + "/" + key] = entryValues;

            database.UpdateChildrenAsync(childUpdates);
        }
    }
    [System.Serializable]
    public class User
    {
        public string username;
        public string email;

        public User()
        {
        }

        public User(string username, string email)
        {
            this.username = username;
            this.email = email;
        }
    }

    public class LeaderBoardEntry
    {
        public string uid;
        public int score = 0;

        public LeaderBoardEntry()
        {
        }

        public LeaderBoardEntry(string uid, int score)
        {
            this.uid = uid;
            this.score = score;
        }

        public Dictionary<string, System.Object> ToDictionary()
        {
            Dictionary<string, System.Object> result = new Dictionary<string, System.Object>(); 

            result["uid"] = uid;
            result["score"] = score;

            return result;
        }
    }
}