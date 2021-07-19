using DefaultNamespace;

public class StartClickedScript : ButtonClicked
{
    public override void Clicked()
    {
        OurSceneManager.LoadLobby();
    }
}