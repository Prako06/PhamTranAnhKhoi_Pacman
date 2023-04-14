using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Ghost[] ghosts;
    public Pacman pacman;
    public List<Pellet> pellets { get; private set; }

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
        pellets = new List<Pellet>();
    }

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (lives <= 0 && Input.GetKeyDown(KeyCode.D))
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
        if (Input.GetKeyDown(KeyCode.D))
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
        if (SaveManager.instance.hasLoaded)
        {
            score = SaveManager.instance.activeSave.score;
            highScore = SaveManager.instance.activeSave.highscore;
            lives = SaveManager.instance.activeSave.lives;
            pacman.transform.position = SaveManager.instance.activeSave.pacMan;
            for (int i = 0; i < pellets.Count; i++)
            {
                if (SaveManager.instance.activeSave.pellets[i] == 0)
                {
                    pellets[i].gameObject.SetActive(false);
                }
            }
            for (int i = 0; i < ghosts.Length; i++)
            {
                ghosts[i].transform.position = SaveManager.instance.activeSaveGhost.saveGhosts[i].ghostsPosition;
                ghosts[i].initialBehavior = SaveManager.instance.activeSaveGhost.saveGhosts[i].ghostsBehavior;
                ghosts[i].movement.nextDirection = SaveManager.instance.activeSaveGhost.saveGhosts[i].nextDirection;
                ghosts[i].movement.direction = SaveManager.instance.activeSaveGhost.saveGhosts[i].CurrenDirection;
            }
        }
        else
        {
            SaveManager.instance.activeSave.score = score;
            SaveManager.instance.activeSave.pacMan = pacman.transform.position;
            SaveManager.instance.activeSave.lives = lives;
            List<int> pelletLists = new List<int>();
            foreach (var pellet in pellets)
            {
                if (pellet.gameObject.activeSelf == false)
                {
                    pelletLists.Add(0);
                }
                else
                {
                    pelletLists.Add(1);
                }
            }
            SaveManager.instance.activeSave.pellets = pelletLists;
            for (int i = 0; i < ghosts.Length; i++)
            {
                SaveManager.instance.activeSaveGhost.saveGhosts[i].ghostsBehavior = ghosts[i].initialBehavior;
            }
        }
        highScoreText.text = "High Score: " + highScore.ToString();
    }

    private void NewRound()
    {
        audioSiren.Play();
        gameOverText.enabled = false;
        gameReady.enabled = false;
        foreach (Pellet pellet in pellets)
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
        gameReady.enabled = false;

        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].gameObject.SetActive(false);
        }
        pacman.gameObject.SetActive(false);

        if (score > highScore)
        {
            highScore = score;
            SaveManager.instance.activeSave.highscore = highScore;
            SaveManager.instance.Save();
        }
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
        if (SaveManager.instance.hasLoaded)
        {
            lives = SaveManager.instance.activeSave.lives;
        }
        else
        {
            SaveManager.instance.activeSave.lives = lives;
        }
        livesText.text = "x" + lives.ToString();
    }

    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString().PadLeft(2, '0');

        if (SaveManager.instance.hasLoaded)
        {
            score = SaveManager.instance.activeSave.score;
        }
        else
        {
            SaveManager.instance.activeSave.score = score;
        }
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
        List<int> pelletLists = new List<int>();
        foreach (var pellet1 in pellets)
        {
            if (pellet1.gameObject.activeSelf == false)
            {
                pelletLists.Add(0);
            }
            else
            {
                pelletLists.Add(1);
            }
        }
        SaveManager.instance.activeSave.pellets = pelletLists;
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
        foreach (Pellet pellet in pellets)
        {
            if (pellet.gameObject.activeSelf)
            {
                return true;
            }
        }

        return false;
    }

    public void ResetGhostMultiplier()
    {
        ghostMultiplier = 1;
    }
}
