using APIs;
using Firebase.Database;
using Serializables;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class MainManager : MonoSingleton<MainManager>
    {
        public string currentLocalPlayerId;

        public MatchmakingManager matchmakingManager;
        public GameManager gameManager;

         
        private KeyValuePair<DatabaseReference, EventHandler<ValueChangedEventArgs>> lobbyListener;


        // Start is called before the first frame update
        void Start()
        {
            matchmakingManager = GetComponent<MatchmakingManager>();
            gameManager = GetComponent<GameManager>();
        }


        public void JoinLobby()
        { 
            var id = Auth.Instance.currentUser.userUID;
            string json = "{" + '"' + "uid" + '"' + ":" + '"' + id + '"' + "}"; 
            DatabaseAPI.PushJSON("lobby/users", json, 
                () => lobbyListener = DatabaseAPI.ListenForValueChanged("lobby/users", 
                args => 
                { 
                    DataSnapshot snapshot = args.Snapshot; 
                    List <LobbyUser> liste = new List<LobbyUser>();
                    foreach (DataSnapshot snapshotChild in snapshot.Children)
                    { 
                        var child = snapshotChild.GetRawJsonValue(); 
                        try
                        {
                            var obj = JsonUtility.FromJson<LobbyUser>(child);
                            if(obj.uid.Length > 1) 
                                liste.Add(obj);
                        }
                        catch (Exception)
                        { 
                            throw;
                        }
                    }

                    MenuSceneHandler.Instance.UserChange(liste);

                },null)
                , null);
            //ListenForChildAdded("lobby/users", LobbyListener, null);
             

        }
        public void LeaveLobby()
        {
            DatabaseAPI.RemoveLobby();
            if(lobbyListener.Value != null)
                DatabaseAPI.StopListeningForValueChanged(lobbyListener);
            //DatabaseAPI.PostJSON($"matchmaking/{playerId}", "null", callback, fallback);
        }
       


    }

}