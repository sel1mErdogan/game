[System.Serializable]
public class GameData
{
    // --- Kayıt Menüsü için Bilgiler ---
    public string saveName; // "My Kingdom A" gibi oyuncunun verdiği isim
    public string lastSaved; // "18/08/2025" gibi son kayıt tarihi

    // --- Asıl Oyun Verileri ---
    public int currentDay;
    // Gelecekte buraya kaydedilecek diğer her şey eklenecek:
    // public int crumbAmount;
    // public List<Vector3> buildingPositions;
    // public List<BuildingData> buildingTypes;

    // Yeni, boş bir oyun verisi oluşturmak için kullanılır.
    public GameData()
    {
        this.saveName = "";
        this.lastSaved = "";
        this.currentDay = 1; // Yeni oyun her zaman 1. günden başlar.
    }
}