using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using UnityEngine.SceneManagement;
using System.Linq;
using APIs;
using System;

namespace Handlers
{
    public class GameSceneHandler : MonoSingleton<GameSceneHandler>
    {
        public GameObject playerPrefab;
        public GameObject yourTurnText;
        public GameObject cube;
        public bool state = false;
        private void Start()
        {
            //var players = MainManager.Instance.gameManager.currentGameInfo.playersIds;
            //foreach (var player in players)
            //{
            //    var newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            //    var newPlayerHandler = newPlayer.GetComponent<PlayerHandler>();

            GameManager.Instance.ConfigureGameArea(Auth.Instance.currentUser.userUID, null, null);
            StartCoroutine(TestLoop());

        }

        IEnumerator TestLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.01f);
                GameManager.Instance.UpdateValue();
            }
        }
        public void Leave()
        {
            //MainManager.Instance.gameManager.StopListeningForLocalPlayerTurn();
            //var players = MainManager.Instance.gameManager.currentGameInfo.playersIds;
            //foreach (var player in players.Where(p => p != MainManager.Instance.currentLocalPlayerId))
            //    MainManager.Instance.gameManager.StopListeningForMoves(player);
            //SceneManager.LoadScene("MenuScene");
        }

        internal void UpdatePos()
        {
            if (state)
                cube.transform.position = GameManager.Instance.otherGD.position;
        }
    }
}