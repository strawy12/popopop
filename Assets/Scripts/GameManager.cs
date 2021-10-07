using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleTon<GameManager>
{
    [SerializeField] private Block blockTemp = null;
    [SerializeField] private Block playerTemp = null;

    private Block[,] blockPosition = new Block[5, 5];
    private Block player = null;
    private List<Block> blocks = new List<Block>();
    private bool isLoading = false;
    public bool Loading { get { return isLoading; } }
    int cnt = 0;
    private void Start()
    {
        isLoading = true;
        SpawnPlayer();
        for (int i = 0; i < 9; i++)
        {
            SpawnBlock();
        }
        isLoading = false;
    }

    private void SpawnPlayer()
    {
        Block block = null;
        block = Instantiate(playerTemp, transform);
        block.transform.SetParent(null);
        block.xPos = 2;
        block.yPos = 2;
        blockPosition[2, 2] = block;
        blocks.Add(block);
        block.SetSpawnCoords();
        player = block;
    }

    private void SpawnBlock()
    {
        int[] nums;
        Block block = null;
        nums = RandomNumPick();
        block = Instantiate(blockTemp, transform);
        block.transform.SetParent(null);
        block.xPos = nums[0];
        block.yPos = nums[1];
        blockPosition[nums[0], nums[1]] = block;
        blocks.Add(block);
        block.SetSpawnCoords();
    }

    private int[] RandomNumPick()
    {
        int x = 0;
        int y = 0;

        do
        {
            x = Random.Range(0, 5);
            y = Random.Range(0, 5);
        } while (blockPosition[x, y] != null);
        return new int[2] { x, y };
    }

    public void MoveBlock(bool isHV, bool isPlma)
    {
        isLoading = true;
        if (isHV)
        {
            if (isPlma)
            {
                SetXPlusPos();
            }
            else
            {
                SetXMinusPos();
            }
        }
        else
        {
            if (isPlma)
            {
                SetYPlusPos();
            }
            else
            {
                SetYMinusPos();
            }
        }
        Invoke("CheckMate", 0.7f);
        Invoke("SpawnBlock", 0.5f);
    }

    private void CheckMate()
    {
        for (int i = 0; i < 5; i++)
        {
            CheckXMate(i);
            CheckYMate(i);
        }
        isLoading = false;
    }

    private void CheckXMate(int i)
    {
        int cnt = 0;
        Block newBlock = null;
        for (int j = 0; j < 5; j++)
        {
            if(blockPosition[i, j] == player)
            {
                break;
            }
            if (blockPosition[i, j] == null)
            {
                break;
            }
            cnt++;
        }
        if (cnt == 5)
        {
            for (int j = 0; j < 5; j++)
            {
                newBlock = blockPosition[i, j];
                blockPosition[i, j] = null;
                Destroy(newBlock.gameObject);
            }
        }
    }

    private void CheckYMate(int i)
    {
        int cnt = 0;
        Block newBlock = null;
        for (int j = 0; j < 5; j++)
        {
            if (blockPosition[j, i] == player)
            {
                break;
            }
            if (blockPosition[j, i] == null)
            {
                break;
            }
            cnt++;
        }
        if (cnt == 5)
        {
            for (int j = 0; j < 5; j++)
            {
                newBlock = blockPosition[j, i];
                blockPosition[j, i] = null;
                Destroy(newBlock.gameObject);
            }
        }
    }

    #region PosSetting

    private void SetXPlusPos()
    {
        Block newBlock = null;
        int xPos;
        int x = player.xPos;
        int y = player.yPos;
        for (int i = 4; i >= x; i--)
        {
            if (CheckBlock(i, y)) continue;
            xPos = CheckXPlus(i, y);
            newBlock = blockPosition[i, y];
            blockPosition[i, y] = null;
            blockPosition[xPos, y] = newBlock;
            newBlock.xPos = xPos;
            newBlock.yPos = y;
            newBlock.SetMoveCoords();
        }
    }
    private void SetXMinusPos()
    {
        Block newBlock = null;
        int xPos;
        int x = player.xPos;
        int y = player.yPos;
        for (int i = 0; i <= x; i++)
        {
            if (CheckBlock(i, y)) continue;
            xPos = CheckXMinus(i, y);
            newBlock = blockPosition[i, y];
            blockPosition[i, y] = null;
            blockPosition[xPos, y] = newBlock;
            newBlock.xPos = xPos;
            newBlock.yPos = y;
            newBlock.SetMoveCoords();
        }
    }

    private void SetYPlusPos()
    {
        Block newBlock = null;
        int yPos;
        int x = player.xPos;
        int y = player.yPos;

        for (int i = 4; i >= y; i--)
        {
            if (CheckBlock(x, i)) continue;
            yPos = CheckYPlus(x, i);
            newBlock = blockPosition[x, i];
            blockPosition[x, i] = null;
            blockPosition[x, yPos] = newBlock;
            newBlock.xPos = x;
            newBlock.yPos = yPos;
            newBlock.SetMoveCoords();
        }
    }

    private void SetYMinusPos()
    {
        Block newBlock = null;
        int yPos;
        int x = player.xPos;
        int y = player.yPos;

        for (int i = 0; i <= y; i++)
        {
            if (CheckBlock(x, i)) continue;
            yPos = CheckYMinus(x, i);
            newBlock = blockPosition[x, i];
            blockPosition[x, i] = null;
            blockPosition[x, yPos] = newBlock;
            newBlock.xPos = x;
            newBlock.yPos = yPos;
            newBlock.SetMoveCoords();
        }
    }

    private bool CheckBlock(int x, int y)
    {
        if (x < 0 || x > 4 || y < 0 || y > 4) return true;
        if (blockPosition[x, y] == null)
        {
            return true;
        }
        return false;
    }

    private int CheckXPlus(int x, int y)
    {
        for (int i = x; i < 5; i++)
        {
            if (!CheckBlock(i + 1, y))
            {
                return i;
            }
        }

        return 4;
    }
    private int CheckXMinus(int x, int y)
    {
        for (int i = x; i >= 0; i--)
        {
            if (!CheckBlock(i - 1, y))
            {
                return i;
            }
        }

        return 0;
    }
    private int CheckYPlus(int x, int y)
    {
        for (int i = y; i < 5; i++)
        {
            if (!CheckBlock(x, i + 1))
            {
                return i;
            }
        }

        return 4;
    }
    private int CheckYMinus(int x, int y)
    {
        for (int i = y; i >= 0; i--)
        {
            if (!CheckBlock(x, i - 1))
            {
                return i;
            }
        }

        return 0;
    }

    #endregion

}
