using System;
using System.Collections.Generic;

/// <summary>
/// Board is 10x10 top left half.
/// </summary>
public class Board
{
	public int[,] m_index = new int[10, 10];
    public List<PlacedBlock> m_blocks = new List<PlacedBlock>();

	public Board()
	{
	}

    /// <summary>
    /// Place a block at given position. Do not check for collision.
    /// </summary>
    /// <param name="block">the block to add to board</param>
    /// <param name="x">row number</param>
    /// <param name="y">column number</param>
    internal void PlaceBlock(Block block, int x, int y)
    {
        PlacedBlock pb = new PlacedBlock();
        pb.block = block;
        pb.position = new Position(x, y);
        m_blocks.Add(pb);

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                m_index[x + i, y + j] += block.m_index[i, j];
            }
        }
    }
}
