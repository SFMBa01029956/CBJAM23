using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;  
using TMPro;

public class newScore : MonoBehaviour
{
    string initials;
    int score = 0;
    [SerializeField]
    GameObject TextScore;

    // private void OnEnable()
    // {
    //     ScoreDisplay.OnScore += UpdateScore;
    // }
    
    // private void OnDisable()
    // {
    //     ScoreDisplay.OnScore -= UpdateScore;
    // }

    // private void UpdateScore(int _score)
    // {
    //     Debug.Log(_score);
    //     score = _score;
    //     TextScore.GetComponent<TMP_Text>().text = "YOUЯ SCOЯE: " + score.ToString();
    // }

        IEnumerator NewScore(int score, string name)
    {
        WWWForm form = new WWWForm();

        form.AddField("name", name);
        form.AddField("score", score);
        
        UnityWebRequest www = UnityWebRequest.Post("https://morbin-backend.onrender.com/newScore", form);
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success){
            Debug.Log("Error");
        }else{
            Debug.Log("Score saved!");
        }
    }

    public void readString(string text)
    {
        initials = text;
        Debug.Log(initials);
    }

    public void onClick()
    {
        StartCoroutine(NewScore(score, initials));
    }
}
