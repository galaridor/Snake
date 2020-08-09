using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private int gameCounter = 0;
    [SerializeField]
    private int height = 19;
    [SerializeField]
    private int width = 15;
    [SerializeField]
    private int pixel = 1;

    [SerializeField]
    private SpriteRenderer terrainRenderer;
    [SerializeField]
    private GameObject terrainObject;

    public GameObject snake;
    public GameObject food;
    public GameObject tailParts;
    Sprite playerSprite;
    public Sprite Apple;
    public Sprite SnakeHead;
    public Sprite SnakeBody;

    public Color32 terrainColorPixelOne = new Color(155, 118, 83, 255);
    public Color32 terrainColorPixelTwo = new Color(207, 112, 22, 255);
    public Color32 snakeColor = new Color(0, 0, 0, 255);
    public Color32 foodColor = new Color(255, 0, 0, 255);

    bool up, down, right, left;

    [SerializeField]
    float horizontalMove = 0;
    [SerializeField]
    float verticalMove = 0;

    private bool isGameOver;

    public float moveSpeed = 0.4f;
    float timer;

    enum Direction
    {
        up, down, left, right
    }

    Direction targetDirection;
    Direction currentDirection;

    Grid snakeGrid;
    Grid appleGrid;
    Grid previusSnakeGrid;

    public Transform cameraT;

    [SerializeField]
    private int score = 0;
    public GameObject scoreText;

    public GameObject audioManager;
    public AudioSource backgroundAudio;
    public AudioSource eatAudio;
    public AudioSource wonAudio;
    public AudioSource loseAudio;

    public GameObject background;
    public GameObject wonScreen;
    public GameObject loseScreen;

    public GameObject joystick;

    public Joystick stick;

    public GameObject muteButton;
    public GameObject pauseButton;

    public Sprite muteSound;
    public Sprite unMuteSound;
    public Sprite pause;
    public Sprite unpause;

    public GameObject pauseText;

    Grid[,] terrainGrid;
    List<Grid> availableSpots = new List<Grid>();
    List<Tail> tail = new List<Tail>();

    // Start is called before the first frame update
    private void Start()
    {
        if (Application.isEditor)
        {
            joystick.SetActive(false);
        }

        Time.timeScale = 1f;

        GenerateTerrain();

        SpawnPlayer();

        PlaceCamera();

        SpawnApple();

        targetDirection = Direction.right;

        scoreText.GetComponent<TextMeshProUGUI>().text = "SCORE: " + score;

        gameCounter = PlayerPrefs.GetInt("gameCounter");
        gameCounter++;
        PlayerPrefs.SetInt("gameCounter", gameCounter);
        PlayerPrefs.Save();
    }

    // Update is called once per frame
    private void Update()
    {
        GetInputs();

        SnakeDirection();

        timer += Time.deltaTime;
        if (timer > moveSpeed)
        {
            timer = 0;
            currentDirection = targetDirection;
            MoveSnake();
        }

        if (isGameOver == true)
        {
            GameOver();
        }
    }

    public void GenerateTerrain()
    {
        terrainGrid = new Grid[width, height];

        Texture2D terrain = new Texture2D(width, height);

        terrainObject = new GameObject("Terrain");
        terrainRenderer = terrainObject.AddComponent<SpriteRenderer>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 wp = Vector3.zero;
                wp.x = x;
                wp.y = y;

                Grid g = new Grid()
                {
                    x = x,
                    y = y,
                    worldPosition = wp
                };

                terrainGrid[x, y] = g;

                availableSpots.Add(g);

                if (x % 2 != 0)
                {
                    if (y % 2 != 0)
                    {
                        terrain.SetPixel(x, y, terrainColorPixelOne);
                    }
                    else
                    {
                        terrain.SetPixel(x, y, terrainColorPixelTwo);
                    }
                }
                else
                {
                    if (y % 2 != 0)
                    {
                        terrain.SetPixel(x, y, terrainColorPixelTwo);
                    }
                    else
                    {
                        terrain.SetPixel(x, y, terrainColorPixelOne);
                    }
                }
            }
        }

        terrain.filterMode = FilterMode.Point;
        terrain.Apply();
        Rect rect = new Rect(0, 0, width, height);
        Sprite sprite = Sprite.Create(terrain, rect, Vector2.zero, 1, 0, SpriteMeshType.FullRect);
        terrainRenderer.sprite = sprite;

    }

    Sprite GenerateSprite(Color32 color)
    {
        Texture2D sprite = new Texture2D(pixel, pixel);
        sprite.SetPixel(0, 0, color);
        sprite.Apply();
        sprite.filterMode = FilterMode.Point;
        Rect rect = new Rect(0, 0, pixel, pixel);
        return Sprite.Create(sprite, rect, Vector2.one * .5f, 1, 0, SpriteMeshType.FullRect);
    }

    void SpawnPlayer()
    {
        snake = new GameObject("Snake");
        SpriteRenderer snakeRender = snake.AddComponent<SpriteRenderer>();
        //playerSprite = GenerateSprite(snakeColor);
        snakeRender.sprite = SnakeHead;
        snakeRender.sortingOrder = 1;
        int xSpawn = Random.Range(1, width - 5);
        int ySpawn = Random.Range(1, height - 1);
        snakeGrid = GetGrid(xSpawn, ySpawn);
        PlaceObject(snake, snakeGrid.worldPosition);
        snake.transform.localScale = Vector3.one * 1.3f;
        tailParts = new GameObject("tailParts");
    }

    Grid GetGrid(int x, int y)
    {
        if (x < 0 || x > width - 1 || y < 0 || y > height - 1)
        {
            return null;
        }
        return terrainGrid[x, y];
    }

    void GetInputs()
    {
        if (Application.isEditor)
        {
            up = Input.GetKeyDown(KeyCode.UpArrow);
            down = Input.GetKeyDown(KeyCode.DownArrow);
            left = Input.GetKeyDown(KeyCode.LeftArrow);
            right = Input.GetKeyDown(KeyCode.RightArrow);

        }
        else if (!Application.isEditor)
        {
            horizontalMove = stick.Horizontal;
            verticalMove = stick.Vertical;
        }
    }

    void MoveSnake()
    {
        int x = 0;
        int y = 0;

        switch (currentDirection)
        {
            case Direction.up:
                if (snake.transform.rotation.z != 0.0f)
                {
                    snake.transform.eulerAngles = new Vector3(snake.transform.eulerAngles.x, snake.transform.eulerAngles.y, 0.0f);
                }
                y = 1;
                break;
            case Direction.down:
                if (snake.transform.rotation.z != 180.0f)
                {
                    snake.transform.eulerAngles = new Vector3(snake.transform.eulerAngles.x, snake.transform.eulerAngles.y, 180.0f);
                }
                y = -1;
                break;
            case Direction.left:
                if (snake.transform.rotation.z != 90.0f)
                {
                    snake.transform.eulerAngles = new Vector3(snake.transform.eulerAngles.x, snake.transform.eulerAngles.y, 90.0f);
                }
                x = -1;
                break;
            case Direction.right:
                if (snake.transform.rotation.z != -90.0f)
                {
                    snake.transform.eulerAngles = new Vector3(snake.transform.eulerAngles.x, snake.transform.eulerAngles.y, -90.0f);
                }
                x = 1;
                break;
        }

        Grid moveSpot = GetGrid(snakeGrid.x + x, snakeGrid.y + y);
        if (moveSpot == null)
        {
            isGameOver = true;
            PlayerPrefs.SetInt("LastScore", score);
            PlayerPrefs.Save();
        }
        else
        {
            if (IsTailHit(moveSpot))
            {
                isGameOver = true;
                PlayerPrefs.SetInt("LastScore", score);
                PlayerPrefs.Save();
            }
            else
            {
                bool AteFood = false;

                if (moveSpot == appleGrid)
                {
                    AteFood = true;
                }

                Grid previuosGrid = snakeGrid;
                availableSpots.Add(previuosGrid);

                if (AteFood)
                {
                    eatAudio.enabled = true;
                    eatAudio.Play();
                    score++;
                    if (score == 20)
                    {
                        isGameOver = true;
                        PlayerPrefs.SetInt("LastScore", score);
                        PlayerPrefs.Save();
                    }
                    tail.Add(GenerateTail(previuosGrid.x, previuosGrid.y));
                    availableSpots.Remove(previuosGrid);
                    scoreText.GetComponent<TextMeshProUGUI>().text = "SCORE: " + score;
                }

                MoveTail();

                PlaceObject(snake, moveSpot.worldPosition);
                snakeGrid = moveSpot;
                availableSpots.Add(snakeGrid);

                if (AteFood)
                {
                    if (moveSpeed > 0.021f)
                    {
                        moveSpeed -= 0.020f;
                    }
                    if (availableSpots.Count > 0)
                    {
                        RandomFoodPlace();
                    }
                    else
                    {
                        isGameOver = true;
                        PlayerPrefs.SetInt("LastScore", score);
                        PlayerPrefs.Save();
                    }
                }
            }
        }
    }

    void SetDirection(Direction d)
    {
        if (!IsMovePossible(d))
        {
            targetDirection = d;
        }
    }

    void SnakeDirection()
    {
        if (up || verticalMove > 0.5f)
        {
            SetDirection(Direction.up);
        }
        else if (down || verticalMove < -0.5f)
        {
            SetDirection(Direction.down);
        }
        else if (left || horizontalMove < -0.5f)
        {
            SetDirection(Direction.left);
        }
        else if (right || horizontalMove > 0.5f)
        {
            SetDirection(Direction.right);
        }
    }

    void PlaceCamera()
    {
        Grid g = GetGrid(width / 2, height / 2);
        Vector3 p = g.worldPosition;
        p += Vector3.one * 0.5f;
        cameraT.position = p;
    }

    void SpawnApple()
    {
        food = new GameObject("Apple");
        SpriteRenderer appleRenderer = food.AddComponent<SpriteRenderer>();
        //appleRenderer.sprite = GenerateSprite(foodColor);
        appleRenderer.sprite = Apple;
        appleRenderer.sortingOrder = 1;
        RandomFoodPlace();
    }

    void RandomFoodPlace()
    {
        int random = Random.Range(0, availableSpots.Count);
        Grid g = availableSpots[random];
        PlaceObject(food, g.worldPosition);
        appleGrid = g;
    }

    Tail GenerateTail(int x, int y)
    {
        Tail tail = new Tail();
        tail.grid = GetGrid(x, y);
        tail.obj = new GameObject();
        tail.obj.transform.parent = tailParts.transform;
        tail.obj.transform.position = tail.grid.worldPosition;
        tail.obj.transform.localScale = Vector3.one * .8f;
        SpriteRenderer r = tail.obj.AddComponent<SpriteRenderer>();
        //r.sprite = playerSprite;
        r.sprite = SnakeBody;
        r.sortingOrder = 1;
        return tail;
    }

    void MoveTail()
    {
        Grid previousGrid = null;

        for (int i = 0; i < tail.Count; i++)
        {
            Tail t = tail[i];
            availableSpots.Add(t.grid);

            if(i == 0)
            {
                previousGrid = t.grid;
                t.grid = snakeGrid;
            }
            else
            {
                Grid prev = t.grid;
                t.grid = previousGrid;
                previousGrid = prev;
            }
            availableSpots.Remove(t.grid);
            PlaceObject(t.obj, t.grid.worldPosition);
        }
    }

    void PlaceObject(GameObject obj, Vector3 pos)
    {
        pos += Vector3.one * .5f;
        obj.transform.position = pos;
    }

    bool IsMovePossible(Direction dir)
    {
        switch (dir)
        {
            default:
            case Direction.up:
                if (currentDirection == Direction.down)
                    return true;
                else
                    return false;
            case Direction.down:
                if (currentDirection == Direction.up)
                    return true;
                else
                    return false;
            case Direction.left:
                if (currentDirection == Direction.right)
                    return true;
                else
                    return false;
            case Direction.right:
                if (currentDirection == Direction.left)
                    return true;
                else
                    return false;
        }
    }

    bool IsTailHit(Grid g)
    {
        for (int i = 0; i < tail.Count; i++)
        {
            if(tail[i].grid == g)
            {
                return true;
            }
        }
        return false;
    }

    public void GameOver()
    {
        backgroundAudio.enabled = false;
        background.SetActive(true);
        joystick.SetActive(false);
        Time.timeScale = 0.0f;

        if (score != 20)
        {
            LoseScreen();
        }
        else if(score == 20)
        {
            WonScreen();
        }
    }

    void WonScreen()
    {
        wonAudio.enabled = true;
        wonScreen.SetActive(true);
    }

    void LoseScreen()
    {
        loseAudio.enabled = true;
        loseScreen.SetActive(true);
    }

    public void Back()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void MuteAudio()
    {
        if(audioManager.activeSelf)
        {
            backgroundAudio.Pause();
            audioManager.SetActive(false);
            muteButton.GetComponent<Image>().sprite = unMuteSound;
        }
        else if (!audioManager.activeSelf)
        {
            backgroundAudio.Play();
            audioManager.SetActive(true);
            muteButton.GetComponent<Image>().sprite = muteSound;
        }

    }

    public void Pause()
    {
        if (Time.timeScale == 0.0f)
        {
            pauseText.SetActive(false);
            Time.timeScale = 1.0f;
            background.SetActive(false);
            pauseButton.GetComponent<Image>().sprite = pause;
        }
        else if (Time.timeScale == 1.0f)
        {
            pauseText.SetActive(true);
            Time.timeScale = 0.0f;
            background.SetActive(true);
            pauseButton.GetComponent<Image>().sprite = unpause;
        }
    }
}