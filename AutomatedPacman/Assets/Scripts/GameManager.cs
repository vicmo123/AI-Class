using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform pellets;

    public Text gameOverText;
    public Text scoreText;
    public Text livesText;

    public int ghostMultiplier { get; private set; } = 1;
    public int score { get; private set; }
    public int lives { get; private set; }

    private void Start()
    {
        NewGame();
    }

    private void Update()
    {
        if (lives <= 0 && Input.anyKeyDown) {
            NewGame();
        }

        SetTargetForPacman();
        GhostToClose();
        PlayerBehaviourManager();
    }

    private void NewGame()
    {
        SetScore(0);
        SetLives(3);
        NewRound();
    }

    private void NewRound()
    {
        gameOverText.enabled = false;

        foreach (Transform pellet in pellets) {
            pellet.gameObject.SetActive(true);
        }

        ResetState();
    }

    private void ResetState()
    {
        for (int i = 0; i < ghosts.Length; i++) {
            ghosts[i].ResetState();
        }

        pacman.ResetState();
    }

    private void GameOver()
    {
        gameOverText.enabled = true;

        for (int i = 0; i < ghosts.Length; i++) {
            ghosts[i].gameObject.SetActive(false);
        }

        pacman.gameObject.SetActive(false);
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
        livesText.text = "x" + lives.ToString();
    }

    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString().PadLeft(2, '0');
    }

    public void PacmanEaten()
    {
        pacman.DeathSequence();

        SetLives(lives - 1);

        if (lives > 0) {
            Invoke(nameof(ResetState), 3f);
        } else {
            GameOver();
        }
    }

    public void GhostEaten(Ghost ghost)
    {
        int points = ghost.points * ghostMultiplier;
        SetScore(score + points);

        ghostMultiplier++;
    }

    public void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false);

        SetScore(score + pellet.points);

        if (!HasRemainingPellets())
        {
            pacman.gameObject.SetActive(false);
            Invoke(nameof(NewRound), 3f);
        }
    }

    public void PowerPelletEaten(PowerPellet pellet)
    {
        for (int i = 0; i < ghosts.Length; i++) {
            ghosts[i].frightened.Enable(pellet.duration);
        }

        PelletEaten(pellet);
        CancelInvoke(nameof(ResetGhostMultiplier));
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);
    }

    private bool HasRemainingPellets()
    {
        foreach (Transform pellet in pellets)
        {
            if (pellet.gameObject.activeSelf) {
                return true;
            }
        }

        return false;
    }

    private void ResetGhostMultiplier()
    {
        ghostMultiplier = 1;
    }

    private void SetTargetForPacman()
    {
        float minMagnitude = float.MaxValue;
        Transform _target = null;

        foreach (var ghost in ghosts)
        {
            if (!ghost.home.enabled && ghost.frightened.enabled)
            {
                if((ghost.transform.position - pacman.transform.position).magnitude <= minMagnitude)
                {
                    minMagnitude = (ghost.transform.position - pacman.transform.position).magnitude;
                    _target = ghost.transform;
                }
            }
        }

        //if it is null, the will stop chasing the ghosts(When frightened phase is finished)
        pacman.target = _target;
    }

    private void PlayerBehaviourManager()
    {
        if(pacman.target != null)
        {
            //target found so start chase
            pacman.eat.enabled = false;
            pacman.avoid.enabled = false;
            pacman.chase.enabled = true;
        }
        else if(pacman.avoid.ghostIsToClose == true)
        {
            //non frightened ghost to close so avoid
            pacman.eat.enabled = false;
            pacman.chase.enabled = false;
            pacman.avoid.enabled = true;
        }
        else
        {
            //nothing so go te eating pellets
            pacman.chase.enabled = false;
            pacman.avoid.enabled = false;
            pacman.eat.enabled = true;
        }
    }

    private void GhostToClose()
    {
        bool isClose = false;

        foreach (var ghost in ghosts)
        {
            if (!ghost.home.enabled && !ghost.frightened.enabled)
            {
                if (Mathf.Abs((ghost.transform.position - pacman.transform.position).magnitude) < pacman.avoid.ghostMinDistance)
                {
                    isClose = true;
                    pacman.avoid.currentEnemey = ghost;
                }
            }
        }

        pacman.avoid.ghostIsToClose = isClose;
    }
}
