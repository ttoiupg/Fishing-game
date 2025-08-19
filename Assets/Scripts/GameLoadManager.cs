using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class GameLoadManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private LootManager lootManager;
    [SerializeField] private VisualFXManager visualFXManager;
    [SerializeField] private SoundFXManger soundFXManger;
    [SerializeField] private DataPersistenceManager dataPersistenceManager;
    [SerializeField] private GameObject musicManager;
    
    [SerializeField] private ViewManager viewManager;
    [SerializeField] private GameObject fishingCanvas;
    [SerializeField] private GameObject cameras;
    [SerializeField] private BucketGoddessController bucketGoddessController;
    [SerializeField] private GameObject eventSystem;
    [SerializeField] private FishCardHandler fishCardHandler;
    [SerializeField] private GameObject globalVolume;
    [SerializeField] private GameObject sea;
    [SerializeField] private GameObject map;

    [SerializeField] private Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
}
