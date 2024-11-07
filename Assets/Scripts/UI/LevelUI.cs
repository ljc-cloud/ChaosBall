using System;
using UnityEngine;
using UnityEngine.UI;

namespace ChaosBall.UI
{
    public class LevelUI : MonoBehaviour
    {
        [SerializeField] private Text levelNameText;
        [SerializeField] private Image levelImage;
        [SerializeField] private Text levelDescriptionText;
        [SerializeField] private Transform lockedImage;

        public void SetData(LevelData levelData)
        {
            levelNameText.text = levelData.levelName;
            levelImage.sprite = levelData.levelSprite;
            levelDescriptionText.text = levelData.levelDescription;
            lockedImage.gameObject.SetActive(levelData.isLocked);
        }
    }
}