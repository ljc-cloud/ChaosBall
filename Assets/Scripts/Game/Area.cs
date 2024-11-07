using System;
using UnityEngine;

namespace ChaosBall.Game
{
    public class Area : MonoBehaviour, IComparable<Area>
    {
    
        [SerializeField] private AreaData areaData;
        [SerializeField][Range(0, 80)] private float width;
        [SerializeField][Range(0, 80)] private float height;
        [SerializeField] private Vector2 position;
    
        [SerializeField] private Transform triggerArea;
        [SerializeField] private Transform spriteArea;
    
        public int Score => areaData.score;
        public AreaData.AreaType Type => areaData.type;
    
        private void Start() 
        {
            InitArea();
        }
    
        private void OnValidate()
        {
            InitArea();
        }
        private void InitArea() {
            transform.position = new Vector3(position.x, transform.position.y, position.y);
            triggerArea.localScale = new Vector3(width, 1, height);
            var sprites = spriteArea.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < sprites.Length; i++) {
                if (sprites[i].name.Equals("t1")) {
                    sprites[i].size = new Vector2(width, 1);
                    sprites[i].transform.localPosition = new Vector3(0, 0, height / 2f - .5f);
                } else if (sprites[i].name.Equals("t2")) {
                    sprites[i].size = new Vector2(width, 1);
                    sprites[i].transform.localPosition = new Vector3(0, 0, -height / 2f + .5f);
                } else if (sprites[i].name.Equals("t3")) {
                    sprites[i].size = new Vector2(height, 1);
                    sprites[i].transform.localPosition = new Vector3(-width / 2f + .5f, 0, 0);
                } else {
                    sprites[i].size = new Vector2(height, 1);
                    sprites[i].transform.localPosition = new Vector3(width / 2f - .5f, 0, 0);
                }
                sprites[i].color = new Color(areaData.color.r, areaData.color.g, areaData.color.b, areaData.color.a);
            }
        }
    
        public int CompareTo(Area otherArea)
        {
            return Score - otherArea.Score;
        }
    }

}
