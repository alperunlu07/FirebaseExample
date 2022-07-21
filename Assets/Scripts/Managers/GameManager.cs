using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using APIs;
using Firebase.Database;
using Handlers;
using Serializables;
using UnityEngine;

namespace Managers
{
    
    public class GameManager : MonoSingleton<GameManager>
    {
        //public GameInfo currentGameInfo;

        private Dictionary<string, bool> readyPlayers;
        private KeyValuePair<DatabaseReference, EventHandler<ChildChangedEventArgs>> readyListener;
        private KeyValuePair<DatabaseReference, EventHandler<ValueChangedEventArgs>> localPlayerTurnListener;
        private KeyValuePair<DatabaseReference, EventHandler<ValueChangedEventArgs>> currentGameInfoListener;
        private KeyValuePair<DatabaseReference, EventHandler<ValueChangedEventArgs>> otherPlayerListener;

        private readonly Dictionary<string, KeyValuePair<DatabaseReference, EventHandler<ChildChangedEventArgs>>>
            moveListeners =
                new Dictionary<string, KeyValuePair<DatabaseReference, EventHandler<ChildChangedEventArgs>>>();

        private string myUID;
        public GameData gameData;

        public GameData currentGD, otherGD;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
        private void Start()
        {


        }
        public void ConfigureGameArea(string _myUID, Action<string> onGameFound, Action<AggregateException> fallback)
        {
            StartCoroutine(ListenerCheck());

            myUID = _myUID;
            Debug.Log("ConfigureGameArea");
            currentGD.data2 = 0;
            DatabaseAPI.PostObject($"games/{myUID}", currentGD, /*() => StartCoroutine(ListenerCheck()*/ CallBack,
                // We listen for the placeholder value changing afterwards...
                //() => 
                (ex) => Debug.Log(ex.Message));

            //StartCoroutine(matchMakingCO());
        }

        private void CallBack()
        {
            Debug.Log("Call back");
            //ListenerCheck();
        }

        IEnumerator ListenerCheck()
        {
            Debug.Log("ListenerCheck");
            string crossUID = MatchmakingManager.Instance.myState.pairUID;
            bool isGameData = false;
            float t = Time.time;
            while (!isGameData)
            {
                yield return new WaitForSeconds(1f);
                DatabaseAPI.GetObject<GameData>($"games/{crossUID}/", (gameData) =>
                {
                    if (gameData.data2 != -1) isGameData = true;
                    else Debug.Log("Game data not found");
                }, null); 
            }



            otherPlayerListener = DatabaseAPI.ListenForValueChanged($"games/{crossUID}/",
            args =>
            {
                var values = args.Snapshot.GetRawJsonValue();
                otherGD = (GameData)StringSerializationAPI.Deserialize(typeof(GameData), values);

                GameSceneHandler.Instance.UpdatePos();
                //Debug.Log(" me -> " + values);

            }, null);

        }
        public void UpdateValue()
        {
            currentGD.position = GameSceneHandler.Instance.cube.transform.position;
            DatabaseAPI.PostObject($"games/{myUID}", currentGD, null, null);
        }

        //public void ListenJoin()
        //{
        //    //Debug.Log("listen Join");

        //    stateListener = DatabaseAPI.ListenForValueChanged($"matchmaking/{Auth.Instance.currentUser.userUID}",
        //        args =>
        //        {
        //            var values = args.Snapshot.GetRawJsonValue();
        //            var myState_ = (MatchmakingState)StringSerializationAPI.Deserialize(typeof(MatchmakingState), values);
        //            if (myState_ != null)
        //            {
        //                if (myState_.state == MState.wait && myState_.pairUID.Length > 1) // pairing request
        //                {
        //                    myState.pairUID = myState_.pairUID;
        //                    PairingRequest();
        //                }
        //                if (myState_.state == MState.paired && myState_.pairUID.Length > 1) // accept response message
        //                {
        //                    isMaster = true;
        //                    myState.pairUID = myState_.pairUID;
        //                    myState.state = MState.paired;
        //                    MatchmakingSceneHandler.Instance.OpenGameScene();
        //                }
        //            }
        //            //Debug.Log(" me -> " + values);

        //        }, null);
        //    queueListener = DatabaseAPI.ListenForValueChanged($"matchmaking/",
        //        args =>
        //        {
        //            // This code gets once the placeholder value is changed
        //            var gameId = args.Snapshot.GetRawJsonValue();

        //            //Debug.Log(" que -> " + gameId);

        //            DataSnapshot snapshot = args.Snapshot;
        //            //Debug.Log(snapshot.ChildrenCount);
        //            //Debug.Log(snapshot.Key);

        //            //matchmakingLists = new List<MatchmakingList>();
        //            matchmakingLists.Clear();
        //            foreach (DataSnapshot snapshotChild in snapshot.Children)
        //            {
        //                //MatchmakingList obj_ = new MatchmakingList();
        //                //obj_.userUID = snapshotChild.Key;
        //                var child = snapshotChild.GetRawJsonValue();
        //                //var std_ = JsonUtility.FromJson<MatchmakingState>(child);
        //                MatchmakingState std_ = (MatchmakingState)StringSerializationAPI.Deserialize(typeof(MatchmakingState), child);
        //                var obj_ = new MatchmakingList() { state = std_, userUID = snapshotChild.Key };
        //                matchmakingLists.Add(obj_);
        //                if (myState.state == MState.search && !waitPairReq)
        //                {
        //                    FindPairObj();
        //                }
        //            }

        //        }, null);

        //}

        //public void GetCurrentGameInfo(string gameId, string localPlayerId, Action<GameInfo> callback,
        //    Action<AggregateException> fallback)
        //{
        //    currentGameInfoListener =
        //        DatabaseAPI.ListenForValueChanged($"games/{gameId}/gameInfo", args =>
        //        {
        //            if (!args.Snapshot.Exists) return;

        //            var gameInfo =
        //                StringSerializationAPI.Deserialize(typeof(GameInfo), args.Snapshot.GetRawJsonValue()) as
        //                    GameInfo;
        //            currentGameInfo = gameInfo;
        //            currentGameInfo.localPlayerId = localPlayerId;
        //            DatabaseAPI.StopListeningForValueChanged(currentGameInfoListener);
        //            callback(currentGameInfo);
        //        }, fallback);
        //}

        //public void SetLocalPlayerReady(Action callback, Action<AggregateException> fallback)
        //{
        //    DatabaseAPI.PostObject($"games/{currentGameInfo.gameId}/ready/{currentGameInfo.localPlayerId}", true,
        //        callback,
        //        fallback);
        //}

        //public void ListenForAllPlayersReady(IEnumerable<string> playersId, Action<string> onNewPlayerReady,
        //    Action onAllPlayersReady,
        //    Action<AggregateException> fallback)
        //{
        //    readyPlayers = playersId.ToDictionary(playerId => playerId, playerId => false);
        //    readyListener = DatabaseAPI.ListenForChildAdded($"games/{currentGameInfo.gameId}/ready/", args =>
        //    {
        //        readyPlayers[args.Snapshot.Key] = true;
        //        onNewPlayerReady(args.Snapshot.Key);
        //        if (!readyPlayers.All(readyPlayer => readyPlayer.Value)) return;
        //        StopListeningForAllPlayersReady();
        //        onAllPlayersReady();
        //    }, fallback);
        //}

        //public void StopListeningForAllPlayersReady() => DatabaseAPI.StopListeningForChildAdded(readyListener);

        //public void SendMove(Move move, Action callback, Action<AggregateException> fallback)
        //{
        //    DatabaseAPI.PushObject($"games/{currentGameInfo.gameId}/{currentGameInfo.localPlayerId}/moves/", move,
        //        () =>
        //        {
        //            Debug.Log("Moved sent successfully!");
        //            callback();
        //        }, fallback);
        //}

        //public void ListenForLocalPlayerTurn(Action onLocalPlayerTurn, Action<AggregateException> fallback)
        //{
        //    localPlayerTurnListener =
        //        DatabaseAPI.ListenForValueChanged($"games/{currentGameInfo.gameId}/turn", args =>
        //        {
        //            var turn =
        //                StringSerializationAPI.Deserialize(typeof(string), args.Snapshot.GetRawJsonValue()) as string;
        //            if (turn == currentGameInfo.localPlayerId) onLocalPlayerTurn();
        //        }, fallback);
        //}

        //public void StopListeningForLocalPlayerTurn() =>
        //    DatabaseAPI.StopListeningForValueChanged(localPlayerTurnListener);

        //public void ListenForMoves(string playerId, Action<Move> onNewMove, Action<AggregateException> fallback)
        //{
        //    moveListeners.Add(playerId, DatabaseAPI.ListenForChildAdded(
        //        $"games/{currentGameInfo.gameId}/{playerId}/moves/",
        //        args => onNewMove(
        //            StringSerializationAPI.Deserialize(typeof(Move), args.Snapshot.GetRawJsonValue()) as Move),
        //        fallback));
        //}

        //public void StopListeningForMoves(string playerId)
        //{
        //    DatabaseAPI.StopListeningForChildAdded(moveListeners[playerId]);
        //    moveListeners.Remove(playerId);
        //}

        //public void SetTurnToOtherPlayer(string currentPlayerId, Action callback, Action<AggregateException> fallback)
        //{
        //    var otherPlayerId = currentGameInfo.playersIds.First(p => p != currentPlayerId);
        //    DatabaseAPI.PostObject(
        //        $"games/{currentGameInfo.gameId}/turn", otherPlayerId, callback, fallback);
        //}
    }
}