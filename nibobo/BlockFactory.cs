using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

public class BlockFactory
{
    private Dictionary<string, Block> m_namedBlocks = new Dictionary<string, Block>();
    private static BlockFactory m_singleton = null;

    private BlockFactory()
	{
        string[] blockNames = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L" };
        for (int i = 0; i < blockNames.Length; i++)
        {
            m_namedBlocks.Add(blockNames[i], MakeBlockByName(blockNames[i]));
        }
    }

    private static Block MakeBlockByName(string name)
    {
        Block b = new Block();
        b.m_name = name;
        switch (name)
        {
            case "A":
                b.m_color = Color.FromArgb(255, 115, 136, 155);
                b.m_varients.Add(new int[4, 4]{
                    {0, 1, 0, 0},
                    {0, 1, 0, 0},
                    {1, 1, 0, 0},
                    {0, 0, 0, 0}
                });
                break;
            case "B":
                b.m_color = Color.LightBlue;
                b.m_varients.Add(new int[4, 4]{
                    {0, 1, 0, 0},
                    {1, 1, 0, 0},
                    {1, 1, 0, 0},
                    {0, 0, 0, 0}
                });
                break;
            case "C":
                b.m_color = Color.FromArgb(255, 211, 198, 205);
                b.m_varients.Add(new int[4, 4]{
                    {0, 1, 0, 0},
                    {0, 1, 0, 0},
                    {0, 1, 0, 0},
                    {1, 1, 0, 0}
                });
                break;
            case "D":
                b.m_color = Color.LightGreen;
                b.m_varients.Add(new int[4, 4]{
                    {0, 1, 0, 0},
                    {0, 1, 0, 0},
                    {1, 1, 0, 0},
                    {0, 1, 0, 0}
                });
                break;
            case "E":
                b.m_color = Color.Purple;
                b.m_varients.Add(new int[4, 4]{
                    {0, 1, 0, 0},
                    {0, 1, 0, 0},
                    {1, 1, 0, 0},
                    {1, 0, 0, 0}
                });
                break;
            case "F":
                b.m_color = Color.FromArgb(255, 252, 121, 166);
                b.m_varients.Add(new int[4, 4]{
                    {0, 1, 0, 0},
                    {1, 1, 0, 0},
                    {0, 0, 0, 0},
                    {0, 0, 0, 0}
                });
                break;
            case "G":
                b.m_color = Color.FromArgb(255, 0, 58, 217);
                b.m_varients.Add(new int[4, 4]{
                    {0, 0, 1, 0},
                    {0, 0, 1, 0},
                    {1, 1, 1, 0},
                    {0, 0, 0, 0}
                });
                break;
            case "H":
                b.m_color = Color.OrangeRed;
                b.m_varients.Add(new int[4, 4]{
                    {0, 0, 1, 0},
                    {0, 1, 1, 0},
                    {1, 1, 0, 0},
                    {0, 0, 0, 0}
                });
                break;
            case "I":
                b.m_color = Color.FromArgb(255, 209, 220, 238);
                b.m_varients.Add(new int[4, 4]{
                    {1, 0, 1, 0},
                    {1, 1, 1, 0},
                    {0, 0, 0, 0},
                    {0, 0, 0, 0}
                });
                break;
            case "J":
                b.m_color = Color.Green;
                b.m_varients.Add(new int[4, 4]{
                    {1, 0, 0, 0},
                    {1, 0, 0, 0},
                    {1, 0, 0, 0},
                    {1, 0, 0, 0}
                });
                break;
            case "K":
                b.m_color = Color.Yellow;
                b.m_varients.Add(new int[4, 4]{
                    {1, 1, 0, 0},
                    {1, 1, 0, 0},
                    {0, 0, 0, 0},
                    {0, 0, 0, 0}
                });
                break;
            case "L":
                b.m_color = Color.FromArgb(255, 200, 20, 31);
                b.m_varients.Add(new int[4, 4]{
                    {0, 1, 0, 0},
                    {1, 1, 1, 0},
                    {0, 1, 0, 0},
                    {0, 0, 0, 0}
                });
                break;
            default:
                throw new ArgumentException("Error: invalid block name: " + name);
        }
        MakeAllVarients(b.m_varients);
        return b;
    }

    public static bool ArrayEquals(int[,] ar1, int[,] ar2)
    {
        return ar1.Cast<int>().SequenceEqual(ar2.Cast<int>());
    }

    /// <summary>
    /// Add all varients to the list. varients should have the initial varient. At most 7 varients will be added.
    /// </summary>
    /// <param name="varients">contains the initial varient, new varients will be added to this List</param>
    private static void MakeAllVarients(List<int[,]> varients)
    {
        var v0 = varients[0];
        var v1 = NormalizeBlock(RotateRight(v0));
        var v2 = NormalizeBlock(RotateRight(v1));
        var v3 = NormalizeBlock(RotateRight(v2));
        var v4 = NormalizeBlock(Flip(v0));
        var v5 = NormalizeBlock(RotateRight(v4));
        var v6 = NormalizeBlock(RotateRight(v5));
        var v7 = NormalizeBlock(RotateRight(v6));
        List<int[,]> allVarients = new List<int[,]>();
        allVarients.Add(v1);
        allVarients.Add(v2);
        allVarients.Add(v3);
        allVarients.Add(v4);
        allVarients.Add(v5);
        allVarients.Add(v6);
        allVarients.Add(v7);
        foreach (var v in allVarients)
        {
            if (!varients.Any(e => ArrayEquals(e, v)))
            {
                varients.Add(v);
            }
        }
    }

    /// <summary>
    /// Flip block horizontally.
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    private static int[,] Flip(int[,] v)
    {
        int[,] result = new int[4, 4];
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                result[i, 3 - j] = v[i, j];
            }
        }
        return result;
    }

    public static Block GetBlockByName(string name)
    {
        if (m_singleton == null)
        {
            m_singleton = new BlockFactory();
        }
        return m_singleton.m_namedBlocks[name];
    }

    /// <summary>
    /// Move block index to top left. strip empty rows and columns.
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static int[,] NormalizeBlock(int[,] v)
    {
        int[,] result = new int[4, 4];
        int skipRows = 0;
        int skipColumns = 0;
        for (int i = 0; i < 4; i++)
        {
            if (v[i, 0] + v[i, 1] + v[i, 2] + v[i, 3] == 0)
            {
                skipRows++;
            }
            else
            {
                break;
            }
        }
        for (int i = 0; i < 4; i++)
        {
            if (v[0, i] + v[1, i] + v[2, i] + v[3, i] == 0)
            {
                skipColumns++;
            }
            else
            {
                break;
            }
        }
        for (int i = 0; i < 4 - skipRows; i++)
        {
            for (int j = 0; j < 4 - skipColumns; j++)
            {
                result[i, j] = v[skipRows + i, skipColumns + j];
            }
        }
        return result;
    }

    /// <summary>
    /// Rotate block index 90 degree to the right.
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static int[,] RotateRight(int[,] v)
    {
        int[,] result = new int[4, 4];
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                result[j, 3 - i] = v[i, j];
            }
        }
        return result;
    }

    public static void PrintBlock(int[,] b)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Console.Write("{0} ", b[i, j]);
            }
            Console.WriteLine();
        }
    }
}
