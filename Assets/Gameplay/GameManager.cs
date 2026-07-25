using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float hitTimeBuffer;
    public float hitTimePerfect;

    [Header("Sistema de Vidas")]
    public int vidas = 3;
    public string nomeCenaGameOver = "GameOver";
    
    [Header("Visual das Vidas")]
    public GameObject[] coracoesVisual; 
    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void PerderVida()
    {
        vidas--;
        
        // Desativa a imagem correspondente. Ex: Se as vidas caem de 3 para 2, desativa o coração do índice [2].
        if (vidas >= 0 && vidas < coracoesVisual.Length)
        {
            coracoesVisual[vidas].SetActive(false);
        }

        if (vidas <= 0)
        {
            SceneManager.LoadScene(nomeCenaGameOver);
        }
    }
}
