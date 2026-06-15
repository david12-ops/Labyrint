using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Range(5, 500)]
    public int mazeWidth = 5, mazeHeight = 5;
    public int startX, startY;

    MazeCell[,] mazeCells;
    Vector2Int currentCell;

    public MazeCell[,] GetMaze()
    {
        mazeCells = new MazeCell[mazeWidth, mazeHeight];

        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                mazeCells[x, y] = new MazeCell(x, y);
            }
        }

        //Start carving our maze
        CarvePath(startX, startY);

        return mazeCells;
    }

    List<Direction> directions = new List<Direction> { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

    List<Direction> GetRandomDirections()
    {
        // Create a copy of the directions list
        List<Direction> dir = new List<Direction>(directions);

        // Make a directions list to put our randomised directions into
        List<Direction> randomDirections = new List<Direction>();

        while (dir.Count > 0)
        {
            int randomIndex = Random.Range(0, dir.Count);
            randomDirections.Add(dir[randomIndex]);
            dir.RemoveAt(randomIndex);
        }

        return randomDirections;
    }

    bool IsCellValid(int x, int y)
    {
        // If the cell is out of bounds or has already been visited, return false
        if (x < 0 || y < 0 || x > mazeWidth - 1 || y > mazeHeight - 1 || mazeCells[x, y].visited)
        {
            return false;
        }

        return true;
    }

    Vector2Int CheckNeigbours()
    {
        List<Direction> rndDir = GetRandomDirections();

        for (int i = 0; i < rndDir.Count; i++)
        {
            Vector2Int neighbour = currentCell;

            switch (rndDir[i])
            {
                case Direction.Up:
                    neighbour.y++;
                    break;

                case Direction.Down:
                    neighbour.y--;
                    break;

                case Direction.Right:
                    neighbour.x++;
                    break;

                case Direction.Left:
                    neighbour.x--;
                    break;
            }

            //If the neigbour we just tried is valid; we can return that neighbour. If not, we go again;
            if (IsCellValid(neighbour.x, neighbour.y)) return neighbour;
        }

        //If we tried all directions and did not find a valid neighbour, we return the currentCell values.
        return currentCell;
    }

    // Takes in two maze positions and sets the cells acordingly to break the wall between them. The primary cell is the one that is being visited, and the secondary cell is the one that is being visited next.
    void BreakWalls(Vector2Int primaryCell, Vector2Int secondaryCell)
    {
        //We can only go in one direction at a time
        if (primaryCell.x > secondaryCell.x)
        {
            mazeCells[primaryCell.x, primaryCell.y].leftWall = false;
        } else if (primaryCell.x < secondaryCell.x)
        {
            mazeCells[secondaryCell.x, secondaryCell.y].leftWall = false;
        } else if (primaryCell.y < secondaryCell.y)
        {
            mazeCells[primaryCell.x, primaryCell.y].topWall = false;
        } else if (primaryCell.y > secondaryCell.y)
        {
            mazeCells[secondaryCell.x, secondaryCell.y].topWall = false;
        }
    }

    //Starting at the x,y passed in, carves a path trough the maze until it reaches a dead end. Then it backtracks to the last cell that has unvisited neighbours and continues carving from there.
    void CarvePath(int x, int y)
    {
        //Perfrom a quick to make sure our start position is within the bounderies of the map,
        //if not, set them to default and throw a little warning up
        if (x < 0 || y < 0 || x > mazeWidth - 1 || y > mazeHeight - 1)
        {
            x = y = 0;
            Debug.LogWarning("Starting position is out of bounds, defaulting to 0, 0");
        }

        //Set current cell to the starting position we were passed
        currentCell = new Vector2Int(x, y);

        //A list to keep track of our current path
        List<Vector2Int> path = new List<Vector2Int>();

        //Loop until we encounter a dead end
        bool deadEnd = false;

        while (!deadEnd)
        {
            //Get next cell we are going to try
            Vector2Int nextCell = CheckNeigbours();

            //If that cell has no valid neighbours, set deadend to true so we break out of the loop
            if(nextCell == currentCell)
            {
                for(int i = path.Count - 1; i >= 0; i--)
                {
                    currentCell = path[i];                  //Set currentCell to the next step back along our path
                    path.RemoveAt(i);                       //Remove this step from the path
                    nextCell = CheckNeigbours();            //Check that cell to see if any other neighbours are valid

                    //if we find a valid neighbour, break out of the loop
                    if (nextCell != currentCell) break;
                }

                if (nextCell == currentCell)
                    deadEnd = true;
            }
            else
            {
                BreakWalls(currentCell, nextCell);                              //Set wall flags on these two cells
                mazeCells[currentCell.x, currentCell.y].visited = true;         //Set cell to visited before moving on
                currentCell = nextCell;                                         //Set current cell to valid neighbour we found
                path.Add(currentCell);                                          //Add this cell to our path
            }
        }
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public class MazeCell
{
    public bool visited;
    public int x, y;

    public bool topWall;
    public bool leftWall;

    // Return x and y as a Vector2Int for convenience sake
    public Vector2Int position
    {
        get
        {
            return new Vector2Int(x, y);
        }
    }

    public MazeCell(int x, int y)
    {
        // Cordinates of the cell in the maze grid.
        this.x = x;
        this.y = y;

        // Whether the cell has been visited or not during maze generation.
        visited = false;

        // All walls are present at the start of maze generation.
        topWall = leftWall = true;
    }
}
