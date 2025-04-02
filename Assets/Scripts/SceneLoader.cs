using System;
using ChaosBall.Event.Game;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChaosBall
{
    public enum Scene 
    {
        MainMenuScene,
        RoomScene,
        LoadingScene,
        GameScene,
        GameOverScene,
    }

    public class SceneLoader
    {
        // public event Action OnSceneLoad;
        // public event Action OnSceneLoadComplete;
    
        public void LoadScene(Scene scene)
        {
            GameInterface.Interface.EventSystem.Publish<SceneLoadEvent>();
            // OnSceneLoad?.Invoke();
            SceneManager.LoadScene(Scene.LoadingScene.ToString());
        
            AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Single);
            // loadSceneAsync.completed += _ => OnSceneLoadComplete?.Invoke(); 
            loadSceneAsync.completed += _ => GameInterface.Interface.EventSystem.Publish(new SceneLoadCompleteEvent
            {
                targetScene = scene
            });
        }

        public AsyncOperation LoadGameSceneAsync()
        {
            AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(Scene.GameScene.ToString(), LoadSceneMode.Single);
            loadSceneAsync.completed += _ =>
            {
                GameInterface.Interface.EventSystem.Publish(new SceneLoadCompleteEvent
                {
                    targetScene = Scene.GameScene
                });
            };
            return loadSceneAsync;
        }

        public void LoadSceneAsync(Scene scene, Action onComplete = null)
        {
            // OnSceneLoad?.Invoke();
            GameInterface.Interface.EventSystem.Publish<SceneLoadEvent>();
            AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Single);

            loadSceneAsync.completed += _ =>
            {
                // OnSceneLoadComplete?.Invoke(); 
                GameInterface.Interface.EventSystem.Publish(new SceneLoadCompleteEvent
                {
                    targetScene = scene
                });
                onComplete?.Invoke();
            };
        }
    }
}