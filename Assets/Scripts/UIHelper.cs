using UnityEngine;
using UnityEngine.UI;

public class UIHelper : MonoBehaviour
{
    //UI references
    [SerializeField]
    private TMPro.TMP_Text scoreText;
    [SerializeField]
    private TMPro.TMP_Text livesText;
    [SerializeField]
    private GameObject gameEndPanel;
    [SerializeField]
    private Button restartButton;
    [SerializeField]
    private TMPro.TMP_Text restartText;
    [SerializeField]
    private TMPro.TMP_Text endScoreText;
    [SerializeField]
    private TMPro.TMP_Text endText;

    public void UpdateLivesAndScores(int lives, int score)
    {
        livesText.text = "Lives: " + lives.ToString();
        scoreText.text = "Score: " + score.ToString();
    }

    public void ShowEndPanel(bool win, bool isServer, int score)
    {
        gameEndPanel.SetActive(true);

        gameEndPanel.SetActive(true); //Show game end UI
        if (win)
        {
            endText.text = "You Won";
        }
        else
        {
            endText.text = "You Lost";
        }
        if (!isServer)
        {
            restartButton.interactable = false;
            restartText.text = "Restart On Host";
        }
        else
        {
            restartButton.interactable = true;
            restartText.text = "Restart";
        }
        endScoreText.text = "Score: " + score.ToString();
    }

    public void HideEndPanel()
    {
        gameEndPanel.SetActive(false);
    }
}
