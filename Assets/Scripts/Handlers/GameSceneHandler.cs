using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using UnityEngine.SceneManagement;
using System.Linq; 

namespace Handlers
{
    public class GameSceneHandler : MonoSingleton<GameSceneHandler>
    {
        public GameObject playerPrefab;
        public GameObject yourTurnText;

        private void Start()
        {
            var players = MainManager.Instance.gameManager.currentGameInfo.playersIds;
            foreach (var player in players)
            {
                var newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
                var newPlayerHandler = newPlayer.GetComponent<PlayerHandler>();

                newPlayerHandler.playerId = player;
                newPlayerHandler.localPlayer = player == MainManager.Instance.currentLocalPlayerId;
                newPlayerHandler.yourTurnText = yourTurnText;
            }
             
        }

        public void Leave()
        {
            MainManager.Instance.gameManager.StopListeningForLocalPlayerTurn();
            var players = MainManager.Instance.gameManager.currentGameInfo.playersIds;
            foreach (var player in players.Where(p => p != MainManager.Instance.currentLocalPlayerId))
                MainManager.Instance.gameManager.StopListeningForMoves(player);
            SceneManager.LoadScene("MenuScene");
        }
    }
}