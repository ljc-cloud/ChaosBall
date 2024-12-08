using ChaosBall.Utility;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace ChaosBall.UI
{
    public class LevelSelectUI : MonoBehaviour
    {
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] private LevelDataList levelDataList;
        [SerializeField] private GameObject levelInfoUIPrefab;
        [SerializeField] private RectTransform levelInfoParent;
        [SerializeField] private GameObject levelPointPrefab;
        [SerializeField] private Button selectButton;
        [SerializeField] private Transform levelPointsParent;

        [SerializeField] private Sprite currentLevelSprite;
        [SerializeField] private Sprite otherLevelSprite;

        private int _currentLevelIndex = 0;

        private Image[] _levelPoints;

        private float _levelInfoWidth;
        private float _levelInfoHeight;
        private float _levelInfoSpacing;
        private float _levelInfoParentWidth;
        
        private void Start()
        {
            InitializeSelectUI();
        }

        private void SelectNextLevel()
        {
            if (_currentLevelIndex >= levelDataList.levelList.Count - 1) return;
            _currentLevelIndex++;
            ResetSelectUI();
        }
        private void SelectPreviousLevel()
        {
            if (_currentLevelIndex <= 0) return;
            _currentLevelIndex--;
            ResetSelectUI();
        }

        private void ResetSelectUI()
        {
            ResetLevelInfoParentPosition();
            ResetLevelPoints();
            ResetLeftRightButton();
            ResetSelectButton();
        }

        private void ResetLevelInfoParentPosition()
        {
            levelInfoParent.GetComponent<RectTransform>().localPosition = new Vector3(
                -(_currentLevelIndex * _levelInfoWidth + _currentLevelIndex * _levelInfoSpacing), 0, 0);
        }

        private void ResetLevelPoints()
        {
            for (int i = 0; i < levelDataList.levelList.Count; i++)
            {
                _levelPoints[i].sprite = _currentLevelIndex == i ? currentLevelSprite : otherLevelSprite;
            }
        }

        private void ResetLeftRightButton()
        {
            if (_currentLevelIndex <= 0)
            {
                leftButton.Hide();
            }
            else
            {
                leftButton.Show();
            }
            if (_currentLevelIndex >= levelDataList.levelList.Count - 1)
            {
                rightButton.Hide();
            }
            else
            {
                rightButton.Show();
            }
        }

        private void ResetSelectButton()
        {
            // selectButton.(!levelDataList.levelList[_currentLevelIndex].isLocked);
            selectButton.enabled = !levelDataList.levelList[_currentLevelIndex].isLocked;
        }

        private void InitializeSelectUI()
        {
            leftButton.Hide();
            var levelCount = levelDataList.levelList.Count;
            if (levelCount == 1)
            {
                rightButton.Hide();
            }

            _levelPoints = new Image[levelCount];

            _levelInfoSpacing = levelInfoParent.GetComponent<HorizontalLayoutGroup>().spacing;
            _levelInfoWidth = levelInfoUIPrefab.GetComponent<RectTransform>().rect.width;
            _levelInfoHeight = levelInfoUIPrefab.GetComponent<RectTransform>().rect.height;
            _levelInfoParentWidth = _levelInfoWidth * levelCount + (levelCount - 1) * _levelInfoSpacing;
            levelInfoParent.GetComponent<RectTransform>().rect.Set(0, 0, _levelInfoParentWidth, _levelInfoHeight);
            
            for (int i = 0; i < levelCount; i++)
            {
                print("Instantiate LevelInfoUI");
                var levelInfoUIGameObject = Instantiate(levelInfoUIPrefab, levelInfoParent);
                levelInfoUIGameObject.GetComponent<LevelUI>().SetData(levelDataList.levelList[i]);
                
                var levelPointGameObject = Instantiate(levelPointPrefab, levelPointsParent);
                var image = levelPointGameObject.GetComponent<Image>();
                image.sprite = _currentLevelIndex == i ? currentLevelSprite : otherLevelSprite;
                _levelPoints[i] = image;
            }
            
            leftButton.onClick.AddListener(SelectPreviousLevel);
            rightButton.onClick.AddListener(SelectNextLevel);
            selectButton.onClick.AddListener(() =>
            {
                SceneLoader.LoadScene(SceneEnum.LoadScene);
                SceneLoader.NEXT_LEVEL = levelDataList.levelList[_currentLevelIndex].level;
            });
        }
    }

}

