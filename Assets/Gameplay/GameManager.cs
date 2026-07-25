using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private float volume = 0.02f;

    public float hitTimeBuffer;
    public float hitTimePerfect;

    private int ultimoFrameTocado = -1;
    private AudioClip ultimoClipTocado;

    [Header("Sistema de Vidas")]
    public int vidas = 3;
    public string nomeCenaGameOver = "GameOver";
    
    [Header("Visual das Vidas")]
    public GameObject[] coracoesVisual;

    public AudioClip[] audios;
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
    public void TocarAudio(AudioClip clip,float SpB)
    {
        if(clip != null)
        {
            if (Time.frameCount == ultimoFrameTocado && clip == ultimoClipTocado)
            {
                return; 
            }

            GameObject temp = new GameObject("SomparaTocar_" + clip.name);

            AudioSource audio = temp.AddComponent<AudioSource>();
            audio.playOnAwake = false;

            audio.clip = clip;
            audio.volume = volume;
            audio.pitch = 1/SpB;
            ultimoFrameTocado = Time.frameCount;
            ultimoClipTocado = clip;
            audio.PlayOneShot(clip);
            float duracaoReal = clip.length / audio.pitch;
            Destroy(audio, duracaoReal);
        }
        
    }
}
