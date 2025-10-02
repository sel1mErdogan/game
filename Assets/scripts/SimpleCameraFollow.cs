using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    // Inspector'dan takip edeceğimiz objeyi (player) buraya sürükleyeceğiz
    public Transform target;

    // Kamera ile oyuncu arasındaki mesafeyi korumak için
    public Vector3 offset;

    void LateUpdate()
    {
        // Eğer bir hedefimiz varsa...
        if (target != null)
        {
            // Kameranın pozisyonunu, her frame'de oyuncunun pozisyonu + aradaki mesafe olarak ayarla
            transform.position = target.position + offset;
        }
    }
}