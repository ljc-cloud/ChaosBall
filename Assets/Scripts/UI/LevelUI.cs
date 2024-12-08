using UnityEngine;
using UnityEngine.UI;

namespace ChaosBall.UI
{
    public class LevelUI : MonoBehaviour
    {
        [SerializeField] private Text levelNameText;
        [SerializeField] private Image levelImage;
        [SerializeField] private Transform lockedImage;
        [SerializeField] private GameObject ballDescriptionPrefab;
        [SerializeField] private Transform ballDescriptionParent;

        public void SetData(LevelData levelData)
        {
            levelNameText.text = levelData.levelName;
            levelImage.sprite = levelData.levelSprite;
            lockedImage.gameObject.SetActive(levelData.isLocked);
            foreach (var ballDescription in levelData.ballDescriptions)
            {
                var descGameObject = Instantiate(ballDescriptionPrefab, ballDescriptionParent);
                descGameObject.GetComponent<BallDescriptionUI>().SetData(ballDescription.sprite, ballDescription.description);
            }
        }
    }
}