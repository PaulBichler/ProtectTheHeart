using DG.Tweening;
using UnityEngine;

public class CameraMoveMenu : MonoBehaviour
{
    public void MoveToCredits()
    {
        transform.DOMoveX(-15f, 1);
    }

    public void MoveToMenu()
    {
        transform.DOMoveX(0f, 1);
    }
    
    public void MoveToHowToPlay1()
    {
        transform.DOMoveX(17f, 1);
    }
    
    public void MoveToHowToPlay2()
    {
        transform.DOMoveX(28f, 1);
    }
}