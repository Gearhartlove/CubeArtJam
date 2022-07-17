using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class GameManage : MonoBehaviour {
    // todo: update with spawn position
    public float xPos = 3;
    public float zPos = 3;

    public Paint dicePaint;

    private void Start() {
        canvasController = GameObject.Find("Canvas").GetComponent<Canvas_Controller>();
        dicePaint = GameObject.Find("Dice").GetComponent<Paint>();
        
        setup_scene(abstract_stage, abstract_dice);
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
    private char[,] progressStage;

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
        return progressStage[(int) position.x, (int) position.y] == c;
    }
    
    // sound helper function
    private bool CorrectPaint(char c, Vector2 position) {
        return currentStage[(int) position.x, (int) position.y] == c;
    }
    
    public void CheckCell(Canvas_Piece piece) {
        Vector2 position = piece.GetPosition();
        // get he color Char to update progress array
        Char c = GetColor(piece);
        Debug.Log(c);
        // correct
        if (!TheSamePaint(c, position)) {
            if (CorrectPaint(c, position)) {
                // correct sound
                
            }
            else {
                // incorrect sound
            }
            
        }
        // update progress array
        progressStage[(int) position.x, (int) position.y] = c;
        // check the stage if it is complete
        bool stageComplete = true;
        for (int y = 0; y < 6; y++) {
            for (int x = 0; x < 6; x++) {
                // couldo: improve what is incorrect message
                if (progressStage[x, y] != currentStage[x, y]) {
                    stageComplete = false;
                }
            }
        }
        if (stageComplete) {
            Debug.Log("Stage Done");
        }
    }

    public void setup_scene(char[,] scene, char[] dice) {
        // scene
        currentStage = scene;
        progressStage = new char[7, 7] {
            {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
            {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
            {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
            {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
            {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
            {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
            {'d', 'd', 'd', 'd', 'd', 'd', 'd'},
        };
        // load a new scene
        // dice
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
        {'d', 'd', 'r', 'r', 'r', 'd', 'd'},
        {'d', 'd', 'r', 'd', 'r', 'd', 'd'},
        {'d', 'd', 'r', 'r', 'r', 'd', 'd'},
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
        {'y', 'y', 'y', 'y', 'y', 'y', 'y'},
        {'y', 'b', 'y', 'y', 'y', 'b', 'y'},
        {'y', 'y', 'b', 'b', 'b', 'y', 'y'},
        {'y', 'y', 'y', 'y', 'y', 'y', 'y'},
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
}