using UnityEditor;
using UnityEditor.SceneManagement;

// Bu script, Unity editörü her açıldığında otomatik olarak çalışır.
[InitializeOnLoad]
public static class StartScenePlayer
{
    // BURAYI KONTROL ET: Başlatıcı sahnenin tam yolu bu olmalı.
    // Eğer Scenes klasörünün içinde değilse, doğru yolu yaz.
    private const string START_SCENE_PATH = "Assets/Scenes/Initializer.unity";

    // Static constructor, bu kodun editör başladığında bir kere çalışmasını sağlar.
    static StartScenePlayer()
    {
        // Play tuşuna basıldığında veya durdurulduğunda haberimiz olsun diye bir dinleyici ekliyoruz.
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        // Eğer Play tuşuna yeni basıldıysa (ve editör "Edit" modundan çıkıyorsa)...
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            // Oyunu başlatmadan önce, eğer mevcut sahnede kaydedilmemiş değişiklikler varsa, kullanıcıya sor.
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            
            // Her şeyden önce, bizim belirlediğimiz başlangıç sahnesini aç.
            EditorSceneManager.OpenScene(START_SCENE_PATH);
        }
    }
}