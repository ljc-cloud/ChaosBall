using System;
using System.Collections;
using System.Collections.Generic;
using ChaosBall.Event.Game;
using ChaosBall.So;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ChaosBall.UI
{
    public enum UIPanelType
    {
        MessageUI,
        MainMenuUI,
        SignInUI,
        SignUpUI,
        RoomListUI,
        RoomUI,
        GameUI,
        GameOverUI,
        CreateRoomUI,
    }

    public enum ShowUIPanelType
    {
        MoveFadeIn,
        FadeIn
    }

    public enum HideUIPanelType
    {
        MoveFadeOut,
        FadeOut
    }
    public class UIManager : BaseManager
    {
        public const float UISHOW_START_POSITION = 500f;
        public const float UISHOW_END_POSITION = 0f;
        public const float UIHIDE_START_POSITION = 0f;
        public const float UIHIDE_END_POSITION = -500f;
        public const float UISHOW_DURATION = .3f;
        
        private RectTransform _mCanvas;
        private UIPanelSoListSo _mUIPanelSoListSo;
        public UIManager(UIPanelSoListSo uiPanelSoListSo)
        {
            _mUIPanelSoListSo = uiPanelSoListSo;
        }
        
        private Dictionary<UIPanelType, BaseUIPanel> _mUIPanelDict = new();
        private Dictionary<UIPanelType, string> _mUIPanelPathDict = new();
        
        private Stack<BaseUIPanel> _mUIPanelStack = new();
        
        private readonly Dictionary<UIPanelType, BaseUIPanel> _mUIPanelPool = new();

        public override void OnInit()
        {
            base.OnInit();
            _mCanvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
            GameInterface.Interface.EventSystem.Subscribe<SceneLoadEvent>(OnSceneLoad);
            GameInterface.Interface.EventSystem.Subscribe<SceneLoadCompleteEvent>(OnSceneLoadComplete);
            InitUIPanel();
        }
        
        public override void OnDestroy()
        {
            GameInterface.Interface.EventSystem.Unsubscribe<SceneLoadCompleteEvent>(OnSceneLoadComplete);
            GameInterface.Interface.EventSystem.Unsubscribe<SceneLoadEvent>(OnSceneLoad);
            base.OnDestroy();
        }

        private void OnSceneLoad(SceneLoadEvent _)
        {
            ClearUIPanel();
        }

        private void OnSceneLoadComplete(SceneLoadCompleteEvent _)
        {
            Debug.Log("ClearUIPanel");
            // ClearUIPanel();
            _mCanvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
        }
        
        public void PushUIPanel(UIPanelType uiPanelType, ShowUIPanelType showUIPanelType)
        {
            Debug.Log($"PushUIPanel,Type: {uiPanelType}");
            // stack栈顶ui隐藏
            if (_mUIPanelStack.TryPeek(out var uiPanel))
            {
                HideUIPanel(uiPanel.transform, HideUIPanelType.MoveFadeOut);
            }

            if (_mUIPanelDict.TryGetValue(uiPanelType, out var panel))
            {
                panel.OnBeforeShow();
                ShowUIPanel(panel.transform, showUIPanelType);
                _mUIPanelStack.Push(panel);
                panel.OnShow();
                panel.OnAfterShow();
            }
            else
            {
                SpawnUIPanelAsync(uiPanelType, baseUIPanel =>
                {
                    baseUIPanel.OnBeforeShow();
                    ShowUIPanel(baseUIPanel.transform, showUIPanelType);
                    _mUIPanelStack.Push(baseUIPanel);
                    baseUIPanel.OnShow();
                    baseUIPanel.OnAfterShow();
                });
            }
        }

        public void PushUIPanelNotHide(UIPanelType uiPanelType, ShowUIPanelType showUIPanelType)
        {
            if (_mUIPanelDict.TryGetValue(uiPanelType, out var panel))
            {
                panel.OnBeforeShow();
                ShowUIPanel(panel.transform, showUIPanelType);
                _mUIPanelStack.Push(panel);
                panel.OnShow();
                panel.OnAfterShow();
            }
            else
            {
                SpawnUIPanelAsync(uiPanelType, baseUIPanel =>
                {
                    baseUIPanel.OnBeforeShow();
                    ShowUIPanel(baseUIPanel.transform, showUIPanelType);
                    _mUIPanelStack.Push(baseUIPanel);
                    baseUIPanel.OnShow();
                    baseUIPanel.OnAfterShow();
                });
            }
        }

        public void PopUIPanel()
        {
            // stack 栈顶ui隐藏并弹出
            if (_mUIPanelStack.TryPop(out var uiPanel))
            {
                Debug.Log("当前PopUI:" + uiPanel.name);
                HideUIPanel(uiPanel.transform, HideUIPanelType.MoveFadeOut);
            }
            
            // stack 栈顶ui显示
            if (_mUIPanelStack.TryPeek(out var panel))
            {
                panel.OnBeforeShow();
                ShowUIPanel(panel.transform, ShowUIPanelType.MoveFadeIn);
                panel.OnShow();
                panel.OnAfterShow();
            }
        }

        public void PopUIPanelNotShow()
        {
            // stack 栈顶ui隐藏并弹出
            if (_mUIPanelStack.TryPop(out var uiPanel))
            {
                HideUIPanel(uiPanel.transform, HideUIPanelType.MoveFadeOut);
            }
        }

        public void PopUIPanelNoAction()
        {
            if (_mUIPanelStack.TryPop(out var uiPanel))
            {
                Debug.Log("当前PopUI:" + uiPanel.name);

            }

        }

        public void ShowMessage(string message)
        {
            if (string.IsNullOrEmpty(message)) return;
            if (_mUIPanelDict.TryGetValue(UIPanelType.MessageUI, out var uiPanel))
            {
                MessageUI messageUI = uiPanel as MessageUI;
                messageUI?.ShowMessage(message);
            }
            else
            {
                SpawnUIPanelAsync(UIPanelType.MessageUI, panel =>
                {
                    MessageUI messageUI = panel as MessageUI;
                    messageUI?.ShowMessage(message);
                });
            }
        }

        public void HideAllUIPanel()
        {
            foreach (var panel in _mUIPanelStack)
            {
                HideUIPanel(panel.transform, HideUIPanelType.FadeOut);
            }
        }
        
        public void ClearUIPanel()
        {
            // HideAllUIPanel();
            
            Debug.Log("清除所有UI");
            _mUIPanelStack.Clear();
            foreach (var baseUIPanel in _mUIPanelDict.Values)
            {
                Object.Destroy(baseUIPanel.gameObject);
            }
            _mUIPanelDict.Clear();
        }

        private void ShowUIPanel(Transform uiTransform, ShowUIPanelType showUIPanelType)
        {
            CanvasGroup canvasGroup = uiTransform.GetComponent<CanvasGroup>();
            canvasGroup.DOFade(1, UISHOW_DURATION);
            if (showUIPanelType is ShowUIPanelType.MoveFadeIn)
            {
                uiTransform.DOLocalMoveX(UISHOW_END_POSITION, UISHOW_DURATION);
            }
            else
            {
                uiTransform.localPosition = new Vector3(UISHOW_END_POSITION
                    , uiTransform.localPosition.y, 0);
            }
            
            canvasGroup.blocksRaycasts = true;
        }

        private void HideUIPanel(Transform uiTransform, HideUIPanelType hideUIPanelType)
        {
            void OnComplete()
            {
                uiTransform.localPosition = new Vector3(UISHOW_START_POSITION, uiTransform.localPosition.y, 0);
            }
            CanvasGroup canvasGroup = uiTransform.GetComponent<CanvasGroup>();
            if (hideUIPanelType is HideUIPanelType.MoveFadeOut)
            {
                uiTransform.DOLocalMoveX(UIHIDE_END_POSITION, UISHOW_DURATION);
            }
            canvasGroup.DOFade(0, UISHOW_DURATION).OnComplete(OnComplete);
            canvasGroup.blocksRaycasts = false;
        }

        private BaseUIPanel SpawnUIPanel(UIPanelType uiPanelType)
        {
            if (_mUIPanelPathDict.TryGetValue(uiPanelType, out string path))
            {
                Debug.Log($"Spawn UI, Type:{uiPanelType}, Path:{path}");
                GameObject uiPrefab = Resources.Load<GameObject>(path);
                GameObject uiGameObject = GameObject.Instantiate(uiPrefab, _mCanvas);
                BaseUIPanel baseUIPanel = uiGameObject.GetComponent<BaseUIPanel>();
                _mUIPanelDict[uiPanelType] = baseUIPanel;
                CanvasGroup canvasGroup = uiGameObject.GetComponent<CanvasGroup>();
                canvasGroup.alpha = 0f;
                canvasGroup.blocksRaycasts = false;
                baseUIPanel.OnInit();
                return baseUIPanel;
            }
            else
            {
                Debug.LogError($"UI Panel Type {uiPanelType} path not found");
                return null;
            }
        }

        private void SpawnUIPanelAsync(UIPanelType uiPanelType, Action<BaseUIPanel> onComplete)
        {
            if (_mUIPanelPathDict.TryGetValue(uiPanelType, out string path))
            {
                Debug.Log($"Spawn UI, Type:{uiPanelType}, Path:{path}");
                ResourceRequest resourceRequest = Resources.LoadAsync<GameObject>(path);
                resourceRequest.completed += _ =>
                {
                    GameObject uiPrefab = resourceRequest.asset as GameObject;
                    GameObject uiGameObject = GameObject.Instantiate(uiPrefab, _mCanvas);
                    BaseUIPanel baseUIPanel = uiGameObject.GetComponent<BaseUIPanel>();
                    _mUIPanelDict[uiPanelType] = baseUIPanel;
                    CanvasGroup canvasGroup = uiGameObject.GetComponent<CanvasGroup>();
                    canvasGroup.alpha = 0f;
                    canvasGroup.blocksRaycasts = false;
                    baseUIPanel.OnInit();
                    onComplete?.Invoke(baseUIPanel);
                };
            }
            else
            {
                Debug.LogError($"UI Panel Type {uiPanelType} path not found");
            }
        }

        private void InitUIPanel()
        {
            foreach (var uiPanelSo in _mUIPanelSoListSo.uIPanelSoList)
            {
                Debug.Log($"UIAdd,Type:{uiPanelSo.uIPanelType},Path:{uiPanelSo.path}");
                _mUIPanelPathDict.Add(uiPanelSo.uIPanelType, uiPanelSo.path);
            }
        }
    }
}