using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

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
    /// Copy constructor.
    /// </summary>
    /// <param name="board"></param>
    public Board(Board board)
    {
        Array.Copy(board.m_index, m_index, board.m_index.Length);
        m_blocks = board.m_blocks.ToList();
    }

    /// <summary>
    /// Place a block at given position. Do not check for collision.
    /// </summary>
    /// <param name="block">the block to add to board</param>
    /// <param name="x">row number</param>
    /// <param name="y">column number</param>
    public void PlaceBlock(Block block, int x, int y, int varient)
    {
        PlacedBlock pb = new PlacedBlock(block, varient, x, y);
        m_blocks.Add(pb);

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (x + i < 10 && y + j < 10)
                {
                    m_index[x + i, y + j] += pb[i, j];
                }
            }
        }
    }

    /// <summary>
    /// Place blocks on board so all cells are filled.
    /// </summary>
    /// <returns>a new Board if board is solved, otherwise, return null.</returns>
    public Board Solve()
    {
        List<Block> restBlocks = new List<Block>();
        foreach (string name in BlockFactory.m_blockNames)
        {
            if (!m_blocks.Any(pb => pb.m_block.m_name == name))
            {
                restBlocks.Add(BlockFactory.GetBlockByName(name));
            }
        }
        return DoSolve(this, restBlocks);
    }

    private static Board DoSolve(Board board, List<Block> restBlocks)
    {
        //Debug.WriteLine("restBlocks.Count = {0}", restBlocks.Count);
        if (restBlocks.Count == 0)
        {
            return board;
        }
        foreach (Block block in restBlocks)
        {
            //Debug.WriteLine("Trying block {0}", block.m_name);
            for (int varient = 0; varient < block.m_varients.Count; varient++)
            {
                //Debug.WriteLine("Trying block {0} varient {1}", block.m_name, varient);
                Board newBoard = board.TryPlaceBlock(block, varient);
                if (newBoard != null)
                {
                    //Debug.WriteLine("TryPlaceBlock okay");
                    List<Block> newRestBlocks = restBlocks.ToList();
                    bool r = newRestBlocks.Remove(block);
                    Debug.Assert(r);
                    Board result = DoSolve(newBoard, newRestBlocks);
                    if (result != null)
                    {
                        return result;  // return first solution for the puzzle.
                    }
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Try place block at first empty cell, from top to bottom, from left to right, leave no empty cells.
    /// </summary>
    /// <param name="block">the block to place</param>
    /// <param name="varient">the block varient</param>
    /// <returns>A new game Board if block varient can be placed at first empty cell.</returns>
    private Board TryPlaceBlock(Block block, int varient)
    {
        Position boardEmptyPos = FindFirstEmptyPosition();
        if (boardEmptyPos == null)
        {
            return null;
        }
        Position topRowLeftPos = FindTopRowLeft(block, varient);
        Debug.Assert(topRowLeftPos != null);

        int x = boardEmptyPos.x - topRowLeftPos.x;
        int y = boardEmptyPos.y - topRowLeftPos.y;
        PlacedBlock pb = new PlacedBlock(block, varient, x, y);
        bool canPlace = true;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (pb[i, j] == 1)
                {
                    if ((x + i < 0) ||
                        (x + i > 9) ||
                        (y + j < 0) ||
                        (y + j > 9 - (x + i)) ||
                        (m_index[x + i, y + j] == 1))
                    {
                        //Debug.WriteLine("canPlace=false");
                        canPlace = false;
                        break;
                    }
                }
            }
        }
        if (canPlace)
        {
            Board newBoard = new Board(this);
            newBoard.m_blocks.Add(pb);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (pb[i, j] == 1)
                    {
                        newBoard.m_index[x + i, y + j] = 1;
                    }
                }
            }
            return newBoard;
        }
        return null;
    }

    /// <summary>
    /// Find top row left most position for a block varient.
    /// </summary>
    /// <param name="block"></param>
    /// <param name="varient"></param>
    /// <returns></returns>
    private Position FindTopRowLeft(Block block, int varient)
    {
        int[,] v = block.m_varients[varient];
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (v[i, j] == 1)
                {
                    return new Position(i, j);
                }
            }
        }
        Debug.Assert(false);
        return null;
    }

    /// <summary>
    /// Find position for first empty position, counting from top to bottom, left to right.
    /// </summary>
    /// <returns></returns>
    private Position FindFirstEmptyPosition()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 9 - i; j++)
            {
                if (m_index[i, j] == 0)
                {
                    return new Position(i, j);
                }
            }
        }
        return null;
    }
}
