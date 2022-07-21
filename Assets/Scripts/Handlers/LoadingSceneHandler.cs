using APIs;
using Serializables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public List<User> userList; // for test 
    public List<User> userList2;

    private void Awake()
    {
        DatabaseAPI.InitializeDatabase();
    }
    void Start()
    {
        
        Auth.Instance.GetUserInfo(LoadScene,null);
        //DatabaseAPI.GetJSON("userInfos", getData, null);
        //DatabaseAPI.GetAllDatas("userInfos");
        DatabaseAPI.GetAllDatas("userInfos", new User(), postobjCallback);

        //DatabaseAPI.PostObject("userInfos", userList, null, null);
        //DatabaseAPI.GetJSON("userList", getData, null);
        //GetJSON(path, snapshot => callback(snapshot.Exists), fallback);

        //    DatabaseAPI.GetJSON("userList",
        //json => { getData((T)StringSerializationAPI.Deserialize(typeof(T), json.GetRawJsonValue())); },
        //null);
    }
    void postobjCallback(List<User> list)
    {
        userList2 = list;
        //Debug.Log("posted");
    }
    void getData(Firebase.Database.DataSnapshot snapshot)
    {
        Debug.Log(snapshot.Value);
        foreach (var user in snapshot.Children)
        {
            IDictionary dictUser = (IDictionary)user.Value;
            string data = dictUser.ToString();
            Debug.Log(data);
            var usr = JsonUtility.FromJson<User>(data);
            userList2.Add(usr);
            Debug.Log("" + dictUser["email"] + " - " + dictUser["pass"]);
        }
    }
    void LoadScene()
    {
        //SceneManager.LoadScene("MenuScene");

        StartCoroutine(LoadSceneCO());
    }
    IEnumerator LoadSceneCO()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("MenuScene");


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            DatabaseAPI.GetAllDatas("userInfos", new User(), postobjCallback);

        }
    }
}
