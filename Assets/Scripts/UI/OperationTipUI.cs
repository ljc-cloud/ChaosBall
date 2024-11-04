using ChaosBall.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChaosBall.UI
{
    public class OperationTipUI : MonoBehaviour
    {
        [SerializeField] private Image moveLeftImage;
        [SerializeField] private Image moveRightImage;
        [SerializeField] private Text moveLeftText;
        [SerializeField] private Text moveRightText;
        [SerializeField] private Text launchText;
        [SerializeField] private Text unlaunchText;
        
        [SerializeField] private Sprite leftArrow;
        [SerializeField] private Sprite rightArrow;
        [SerializeField] private Sprite buttonBackground;


        private BallAction _action;
        
        private void Start()
        {
            _action = new BallAction();
            GameManager.Instance.OnChangePlayer += ChangeOperationUI;
        }

        private void ChangeOperationUI(PlayerEnum player)
        {
            if (player == PlayerEnum.Player1)
            {
                // moveLeftImage.sprite = buttonBackground;
                // moveRightImage.sprite = buttonBackground;
                // moveLeftText.gameObject.SetActive(true);
                // moveRightText.gameObject.SetActive(true);
                moveLeftText.text = _action.Player1.LR.controls[0].name.ToUpper();
                moveRightText.text = _action.Player1.LR.controls[1].name.ToUpper();
                launchText.text = _action.Player1.Launch.controls[0].name.ToUpper();
                unlaunchText.text = _action.Player1.UnLaunch.controls[0].name.ToUpper();
            }
            else
            {
                // moveLeftText.gameObject.SetActive(false);
                // moveRightText.gameObject.SetActive(false);
                // moveLeftImage.sprite = leftArrow;
                // moveRightImage.sprite = rightArrow;
                // moveLeftImage.color = Color.white;
                moveLeftText.text = "⬅";
                moveRightText.text = "➡";
                launchText.text = "0";
                unlaunchText.text = ".";
                // launchText.text = _action.Player2.Launch.controls[0].name.ToUpper();
                // unlaunchText.text = _action.Player2.UnLaunch.controls[0].name.ToUpper();
            }
        }
    }
}
