using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class GameManage : MonoBehaviour {
    // todo: update with spawn position
    public float xPos = 3;
    public float zPos = 3;

    public Paint dicePaint;
    private bool progressIncremented = false;

    [SerializeField] private GameObject victoryMenu;

    public enum Level {
        Target,
        Abstract,
        Greenfields,
        Smiley,
        Debug,
        Window,
        Bee,
        Swim,
        Cake,
        Default,
    }

    [SerializeField] private Level stage;
    [SerializeField] private SFX_Manager sfx;

    private void Start() {
        canvasController = GameObject.Find("Canvas").GetComponent<Canvas_Controller>();
        if (GameObject.Find("Dice") != null) {
            dicePaint = GameObject.Find("Dice").GetComponent<Paint>();
        }
        else {
            dicePaint = GameObject.Find("Level-Select-Dice").GetComponent<Paint>();
        }
        setup_scene(stage);
    }

    private void Update() {
        if (!sfx) {
            sfx = GameObject.Find("SFX").GetComponent<SFX_Manager>();
        }
    }

    public bool InBounds(Vector3 dir) {
        var newXPos = xPos + dir.x;
        if (newXPos > 6 || newXPos < 0) {
            return false;
        }

        var newZPos = zPos + dir.z;
        if (newZPos > 6 || newZPos < 0) {
            return false;
        }

        return true;
    }

    public float correctCells = 0;

    private char[,] currentStage;
    private char[,] progressingStage;

    private Canvas_Controller canvasController;

    public Char GetColor(Canvas_Piece piece) {
        Material material = piece.GetComponent<MeshRenderer>().material;
        // todo: how to compare the materials to each other
        // Green
        if (material.name.Contains(canvasController.GreenCanvas.name)) {
            return 'g';
        }

        // Yellow
        if (material.name.Contains(canvasController.YellowCanvas.name)) {
            return 'y';
        }

        // Red
        if (material.name.Contains(canvasController.RedCanvas.name)) {
            return 'r';
        }

        // Blue
        if (material.name.Contains(canvasController.BlueCanvas.name)) {
            return 'u';
        }

        // Orange
        if (material.name.Contains(canvasController.OrangeCanvas.name)) {
            return 'o';
        }

        // Pink
        if (material.name.Contains(canvasController.PinkCanvas.name)) {
            return 'i';
        }

        // Purple
        if (material.name.Contains(canvasController.PurpleCanvas.name)) {
            return 'p';
        }

        // Black
        if (material.name.Contains(canvasController.BlackCanvas.name)) {
            return 'b';
        }

        return 'd';
    }

    // sound helper function
    private bool TheSamePaint(char c, Vector2 position) {
        return progressingStage[(int) position.y, (int) position.x] == c;
    }

    // sound helper function
    private bool CorrectPaint(char c, Vector2 position) {
        return currentStage[(int) position.y, (int) position.x] == c;
    }

    public void CheckCell(Canvas_Piece piece) {
        Vector2 position = piece.GetPosition();
        // get he color Char to update progress array
        Char c = GetColor(piece);
        // correct
        if (!TheSamePaint(c, position)) {
            if (CorrectPaint(c, position)) {
                sfx.PlayGoodColor();
            }
            else {
                sfx.PlayBadColor();
            }
        }

        // update progress 
        // Debug.Log("Changing (" + position.x + "," + position.y + ") to color " + c);
        progressingStage[(int) position.y, (int) position.x] = c;
        // check the stage if it is complete
        bool stageComplete = true;
        // Debug.Log("-----------------------------------------");
        for (int y = 0; y <= 6; y++) {
            for (int x = 0; x <= 6; x++) {
                // couldo: improve what is incorrect message
                if (progressingStage[y, x] != currentStage[y, x]) {
                    // Debug.Log("NOT EQUAL (" + x + "," + y + ") | correct: " + currentStage[y, x] + " progress: " +
                              // progressingStage[y, x]);
                    stageComplete = false;
                }
            }
        }

        if (stageComplete && !progressIncremented) {
            var pprog = GameObject.Find("PlayerProgress").GetComponent<PlayerProgress>();
            progressIncremented = true;
            if (!pprog.IsLevelCompleded(SceneManager.GetActiveScene().buildIndex) && pprog.getStagesComplet < 7) {
                pprog.CompleteLevel(SceneManager.GetActiveScene().buildIndex);
                pprog.IncrementStagesComplete();
            }

            // play win animation / show win UI here
            victoryMenu.SetActive(true);
            victoryMenu.GetComponent<Victory_Screen>().Win();
        }
    }

    public void setup_scene(Level level) {
        char[,] stage = new char[7, 7];
        char[] dice = new char[6];

        switch (level) {
            case Level.Target:
                stage = target_stage;
                dice = target_dice;
                break;
            case Level.Abstract:
                stage = abstract_stage;
                dice = abstract_dice;
                break;
            case Level.Greenfields:
                stage = greenfield_stage;
                dice = greenfield_dice;
                break;
            case Level.Smiley:
                stage = smiley_stage;
                dice = smiley_dice;
                break;
            case Level.Debug:
                stage = debug_stage;
                dice = debug_dice;
                break;
            case Level.Window:
                stage = window_stage;
                dice = window_dice;
                break;
            case Level.Bee:
                stage = bee_stage;
                dice = bee_dice;
                break;
            case Level.Swim:
                stage = swim_stage;
                dice = swim_dice;
                break;
            case Level.Cake:
                stage = cake_stage;
                dice = cake_dice;
                break;
            case Level.Default:
                stage = default_stage;
                dice = GetRandomDice();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }

        // stage
        currentStage = stage;
        // Stage the player is progressing. Initialized to default values. 
        progressingStage = new char[7, 7] {
            {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
            {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
            {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
            {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
            {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
            {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
            {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
        };
        // assign color to the dice
        int i = 1;
        foreach (char c in dice) {
            Color color = new Color();
            switch (c) {
                case 'r':
                    color = dicePaint.red;
                    break;
                case 'g':
                    color = dicePaint.green;
                    break;
                case 'y':
                    color = dicePaint.yellow;
                    break;
                case 'u':
                    color = dicePaint.blue;
                    break;
                case 'o':
                    color = dicePaint.orange;
                    break;
                case 'i':
                    color = dicePaint.pink;
                    break;
                case 'p':
                    color = dicePaint.purple;
                    break;
                case 'b':
                    color = dicePaint.black;
                    break;
            }

            dicePaint.SetDiceColor(i, color);
            i++;
        }
    }

    // Generate Dice with all side's colors randimized.
    private char[] GetRandomDice() {
        var rand = new Random();
        // define possible sides for dice 
        char[] colors = new[] {'r', 'g', 'y', 'u', 'o', 'i', 'p', 'b'};
        var color_count = colors.Length;
        char[] random_dice = new char[6];


        for (int i = 0; i < random_dice.Length; i++) {
            var rand_color = rand.Next(color_count);
            random_dice[i] = colors[rand_color];
        }

        return random_dice;
    }

    // blank stage template
    private static char[,] default_stage = new char[7, 7] {
        {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
        {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
        {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
        {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
        {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
        {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
        {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
    };

    // easy
    private static char[] target_dice = new char[6] {
        'r', //1
        'r', //2
        'r', //3
        'r', //4
        'r', //5
        'r', //6
    };

    private static char[,] target_stage = new char[7, 7] {
        {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
        {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
        {'d', 'r', 'r', 'r', 'r', 'd', 'd'},
        {'d', 'd', 'r', 'd', 'r', 'd', 'd'},
        {'d', 'd', 'r', 'r', 'r', 'r', 'd'},
        {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
        {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
    };


    // medium 
    private static char[] greenfield_dice = new char[6] {
        'g', //1
        'g', //2
        'g', //3
        'y', //4
        'g', //5
        'g', //6
    };


    static char[,] greenfield_stage = new char[7, 7] {
        {'g', 'g', 'g', 'g', 'g', 'g', 'g'},
        {'g', 'g', 'g', 'g', 'g', 'g', 'g'},
        {'g', 'g', 'g', 'g', 'g', 'g', 'g'},
        {'g', 'g', 'g', 'g', 'g', 'g', 'g'},
        {'g', 'g', 'g', 'g', 'g', 'g', 'g'},
        {'g', 'g', 'g', 'g', 'g', 'g', 'g'},
        {'g', 'g', 'g', 'g', 'g', 'g', 'g'}
    };

    // hard

    private static char[] smiley_dice = new char[6] {
        'b', //1
        'y', //2
        'y', //3
        'y', //4
        'y', //5
        'y', //6
    };


    static char[,] smiley_stage = new char[7, 7] {
        {'y', 'y', 'y', 'y', 'y', 'y', 'y'},
        {'y', 'y', 'b', 'y', 'b', 'y', 'y'},
        {'y', 'y', 'b', 'y', 'b', 'y', 'y'},
        {'y', 'y', 'y', 'y', 'y', 'y', 'y'},
        {'y', 'b', 'y', 'y', 'y', 'b', 'y'},
        {'y', 'y', 'b', 'b', 'b', 'y', 'y'},
        {'y', 'y', 'y', 'y', 'y', 'y', 'y'},
    };

    // easy
    private static char[] abstract_dice = new char[6] {
        'i', //1
        'p', //2
        'u', //3
        'u', //4
        'i', //5
        'p', //6
    };


    static char[,] abstract_stage = new char[7, 7] {
        {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
        {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
        {'d', 'd', 'u', 'i', 'u', 'd', 'd'},
        {'d', 'd', 'p', 'i', 'i', 'd', 'd'},
        {'d', 'd', 'u', 'i', 'u', 'd', 'd'},
        {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
        {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
    };

    // debug
    private static char[] debug_dice = new char[6] {
        'b', //1
        'b', //2
        'b', //3
        'b', //4
        'b', //5
        'b', //6
    };

    private static char[,] debug_stage = new char[7, 7] {
        {'b', 'b', 'd', 'd', 'd', 'd', 'd'},
        {'b', 'd', 'd', 'd', 'd', 'd', 'd'},
        {'b', 'd', 'd', 'd', 'd', 'd', 'd'},
        {'b', 'b', 'b', 'b', 'd', 'd', 'd'},
        {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
        {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
        {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
    };

    // NEW STAGES
    // easy
    private static char[] window_dice = new char[6] {
        'b', //1
        'b', //2
        'b', //3
        'b', //4
        'b', //5
        'b', //6
    };

    private static char[,] window_stage = new char[7, 7] {
        {'b', 'b', 'b', 'b', 'b', 'b', 'b'},
        {'b', 'd', 'd', 'b', 'd', 'd', 'b'},
        {'b', 'd', 'd', 'b', 'd', 'd', 'b'},
        {'b', 'b', 'b', 'b', 'b', 'b', 'b'},
        {'b', 'd', 'd', 'b', 'd', 'd', 'b'},
        {'b', 'd', 'd', 'b', 'd', 'd', 'b'},
        {'b', 'b', 'b', 'b', 'b', 'b', 'b'},
    };

    // medium
    private static char[] swim_dice = new char[6] {
        'b', //1
        'u', //2
        'b', //3
        'b', //4
        'u', //5
        'b', //6
    };

    private static char[,] swim_stage = new char[7, 7] {
        {'b', 'u', 'b', 'u', 'b', 'u', 'b'},
        {'b', 'u', 'b', 'u', 'b', 'u', 'b'},
        {'b', 'u', 'b', 'u', 'b', 'u', 'b'},
        {'b', 'u', 'b', 'u', 'b', 'u', 'b'},
        {'b', 'u', 'b', 'u', 'b', 'u', 'b'},
        {'b', 'u', 'b', 'u', 'b', 'u', 'b'},
        {'b', 'u', 'b', 'u', 'b', 'u', 'b'},
    };

    // medium
    private static char[] bee_dice = new char[6] {
        'b', //1
        'y', //2
        'b', //3
        'b', //4
        'y', //5
        'b', //6
    };

    private static char[,] bee_stage = new char[7, 7] {
        {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
        {'d', 'd', 'b', 'b', 'b', 'd', 'd'},
        {'d', 'd', 'y', 'y', 'y', 'd', 'd'},
        {'d', 'd', 'b', 'b', 'b', 'd', 'd'},
        {'d', 'd', 'y', 'y', 'y', 'd', 'd'},
        {'d', 'd', 'd', 'b', 'd', 'd', 'd'},
        {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
    };

    // derp

    // medium
    private static char[] cake_dice = new char[6] {
        'i', //1
        'i', //2
        'b', //3
        'b', //4
        'b', //5
        'b', //6
    };

    private static char[,] cake_stage = new char[7, 7] {
        {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
        {'d', 'd', 'i', 'd', 'i', 'd', 'd'},
        {'d', 'b', 'b', 'b', 'b', 'b', 'd'},
        {'d', 'i', 'i', 'i', 'i', 'i', 'd'},
        {'d', 'b', 'b', 'b', 'b', 'b', 'd'},
        {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
        {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
    };
}