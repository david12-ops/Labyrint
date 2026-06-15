using UnityEngine;

public class MazeRender : MonoBehaviour
{
    [SerializeField] MazeGenerator mazeGenerator;
    [SerializeField] GameObject MazeCellPrefab;

    //This the physical size of our maze cells. Getting this wrong will result overlaping
    //or visisble gaps between each cell.
    public float CellSize = 1f;

    private void Start()
    {
        //Get our MazeGenerator script to make us maze.
        MazeCell[,] maze = mazeGenerator.GetMaze();

        //Loop trough every cell in the maze
        for(int x = 0;x < mazeGenerator.mazeWidth; x++)
        {
            for(int y = 0;y < mazeGenerator.mazeHeight;y++)
            {
                //Instantiate a new maze cell prefab as a child of the MazeRender object
                GameObject newCell = Instantiate(MazeCellPrefab, new Vector3((float)x * CellSize, 0f, (float)y * CellSize), Quaternion.identity, transform);

                //Get a reference to the cell's MazeCellPrefab
                MazeCellObject mazeCell = newCell.GetComponent<MazeCellObject>();

                //Determine which walls need to be active
                bool top = maze[x,y].topWall;
                bool left = maze[x,y].leftWall;

                //Bottom and right walls are deactivated by default unless we are at the bottom or right
                //edge of the maze
                bool right = false;
                bool bottom = false;

                if(x == mazeGenerator.mazeWidth - 1) right = true;
                if(y == 0) bottom = true;

                mazeCell.Init(top, bottom, right, left);
            }
        }
    }
}
