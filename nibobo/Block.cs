﻿using System.Collections.Generic;
using System.Drawing;

public class Block
{
    public string m_name;
    public Color m_color;
    /// <summary>
    /// Stores all rotation and flip transformations of this block.
    /// </summary>
    public List<int[,]> m_varients = new List<int[,]> { };

    public Block()
    {
    }
}

public class PlacedBlock
{
    public Block m_block;
    /// <summary>
    /// Which varient is in use, default is 0.
    /// </summary>
    public int m_varient;
    /// <summary>
    /// 4x4 block index top left position on game board.
    /// </summary>
    public Position m_position;

    public PlacedBlock()
    {
    }

    public PlacedBlock(Block block, int varient, int x, int y)
    {
        m_block = block;
        m_varient = varient;
        m_position = new Position(x, y);
    }

    public int this[int i, int j]
    {
        get => m_block.m_varients[m_varient][i, j];
        set
        {
            m_block.m_varients[m_varient][i, j] = value;
        }
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
