using DefaultNamespace;
using UnityEngine;

public class ExitClickedScript : ButtonClicked
{
    public override void Clicked()
    {
        Application.Quit();
    }
}