using System;
using System.Collections.Generic;

namespace Serializables
{
    [Serializable]
    public class GameInfo
    {
        public string gameId;
        public string[] playersIds;
        public string localPlayerId;
    }

    [System.Serializable]
    public class User
    {
        public string userId;
        public string userUID;
        public string username;
        public string password;
        public string email;

        public User()
        {

        }

        public User(string userId,string userUID, string username, string password, string email)
        {
            this.userId = userId;
            this.userUID = userUID;
            this.username = username;
            this.password = password;
            this.email = email;
        }
    }
    [System.Serializable]
    public class Lobby
    {
        public List<User> userList; 
    }
    [System.Serializable] 
    public class LobbyUser
    {
        public string uid;
    }
    [System.Serializable]
    public class MatchmakingState
    {
        public mState state;
    }
    public enum mState
    {
        wait = 0,
        search,
        paired
    }
    [System.Serializable]
    public class MatchmakingList
    {
        public string userUID;
        public MatchmakingState state;
    }
}