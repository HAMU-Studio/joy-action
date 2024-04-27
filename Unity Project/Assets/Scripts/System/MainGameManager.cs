using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainGameManager : MonoBehaviour
{
    [Header("�V�[�����ǂݍ��܂ꂽ����s����")]
    [SerializeField] bool OnLoadScene;

    [SerializeField] int missCount = default!;
    [SerializeField] private BlinkingScript blinkingScript = default!;
    //[SerializeField] ResultManager resultManager = default!;

    public bool isDefeat = false, isInvincible = false; //���G���Ԓ����ǂ���
   
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
    }

    private void Update()
    {

        if (UnityEngine.Input.GetKeyDown(KeyCode.P))    //�f�o�b�O�p�AP�Ń��U���g��
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
    /// ���ڂ̃n�[�g���w�肵����œ_�ł������͂�
    /// </summary>
    /// <param name="i"></param>
    public void InOrder(int i)
    {
        blinkingScript.StartCoroutine(blinkingScript.DamageIndication(i));
    }

}
