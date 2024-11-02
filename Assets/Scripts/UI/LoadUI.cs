using System;
using System.Collections;
using ChaosBall.Utility;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ChaosBall.UI
{
    public class LoadUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private TextMeshProUGUI loadCompleteText;

        private bool _isLoadDone;

        private AsyncOperation _operation;
        
        private void Start()
        {
            _isLoadDone = false;
            loadCompleteText.Hide();
            SceneLoader.LoadSceneAsync(SceneEnum.GameScene, operation =>
            {
                _operation = operation;
                operation.allowSceneActivation = false;
                StartCoroutine(LoadGameScene());
            });
        }

        private IEnumerator LoadGameScene()
        {
            while (!_operation.isDone)
            {
                progressText.text = _operation.progress * 100 + "%";
                if (Math.Abs(_operation.progress - .9f) <= .01f)
                {
                    progressText.text = "100%";
                    loadCompleteText.Show();
                    _isLoadDone = true;
                }
                yield return new WaitForEndOfFrame();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isLoadDone) return;
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                _operation.allowSceneActivation = true;
            }
        }
    }

}
