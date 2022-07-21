using APIs;
using Firebase.Database;
using Handlers;
using Serializables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Managers
{
 
    public class MatchmakingManager : MonoSingleton<MatchmakingManager>
    {
        //public mState state;

        //public bool match
        private KeyValuePair<DatabaseReference, EventHandler<ValueChangedEventArgs>> stateListener;
        private KeyValuePair<DatabaseReference, EventHandler<ValueChangedEventArgs>> queueListener;

        public List<MatchmakingList> matchmakingLists, avalaibleUsrList;

        private string myUID; 

        public MatchmakingState myState;
        public List<string> lastSendingPairReq = new List<string>();

        private float waitTime = 0f;
        public bool waitPairReq = false;
        public bool isMaster = false;
        //public MatchmakingSceneHandler handlers;

        private void Awake()
        {
            //var str = StringSerializationAPI.Serialize(typeof(MState), MState.paired);
            //Debug.Log(str);
            //mState = (MState)StringSerializationAPI.Deserialize(typeof(MState), str);
            //Debug.LogError("");
        }
        //public bool isPaired;
        //public void JoinQueue(string myUID, Action<string> onGameFound, Action<AggregateException> fallback) =>
        //    // We post the placeholder first..
        //    DatabaseAPI.PostObject($"matchmaking/{myUID}", "placeholder",
        //        // We listen for the placeholder value changing afterwards...
        //        () => queueListener = DatabaseAPI.ListenForValueChanged($"matchmaking/{myUID}",
        //            args =>
        //            {
        //                // This code gets once the placeholder value is changed
        //                var gameId =
        //                    StringSerializationAPI.Deserialize(typeof(string), args.Snapshot.GetRawJsonValue()) as
        //                        string;
        //                if (gameId == "placeholder") return;
        //                LeaveQueue(myUID, () => onGameFound(
        //                    gameId), fallback);
        //            }, fallback), fallback);
        //"{\"state\": \"wait\"}"
        public void JoinMatchmaking(string _myUID, Action<string> onGameFound, Action<AggregateException> fallback)
        {
            myUID = _myUID;
            DatabaseAPI.PostObject($"matchmaking/{myUID}", myState, ListenJoin,
                // We listen for the placeholder value changing afterwards...
                //() => 
                fallback);

            StartCoroutine(matchMakingCO());
        }

        public void ListenJoin()
        {
            //Debug.Log("listen Join");

            stateListener = DatabaseAPI.ListenForValueChanged($"matchmaking/{Auth.Instance.currentUser.userUID}",
                args =>
                {
                    var values = args.Snapshot.GetRawJsonValue();
                    var myState_ = (MatchmakingState)StringSerializationAPI.Deserialize(typeof(MatchmakingState), values);
                    if(myState_ != null)
                    {
                        if (myState_.state == MState.wait && myState_.pairUID.Length > 1) // pairing request
                        {
                            myState.pairUID = myState_.pairUID;
                            PairingRequest();
                        }
                        if (myState_.state == MState.paired && myState_.pairUID.Length > 1) // accept response message
                        {
                            isMaster = true; 
                            myState.pairUID = myState_.pairUID;
                            myState.state = MState.paired;
                            MatchmakingSceneHandler.Instance.OpenGameScene();
                        }
                    } 
                    //Debug.Log(" me -> " + values);

                }, null);
            queueListener = DatabaseAPI.ListenForValueChanged($"matchmaking/",
                args =>
                {
                    // This code gets once the placeholder value is changed
                    var gameId = args.Snapshot.GetRawJsonValue();
                     
                    //Debug.Log(" que -> " + gameId);

                    DataSnapshot snapshot = args.Snapshot;
                    //Debug.Log(snapshot.ChildrenCount);
                    //Debug.Log(snapshot.Key);

                    //matchmakingLists = new List<MatchmakingList>();
                    matchmakingLists.Clear();
                    foreach (DataSnapshot snapshotChild in snapshot.Children)
                    {
                        //MatchmakingList obj_ = new MatchmakingList();
                        //obj_.userUID = snapshotChild.Key;
                        var child = snapshotChild.GetRawJsonValue();
                        //Debug.Log()
                        //var std_ = JsonUtility.FromJson<MatchmakingState>(child);
                        MatchmakingState std_ = (MatchmakingState)StringSerializationAPI.Deserialize(typeof(MatchmakingState), child);  
                        var obj_ = new MatchmakingList() { state = std_, userUID = snapshotChild.Key };
                        matchmakingLists.Add(obj_);
                        if(myState.state == MState.search && !waitPairReq)
                        {
                            FindPairObj();
                        }
                    }

                }, null);

        }

      

        private void PairingRequest()
        {
            Debug.Log("Pairing Request from " + myState.pairUID);
            int id = matchmakingLists.FindIndex(x => x.userUID == myState.pairUID);
            if (id > -1)
            {
                AcceptPairReq(matchmakingLists[id]);
                isMaster = false;
            }
            else
            {
                Debug.Log("not exist at matchmakingLists ");
            }
        }
        private void AcceptPairReq(MatchmakingList other)
        {
            other.state.pairUID = myUID;
            other.state.state = MState.paired;
            myState.state = MState.paired; 
            DatabaseAPI.PostObject($"matchmaking/{other.userUID}", other.state, null, null);
            MatchmakingSceneHandler.Instance.OpenGameScene();
        }

        private void FindPairObj()
        {
            avalaibleUsrList = matchmakingLists.FindAll(x => x.userUID != myUID && x.state.state == MState.wait && x.state.pairUID == "").ToList();

            for (int i = 0; i < avalaibleUsrList.Count; i++)
            {
                if (lastSendingPairReq.IndexOf(avalaibleUsrList[i].userUID) == -1)
                {
                    lastSendingPairReq.Add(avalaibleUsrList[i].userUID);
                    waitPairReq = true;
                    waitTime = Time.time + 5f;
                    SendPairReq(avalaibleUsrList[i]);
                }
            }
        }
        private void SendPairReq(MatchmakingList other)
        {
            other.state.pairUID = myUID;
            DatabaseAPI.PostObject($"matchmaking/{other.userUID}", other.state, null, null);

        }
        IEnumerator matchMakingCO()
        { 
            while (myState.state != MState.paired)
            {
                waitTime = Time.time;
                yield return new WaitUntil(() => (Time.time - waitTime) > 5f);

                if (waitPairReq)
                    waitPairReq = false;
                myState.state = myState.state == MState.wait ? MState.search : MState.wait;
                //TODO: Update state

                DatabaseAPI.PostObject($"matchmaking/{myUID}", myState, null,null);
            }
        }
        public void LeaveListeners()
        {
            if(queueListener.Value != null)
                DatabaseAPI.StopListeningForValueChanged(queueListener);
            if (stateListener.Value != null)
                DatabaseAPI.StopListeningForValueChanged(stateListener); 

            DatabaseAPI.PostJSON($"matchmaking/" + Auth.Instance.currentUser.userUID, "null", null,null);


        }

        public void LeaveQueue(string myUID, Action callback, Action<AggregateException> fallback)
        {
            DatabaseAPI.StopListeningForValueChanged(queueListener);
            DatabaseAPI.PostJSON($"matchmaking/{myUID}", "null", callback, fallback);
        }

        private void Update()
        {

        }

        private void OnDestroy()
        {
            LeaveListeners();
        }
    }
}