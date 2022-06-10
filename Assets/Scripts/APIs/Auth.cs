using Serializables;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace APIs
{
    public class Auth : MonoSingleton<Auth>
    {
        // Start is called before the first frame update

        Firebase.Auth.FirebaseAuth auth;

        public bool isAuth = false;
        public User currentUser;
        private void Awake()
        {
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        }
        void Start()
        {
            //NewAuthReq("alper@alper.com","123456");
            //Login("alper@alper.com", "123456");
            //SingOut();
            //GetUserInfo();

        }
        public void SignIn(string email, string password)
        {
            auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }

                // Firebase user has been created.
                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
            });
        }
        public void SingOut()
        {
            auth.SignOut();
        }
        public void Login() //With Anonymously 
        {
            auth.SignInAnonymouslyAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInAnonymouslyAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                    return;
                }
                if (task.IsCompleted)
                {
                    isAuth = true; 
                    Firebase.Auth.FirebaseUser newUser = task.Result;
                    Debug.LogFormat("User signed in successfully: {0} ({1})",
                        newUser.DisplayName, newUser.UserId);
                }


            });
        }
        public void Login(string email, string password)
        {
            auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }
                if (task.IsCompleted)
                {
                    isAuth = true;
                    Firebase.Auth.FirebaseUser newUser = task.Result;
                    Debug.LogFormat("User signed in successfully: {0} -  ({1})",
                        newUser.DisplayName, newUser.UserId);
                }


            });
        }
        public void GetUserInfo()
        {
            try
            {
                Firebase.Auth.FirebaseUser user = auth.CurrentUser;
                if (user != null)
                {
                    currentUser.email = user.Email;
                    currentUser.username = user.DisplayName;
                    currentUser.userUID = user.UserId;

                    //string name = user.DisplayName;
                    //string email = user.Email;
                    //System.Uri photo_url = user.PhotoUrl;
                    // The user's Id, unique to the Firebase project.
                    // Do NOT use this value to authenticate with your backend server, if you
                    // have one; use User.TokenAsync() instead.
                    //string uid = user.UserId;
                    //Debug.Log(name + " " + email + " " + photo_url + " " + uid);
                    isAuth = true;
                }
                else
                {
                    Debug.Log("user is null");
                    //Login("alper@alper.com", "123456");
                    Login();
                }
            }
            catch (System.Exception)
            {
                Debug.Log("no user");
                Login();
            }

        }
        public void GetUserInfo(Action callback,Action fallback)
        {
            try
            {
                Firebase.Auth.FirebaseUser user = auth.CurrentUser;
                if (user != null)
                {
                    currentUser.email = user.Email;
                    currentUser.username = user.DisplayName;
                    currentUser.userUID = user.UserId; 
                    //string name = user.DisplayName;
                    //string email = user.Email;
                    //System.Uri photo_url = user.PhotoUrl;
                    // The user's Id, unique to the Firebase project.
                    // Do NOT use this value to authenticate with your backend server, if you
                    // have one; use User.TokenAsync() instead.
                    //string uid = user.UserId;
                    //Debug.Log(name + " " + email + " " + photo_url + " " + uid);
                    isAuth = true;
                    callback();
                }
                else
                {
                    Debug.Log("user is null");
                    //Login("alper@alper.com", "123456");
                    Login();
                    fallback();
                }
            }
            catch (System.Exception)
            {
                Debug.Log("no user");
                Login();
                fallback();
            }
        } 
        void Update()
        {

        }
    }
}