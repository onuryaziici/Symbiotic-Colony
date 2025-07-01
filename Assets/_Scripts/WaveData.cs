// WaveData.cs
using UnityEngine;

[System.Serializable]
public class EnemyGroup
{
    public GameObject enemyPrefab;
    public int count;
}

[CreateAssetMenu(fileName = "New Wave", menuName = "Symbiotic Colony/Wave Data")]
public class WaveData : ScriptableObject
{
    public EnemyGroup[] enemyGroups; // Bu dalgada hangi düşmanlardan kaç tane geleceği
    public float timeBetweenSpawns = 0.5f; // Düşmanların doğma aralığı
}