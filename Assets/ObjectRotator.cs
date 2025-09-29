using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    // Hızı Inspector'dan ayarlayabilmek için public yapıyoruz
    public float donusHizi = 30f;

    void Update()
    {
        // Bu objeyi, kendi Y ekseni (yukarı yönü) etrafında her saniye döndür
        transform.Rotate(Vector3.up, donusHizi * Time.deltaTime);
    }
}