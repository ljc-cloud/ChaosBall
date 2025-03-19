using UnityEngine;

namespace ChaosBall
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject birdPrefab;
        [SerializeField] private GameObject[] birdPrefabArray;
        public static GameManager Instance { get; private set; }
        public const int MAX_ROUND = 4;

        private int _mCurrentRound = 1;
        private GameObject[] _mBirdPrefabArray;
        private int[] _mScoreArray;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Debug.Log("Starting Game!");
            // _mBirdPrefabArray = new GameObject[MAX_ROUND];
            // Array.Fill(_mBirdPrefabArray, birdPrefab);
            _mScoreArray = new int[MAX_ROUND];

            GameObject birdGameObject = Instantiate(birdPrefabArray[_mCurrentRound - 1]);
        }

        public void FinishRound(int score)
        {
            _mScoreArray[_mCurrentRound - 1] = score;
            Debug.Log($"当前回合:{_mCurrentRound}, 分数:{score}");
            _mCurrentRound++;
            if (_mCurrentRound <= MAX_ROUND)
            {
                Instantiate(birdPrefabArray[_mCurrentRound - 1]);
            }
            else
            {
                Debug.Log("所有回合结束！");
            }
        }
    }
}
