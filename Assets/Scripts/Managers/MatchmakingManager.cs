using APIs;
using Firebase.Database;
using Serializables;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{

    public class MatchmakingManager : MonoSingleton<MatchmakingManager>
    {
        //public mState state;

        //public bool match
        private KeyValuePair<DatabaseReference, EventHandler<ValueChangedEventArgs>> stateListener;
        private KeyValuePair<DatabaseReference, EventHandler<ValueChangedEventArgs>> queueListener;

        public List<MatchmakingList> matchmakingLists;

        //public void JoinQueue(string playerId, Action<string> onGameFound, Action<AggregateException> fallback) =>
        //    // We post the placeholder first..
        //    DatabaseAPI.PostObject($"matchmaking/{playerId}", "placeholder",
        //        // We listen for the placeholder value changing afterwards...
        //        () => queueListener = DatabaseAPI.ListenForValueChanged($"matchmaking/{playerId}",
        //            args =>
        //            {
        //                // This code gets once the placeholder value is changed
        //                var gameId =
        //                    StringSerializationAPI.Deserialize(typeof(string), args.Snapshot.GetRawJsonValue()) as
        //                        string;
        //                if (gameId == "placeholder") return;
        //                LeaveQueue(playerId, () => onGameFound(
        //                    gameId), fallback);
        //            }, fallback), fallback);
        //"{\"state\": \"wait\"}"
        public void JoinMatchmaking(string playerId, Action<string> onGameFound, Action<AggregateException> fallback) =>
 
        DatabaseAPI.PushObject($"matchmaking/{playerId}",new MatchmakingState { state = mState.wait} ,ListenJoin,
            // We listen for the placeholder value changing afterwards...
            //() => 
            fallback);

        public void ListenJoin()
        {
            stateListener = DatabaseAPI.ListenForValueChanged($"matchmaking/{Auth.Instance.currentUser.userUID}",
                args =>
                { 
                    var gameId = args.Snapshot.GetRawJsonValue();
                     
                    Debug.Log(" me -> " + gameId);
 
                }, null);
            queueListener = DatabaseAPI.ListenForValueChanged($"matchmaking/",
                args =>
                {
                    // This code gets once the placeholder value is changed
                    var gameId = args.Snapshot.GetRawJsonValue();
                     
                    Debug.Log(" que -> " + gameId);

                    DataSnapshot snapshot = args.Snapshot;
                    Debug.Log(snapshot.ChildrenCount);
                    Debug.Log(snapshot.Key);

                    matchmakingLists = new List<MatchmakingList>();
                    foreach (DataSnapshot snapshotChild in snapshot.Children)
                    { 
                        MatchmakingList obj_ = new MatchmakingList();
                        obj_.userUID = snapshotChild.Key;
                        var child = snapshotChild.GetRawJsonValue();
                        
                        Debug.Log(child);

                        var obj1 = new MatchmakingState();
                        foreach (var item in snapshotChild.Children)
                        {
                            obj1 = JsonUtility.FromJson<MatchmakingState>(item.GetRawJsonValue()); 
                        }
                        obj_.state = obj1; 
                        matchmakingLists.Add(obj_);
                        //Debug.Log(snapshotChild.Children.ToString());

                        //try
                        //{
                        //    obj_ = JsonUtility.FromJson<T>(child);
                        //    liste.Add(obj_);
                        //}
                        //catch (Exception)
                        //{

                        //    throw;
                        //}
                    }

                }, null);

        }

        public void LeaveListeners()
        {
            DatabaseAPI.StopListeningForValueChanged(queueListener);
            DatabaseAPI.StopListeningForValueChanged(stateListener);
            DatabaseAPI.PostJSON($"matchmaking/" + Auth.Instance.currentUser.userUID, "null", null,null);


        }

        public void LeaveQueue(string playerId, Action callback, Action<AggregateException> fallback)
        {
            DatabaseAPI.StopListeningForValueChanged(queueListener);
            DatabaseAPI.PostJSON($"matchmaking/{playerId}", "null", callback, fallback);
        }
    }
}