using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipalManager : MonoBehaviour
{
    [Header("Nome da cena do jogo")]
    public string cenaDoJogo = "SampleScene";

    public void JogarFaseNormal()
    {
        // Salva o valor 0 (Falso) para o modo infinito
        PlayerPrefs.SetInt("ModoInfinito", 0);
        PlayerPrefs.Save();
        
        SceneManager.LoadScene(cenaDoJogo);
    }

    public void JogarModoInfinito()
    {
        // Salva o valor 1 (Verdadeiro) para o modo infinito
        PlayerPrefs.SetInt("ModoInfinito", 1);
        PlayerPrefs.Save();
        
        SceneManager.LoadScene(cenaDoJogo);
    }
}