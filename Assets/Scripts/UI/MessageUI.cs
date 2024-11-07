using System.Collections;
using System.Collections.Generic;
using ChaosBall.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace ChaosBall.UI
{
    public class MessageUI : MonoBehaviour
    {
        [SerializeField] private Text messageText;

        private Animator _animator;
        
        private Queue<string> _messageQue = new Queue<string>();

        private int _messageInHash = Animator.StringToHash("msg_in");
        private int _messageOutHash = Animator.StringToHash("msg_out");

        private float _timer;
        private float _messageExist = 1.5f;
        private bool _messaging = false;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            
            GameManager.Instance.OnSendMessage += ExecuteMessage;
        }

        private void ExecuteMessage(string message)
        {
            _messageQue.Enqueue(message);
            StartCoroutine(ShowMessage());
        }
        
        private void OnDestroy()
        {
            GameManager.Instance.OnSendMessage -= ExecuteMessage;
            StopCoroutine(nameof(ShowMessage));
        }

        private IEnumerator ShowMessage()
        {
            while (_messageQue.Count > 0)
            {
                messageText.text = _messageQue.Dequeue();
                _animator.SetTrigger(_messageInHash);
                yield return new WaitForSeconds(2f);
                _animator.SetTrigger(_messageOutHash);
            }
        }

        public void NotifyMessageEnd()
        {
            if (_messageQue.Count == 0)
            {
                GameManager.Instance.SetMessagingOver();
            }
        }
    } 
}