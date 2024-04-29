using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class PlayerDetector : MonoBehaviour
{
    // OnTriggerStay�C�x���g���Ɏ��s�������֐���o�^����ϐ��i������Collider�����j
    [SerializeField]
    private UnityEvent<Collider> onTriggerStayEvent = new UnityEvent<Collider>();

    // OnTriggerExit�C�x���g���Ɏ��s�������֐���o�^����ϐ��i������Collider�����j
    [SerializeField]
    private UnityEvent<Collider> onTriggerExitEvent = new UnityEvent<Collider>();

    // Is Trigger��ON�ő���GameObject��Collider���ɂ���Ƃ��ɌĂ΂ꑱ����
    private void OnTriggerStay(Collider other)
    {
        // Inspector�^�u��onTriggerStayEvent�Ŏw�肳�ꂽ���������s����
        onTriggerStayEvent.Invoke(other);
    }
    
    // Is Trigger��ON�ő���GameObject��Collider����o���Ƃ��ɌĂ΂��
    private void OnTriggerExit(Collider other)
    {
        // Inspector�^�u��onTriggerExitEvent�Ŏw�肳�ꂽ���������s����
        onTriggerExitEvent.Invoke(other);
    }

}