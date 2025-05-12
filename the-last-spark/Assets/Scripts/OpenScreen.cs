using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenScreen : MonoBehaviour
{
    public GameObject helpBox;

    // Start is called before the first frame update
    void Start()
    {
        helpBox.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            StartCoroutine(HandleStartGame());
        }
    }

    private IEnumerator HandleStartGame()
    {
        helpBox.SetActive(true);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("2.GameScene");
    }
}
