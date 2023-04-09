using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform pellets;

    public Text gameOverText;
    public Text gameReady;
    public Text scoreText;
    public Text highScoreText;
    public Text livesText;
    public AudioSource audioBeginner;
    public AudioSource audioMunch;
    public AudioSource audioSiren;
    public AudioSource audioDeath;
    public AudioSource audioEatGhost;
    public AudioSource audioVictory;

    public int ghostMultiplier { get; private set; } = 1;
    public int score { get; private set; }
    public int highScore { get; private set; }
    public int lives { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartGame();

    }

    private void Update()
    {
        if (lives <= 0 && Input.anyKeyDown)
        {
            NewGame();
        }
    }

    private void StartGame()
    {
        audioVictory.Stop();
        audioEatGhost.Stop();
        audioDeath.Stop();
        audioSiren.Stop();
        audioMunch.Stop();
        audioBeginner.Play();
        if (Input.anyKeyDown)
        {
            NewGame();
        }
        pacman.gameObject.SetActive(false);
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].gameObject.SetActive(false);
        }

        gameReady.enabled = true;
        gameOverText.enabled = false;
    }

    private void NewGame()
    {
        audioDeath.Stop();
        audioBeginner.Stop();
        audioEatGhost.Stop();
        audioSiren.Play();
        SetScore(0);
        SetLives(3);
        NewRound();
    }

    private void NewRound()
    {
        audioSiren.Play();
        gameOverText.enabled = false;

        foreach (Transform pellet in pellets)
        {
            pellet.gameObject.SetActive(true);
        }

        ResetState();
    }

    private void ResetState()
    {
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].ResetState();
        }

        pacman.ResetState();
    }

    private void GameOver()
    {
        audioSiren.Stop();
        audioVictory.Stop();
        audioDeath.Play();
        gameOverText.enabled = true;

        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].gameObject.SetActive(false);
        }
        if (score > highScore)
        {
            highScore = score;
            highScoreText.text = highScore.ToString();

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
        audioDeath.Play();

        pacman.DeathSequence();

        SetLives(lives - 1);

        if (lives > 0)
        {
            Invoke(nameof(ResetState), 3f);
        }
        else
        {
            GameOver();
        }
    }

    public void GhostEaten(Ghost ghost)
    {
        audioEatGhost.Play();

        int points = ghost.points * ghostMultiplier;
        SetScore(score + points);

        ghostMultiplier++;
    }

    public void PelletEaten(Pellet pellet)
    {
        audioMunch.Play();

        pellet.gameObject.SetActive(false);

        SetScore(score + pellet.points);

        if (!HasRemainingPellets())
        {
            pacman.gameObject.SetActive(false);
            Invoke(nameof(NewRound), 5f);
            audioVictory.Play();
            audioSiren.Stop();
        }
    }

    public void PowerPelletEaten(PowerPellet pellet)
    {
        for (int i = 0; i < ghosts.Length; i++)
        {
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
            if (pellet.gameObject.activeSelf)
            {
                return true;
            }
        }

        return false;
    }

    private void ResetGhostMultiplier()
    {
        ghostMultiplier = 1;
    }

}
