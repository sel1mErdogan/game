using UnityEngine;
using UnityEngine.Events;

public class D : MonoBehaviour
{
    [Header("Zaman Ayarları")]
    [SerializeField] private float dayDurationInSeconds = 120f;

    [Header("Işık Ayarları")]
    [SerializeField] private Light sunLight;
    // ... (Diğer renk ayarların aynı kalacak) ...

    [Header("Olaylar (Events)")]
    public UnityEvent OnDayStart;
    public UnityEvent OnNightStart;

    // --- DÜZELTME BURADA ---
    // Değişkeni public hale getirdik ki GameManager ona erişebilsin.
    public int CurrentDay = 1;

    public float timeOfDay; // private float timeOfDay; şeklindeydi, public yaptık.
    private bool isNight = false;


    void Update()
    {
        if (sunLight == null) return;

        timeOfDay += Time.deltaTime / dayDurationInSeconds;
        timeOfDay %= 1;

        float sunAngle = timeOfDay * 360f;
        sunLight.transform.rotation = Quaternion.Euler(new Vector3(sunAngle - 90, 170, 0));

        UpdateLighting();
        CheckDayNightTransition();
    }

    private void CheckDayNightTransition()
    {
        bool currentlyIsNight = Vector3.Dot(sunLight.transform.forward, Vector3.up) > 0;

        if (currentlyIsNight && !isNight)
        {
            isNight = true;
            OnNightStart?.Invoke();
        }
        else if (!currentlyIsNight && isNight)
        {
            isNight = false;
            CurrentDay++; // Yeni gün başladığında sayacı artır
            OnDayStart?.Invoke();
        }
    }

    // Bu fonksiyon olduğu gibi kalabilir.
    private void UpdateLighting()
    {
        float dotProduct = Vector3.Dot(sunLight.transform.forward, Vector3.down);
        RenderSettings.ambientLight = Color.Lerp(new Color(0.1f, 0.1f, 0.1f), new Color(0.5f, 0.5f, 0.5f), dotProduct);

        if (dotProduct > 0)
        {
            sunLight.color = Color.Lerp(new Color(1f, 0.6f, 0.4f), Color.white, dotProduct);
        }
        else
        {
            sunLight.color = new Color(0.1f, 0.1f, 0.3f);
        }
    }
    public void LoadTimeOfDay(float loadedTime)
    {
        this.timeOfDay = loadedTime;
    }
}