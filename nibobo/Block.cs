using System;
using System.Drawing;

public class Block
{
    public string m_name;
    public Color m_color;
    public int[,] m_index = new int[4, 4];
    public int m_varient;
    public int[,,] m_allDirections;

    public Block()
    {
    }
}

public class Position
{
    public int x;
    public int y;

    public Position()
    {
        x = 0;
        y = 0;
    }

    public Position(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class PlacedBlock
{
    public Block block;
    /// <summary>
    /// 4x4 block index top left position on game board.
    /// </summary>
    public Position position;
}