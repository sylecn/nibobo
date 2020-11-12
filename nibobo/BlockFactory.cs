using System;
using System.Collections.Generic;
using System.Drawing;

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
        switch (name)
        {
            case "A":
                b.m_name = name;
                b.m_color = Color.Gray;
                b.m_index = new int[4, 4]{
                    {0, 1, 0, 0},
                    {0, 1, 0, 0},
                    {1, 1, 0, 0},
                    {0, 0, 0, 0}
                };
                break;
            case "B":
                b.m_name = name;
                b.m_color = Color.LightBlue;
                b.m_index = new int[4, 4]{
                    {0, 1, 0, 0},
                    {1, 1, 0, 0},
                    {1, 1, 0, 0},
                    {0, 0, 0, 0}
                };
                break;
            case "C":
                b.m_name = name;
                b.m_color = Color.LightPink;
                b.m_index = new int[4, 4]{
                    {0, 1, 0, 0},
                    {0, 1, 0, 0},
                    {0, 1, 0, 0},
                    {1, 1, 0, 0}
                };
                break;
            case "D":
                b.m_name = name;
                b.m_color = Color.LightGreen;
                b.m_index = new int[4, 4]{
                    {0, 1, 0, 0},
                    {0, 1, 0, 0},
                    {1, 1, 0, 0},
                    {0, 1, 0, 0}
                };
                break;
            case "E":
                b.m_name = name;
                b.m_color = Color.Purple;
                b.m_index = new int[4, 4]{
                    {0, 1, 0, 0},
                    {0, 1, 0, 0},
                    {1, 1, 0, 0},
                    {1, 0, 0, 0}
                };
                break;
            case "F":
                b.m_name = name;
                b.m_color = Color.Pink;
                b.m_index = new int[4, 4]{
                    {0, 1, 0, 0},
                    {1, 1, 0, 0},
                    {0, 0, 0, 0},
                    {0, 0, 0, 0}
                };
                break;
            case "G":
                b.m_name = name;
                b.m_color = Color.Blue;
                b.m_index = new int[4, 4]{
                    {0, 0, 1, 0},
                    {0, 0, 1, 0},
                    {1, 1, 1, 0},
                    {0, 0, 0, 0}
                };
                break;
            case "H":
                b.m_name = name;
                b.m_color = Color.Orange;
                b.m_index = new int[4, 4]{
                    {0, 0, 1, 0},
                    {0, 1, 1, 0},
                    {1, 1, 0, 0},
                    {0, 0, 0, 0}
                };
                break;
            case "I":
                b.m_name = name;
                b.m_color = Color.White;
                b.m_index = new int[4, 4]{
                    {1, 0, 1, 0},
                    {1, 1, 1, 0},
                    {0, 0, 0, 0},
                    {0, 0, 0, 0}
                };
                break;
            case "J":
                b.m_name = name;
                b.m_color = Color.Green;
                b.m_index = new int[4, 4]{
                    {1, 0, 0, 0},
                    {1, 0, 0, 0},
                    {1, 0, 0, 0},
                    {1, 0, 0, 0}
                };
                break;
            case "K":
                b.m_name = name;
                b.m_color = Color.Yellow;
                b.m_index = new int[4, 4]{
                    {1, 1, 0, 0},
                    {1, 1, 0, 0},
                    {0, 0, 0, 0},
                    {0, 0, 0, 0}
                };
                break;
            case "L":
                b.m_name = name;
                b.m_color = Color.Yellow;
                b.m_index = new int[4, 4]{
                    {0, 1, 0, 0},
                    {1, 1, 1, 0},
                    {0, 1, 0, 0},
                    {0, 0, 0, 0}
                };
                break;
            default:
                throw new ArgumentException("Error: invalid block name: " + name);
        }
        return b;
    }

    public static Block GetBlockByName(string name)
    {
        if (m_singleton == null)
        {
            m_singleton = new BlockFactory();
        }
        return m_singleton.m_namedBlocks[name];
    }
}
