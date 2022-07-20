using APIs;
using Managers;
using Serializables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuSceneHandler : MonoSingleton<MenuSceneHandler>
{
    //public InputField userName;
    public Text currentUserCountText;
    public List<LobbyUser> lobbyUsers;
    void Start()
    {
        //
        //DatabaseAPI.AddLobby();

        //DatabaseAPI.PushObject($"games/asd/xcdas/moves/", "move",
        //       () =>
        //       {
        //           Debug.Log("Moved sent successfully!");

        //       }, null);
        MainManager.Instance.currentLocalPlayerId = Auth.Instance.currentUser.userUID;

        //Play(); // for test 
    }
    private void OnEnable()
    { 
        MainManager.Instance.JoinLobby();
    }
    public void UserChange(List<LobbyUser> users)
    {
        //Debug.Log(userRawData);
        lobbyUsers = users;
        currentUserCountText.text = lobbyUsers.Count.ToString();

    }
    public void Play()
    {
        //string playerId = userName.text;
        //Debug.Log(playerId);
        SceneManager.LoadScene("MatchmakingScene");
    }
    // Update is called once per frame
    void Update()
    {

    }
    private void OnDestroy()
    {
        MainManager.Instance.LeaveLobby(); 
    }
    private void OnDisable()
    {

    }
}
