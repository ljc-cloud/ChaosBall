using ChaosBall.Net.Request;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChaosBall
{
    public class Loader : MonoBehaviour
    {
        [SerializeField] private Slider progressBar;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private TextMeshProUGUI tipText;

        private int _mNowProgress;
        private int _mTargetProgress;
        private AsyncOperation _mLoadSceneAsyncOperation;

        private bool _mRequestSend;
    
        private void Start()
        {
            tipText.gameObject.SetActive(false);
            _mLoadSceneAsyncOperation = GameInterface.Interface.SceneLoader.LoadGameSceneAsync();
            _mLoadSceneAsyncOperation.allowSceneActivation = false;
        }

        private void Update()
        {
            if (_mLoadSceneAsyncOperation == null)
            {
                return;
            }

            if (_mLoadSceneAsyncOperation.progress < 0.9f)
            {
                _mTargetProgress = (int)(_mLoadSceneAsyncOperation.progress * 100);
            }
            else
            {
                _mTargetProgress = 100;
            }

            if (_mNowProgress < _mTargetProgress)
            {
                _mNowProgress++;
            }

            progressBar.value = (float)_mNowProgress / 100;
            progressText.text = $"{_mNowProgress}%";
            if (_mNowProgress == 100 && !_mRequestSend)
            {
                LoadGameSceneCompleteRequest request = GameInterface.Interface.RequestManager
                    .GetRequest<LoadGameSceneCompleteRequest>();
                request.SendLoadGameSceneCompleteRequest(OnAllPlayerLoadComplete);
                _mRequestSend = true;
                tipText.gameObject.SetActive(true);
            }
        }

        private void OnAllPlayerLoadComplete()
        {
            _mLoadSceneAsyncOperation.allowSceneActivation = true;
        }
    }
}