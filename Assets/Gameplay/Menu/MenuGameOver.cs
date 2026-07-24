using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuGameOver : MonoBehaviour
{
    [Header("Nome da cena principal do jogo")]
    public string nomeCenaJogo = "SampleScene"; // Troque para o nome da sua cena principal

    public void ReiniciarJogo()
    {
        // Carrega a cena do jogo novamente do zero
        SceneManager.LoadScene(nomeCenaJogo);
    }
}