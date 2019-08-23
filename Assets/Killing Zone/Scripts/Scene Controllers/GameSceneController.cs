using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneController : MonoBehaviour
{
    [Header("Gameplay")]
    [SerializeField] Player _player;


    // Start is called before the first frame update
    void Start()
    {
        _player.OnPlayerDied += OnPlayerDied;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnPlayerDied()
    {
        //Debug.Log("Player Died");
        Invoke("ReloadGame", 3);
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
