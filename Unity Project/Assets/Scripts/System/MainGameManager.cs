using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainGameManager : MonoBehaviour
{
    [Header("シーンが読み込まれたら実行する")]
    [SerializeField] bool OnLoadScene;

    [SerializeField] int missCount = default!;
    [SerializeField] private BlinkingScript blinkingScript = default!;
    //[SerializeField] ResultManager resultManager = default!;
    //[SerializeField] private List<Button> buttons;
    [SerializeField] private GameObject startButton;
    public bool isDefeat = false, isInvincible = false; //無敵時間中かどうか
   
    int currentCount = 0;

    //GameObject obj = default;
    bool onlyF = false;

    void Start()
    {
        if (OnLoadScene)
        {
            Debug.Log("OnLoadScene");
            onlyF = false;
            isDefeat = false;            
        }
        //buttons[0].Select();
      //  startButton = startButton.GetComponent<GameObject>();
        Time.timeScale = 0f;
    }

    private void Update()
    {

        if (UnityEngine.Input.GetKeyDown(KeyCode.P))    //デバッグ用、Pでリザルトへ
        {
            isDefeat = false;
        }     

        if (isDefeat)
        {         
            isDefeat = false;
        }
      
    }
    public void Miss()
    {
        if (currentCount >= missCount) return;

        InOrder(currentCount);      

        currentCount++;
    }

    /// <summary>
    /// 何個目のハートか指定した上で点滅だったはず
    /// </summary>
    /// <param name="i"></param>
    public void InOrder(int i)
    {
        blinkingScript.StartCoroutine(blinkingScript.DamageIndication(i));
    }

    public void StartButton()
    {
        Time.timeScale = 1f;
        startButton.SetActive(false);
    }

}
