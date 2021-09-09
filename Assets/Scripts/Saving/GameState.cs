using System;
using System.Collections.Generic;

/**
 * Contains all the data for game state
 */

[Serializable]
 public class GameState {
    public const int GridSize = 30;

    public int DialogPosition;
    public List<List<bool>> Grid;
 
    public GameState() {
        DialogPosition = 0;
        Grid = new List<List<bool>>(GridSize);
        for (int x = 0; x < GridSize; x++) {
            List<bool> row = new List<bool>(GridSize);
            for (int y = 0; y < GridSize; y++) {
                row.Add(false);
            }
            Grid.Add(row);
        }
    }
 }