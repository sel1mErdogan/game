using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    void Start()
    {
        // Bu sahne açılır açılmaz, bir sonraki sahne olan "AnaMenu" sahnesini yükle.

        // Eğer ana menü sahnenin adı farklıysa, tırnak içindeki ismi onunla değiştir.

        SceneManager.LoadScene("AnaMenu");
    }
}