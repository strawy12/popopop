using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoSingleTon<GameManager>
{
    [SerializeField] private Block blockTemp = null;
    [SerializeField] private Block playerTemp = null;
    [SerializeField] private Block scoreTemp = null;
    [SerializeField] private Transform blockTransform = null;
    [SerializeField] private Transform scoreTransform = null;

    private Block[,] blockPosition = new Block[5, 5];
    private Block[,] scorePosition = new Block[5, 5];
    private UIManager uiManager = null;
    private bool[] bonusCheck = new bool[5];
    private int bonusCnt = 0;
    public enum EObjectType {block, score };
    private Block player = null;
    private Dictionary<EObjectType, Queue<GameObject>> poolingDict = new Dictionary<EObjectType, Queue<GameObject>>();
    private bool isLoading = false;
    private bool isGameOver = false;
    private int score = 0;
    public UIManager UI { get { return uiManager; } }
    public bool Loading { get { return isLoading; } }
    int moveCnt = 0;

    private void Update()
    {
        if (GameOver())
        {
            isGameOver = true;
        }
    }
    private void Awake()
    {
        uiManager = GetComponent<UIManager>();
        poolingDict.Add(EObjectType.block, new Queue<GameObject>());
        poolingDict.Add(EObjectType.score, new Queue<GameObject>());
    }
    private void Start()
    {
        isLoading = true;

        StartCoroutine(SpawnStartBlock());
        UI.SetScoreText(0);

        isLoading = false;
    }

    private IEnumerator SpawnStartBlock()
    {
        SpawnPlayer();
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < 9; i++)
        {
            SpawnBlock(true);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void AddScore(int score)
    {
        this.score += score;
        UI.SetScoreText(this.score);
    }

    private void SpawnPlayer()
    {
        Block block = null;
        block = Instantiate(playerTemp, transform);
        block.transform.SetParent(null);
        block.xPos = 2;
        block.yPos = 2;
        blockPosition[2, 2] = block;
        block.SetSpawnCoords();
        player = block;
    }

    private void SpawnBlock(bool isStart = false)
    {
        int[] nums;
        int bonus = Random.Range(1, 11);
        Block block = null;
        nums = RandomNumPick();
        
        if (poolingDict[EObjectType.block].Count > 0)
        {
            block = poolingDict[EObjectType.block].Dequeue().GetComponent<Block>();
            block.gameObject.SetActive(true);
        }
        else
        {
            block = Instantiate(blockTemp, blockTransform);
        }

        block.xPos = nums[0];
        block.yPos = nums[1];
        blockPosition[nums[0], nums[1]] = block;
        if (bonus == 5 && !isStart)
        {
            block.isBonus = true;
        }
        block.SetSpawnCoords();

    }

    public void ScorePositionEmpty(int x, int y)
    {
        scorePosition[x, y] = null;
    }

    public void PoolEnqueue(EObjectType type, GameObject obj)
    {
        poolingDict[type].Enqueue(obj);
    }

    private void SpawnScore(int x, int y)
    {
        Block score = null;
        if (poolingDict[EObjectType.score].Count > 0)
        {
            score = poolingDict[EObjectType.score].Dequeue().GetComponent<Block>();
            score.gameObject.SetActive(true);
        }
        else
        {
            score = Instantiate(scoreTemp, scoreTransform);
        }
        score.xPos = x;
        score.yPos = y;
        scorePosition[x, y] = score;
        score.SetSpawnCoords();
    }

    private int[] RandomNumPick()
    {
        int x = 0;
        int y = 0;
        do
        {
            x = Random.Range(0, 5);
            y = Random.Range(0, 5);

        } while (blockPosition[x, y] != null || CheckSpawn(x, y));
        return new int[2] { x, y };
    }

    private bool CheckSpawn(int x, int y)
    {
        if(CheckMaxBlock() && CheckLine(x, y))
        {
            return false;
        }

        return CheckLine(x, y) || CheckScorePos(x, y);
    }

    private bool CheckScorePos(int x, int y)
    {
        if(scorePosition[x,y] != null)
        {
            return true;
        }
        return false;
    }

    private bool CheckMaxBlock()
    {
        List<int[]> blockPosList = new List<int[]>();

        for (int i = 0; i < 5; i++)
        {
            for(int j = 0; j < 5; j++)
            {
                if(blockPosition[i,j] == null)
                {
                    int[] pos = { i, j };
                    blockPosList.Add(pos);

                }
            }
        }

        for (int i = 0; i < blockPosList.Count; i++)
        {
            if (CheckLine(blockPosList[i][0], blockPosList[i][1]) || CheckScorePos(blockPosList[i][0], blockPosList[i][1]))
            {
                blockPosList.RemoveAt(i);
                i--;
            }

        }


        if(blockPosList.Count == 0)
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    private bool CheckLine(int x, int y)
    {
        int xCnt = 0;
        int yCnt = 0;
        for (int i = 0; i < 5; i++)
        {
            if (blockPosition[x, i] != null && blockPosition[x, i] != player)
            {
                xCnt++;
            }
            if (blockPosition[i, y] != null && blockPosition[i, y] != player)
            {
                yCnt++;
            }
        }
        if (xCnt == 4 || yCnt == 4)
        {
            return true;
        }
        return false;
    }

    public IEnumerator MoveBlock(bool isHV, bool isPlma)
    {
        if (isGameOver) yield break;
        isLoading = true;
        int playerPosX = player.xPos;
        int playerPosY = player.yPos;
        int sideWallNum = 0;
        if (isHV)
        {
            if (isPlma)
            {
                SetXPlusPos();
                sideWallNum = 0;
            }
            else
            {
                SetXMinusPos();
                sideWallNum = 1;
            }
        }
        else
        {
            if (isPlma)
            {
                SetYPlusPos();
                sideWallNum = 2;
            }
            else
            {
                SetYMinusPos();
                sideWallNum = 3;
            }
        }
        UI.ActiveMoveEffect(sideWallNum);
        if (playerPosX == player.xPos && playerPosY == player.yPos)
        {
            isLoading = false;
            yield break;
        }
        moveCnt++;
        yield return new WaitForSeconds(0.3f);
        if (moveCnt >= 2)
        {
            SpawnBlock();
            moveCnt = 0;
        }
        yield return new WaitForSeconds(0.2f);

        CheckMate();
        
    }
    private bool GameOver()
    {
        for (int i = 0; i < 5; i++)
        {
            if (blockPosition[i, player.yPos] == null || blockPosition[player.xPos, i] == null)
            {
                return false;
            }
        }
        return true;
    }
    private void CheckMate()
    {
        isLoading = false;

        for (int i = 0; i < 5; i++)
        {
            CheckXMate(i);
            CheckYMate(i);
        }
    }

    private void CheckXMate(int i)
    {
        int cnt = 0;
        Block newBlock = null;
        for (int j = 0; j < 5; j++)
        {
            if (blockPosition[i, j] == player)
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
                if(newBlock.isBonus)
                {
                    BonusActive();
                    newBlock.isBonus = false;

                }
                newBlock.Despawn();
                SpawnScore(i, j);
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
                if (newBlock.isBonus)
                {
                    BonusActive();
                    newBlock.isBonus = false;
                }
                newBlock.Despawn();
                SpawnScore(j, i);
            }
        }
    }

    private void BonusActive()
    {
        bonusCheck[bonusCnt] = true;
        UI.ActiveBonusText(bonusCnt);
        bonusCnt++;
        if (bonusCnt >= 5)
        {
            bonusCheck = new bool[5];
            bonusCnt = 0;
            for(int i = 0; i < 5; i++)
            {
                if(blockPosition[player.xPos, i] != null && blockPosition[player.xPos, i] != player)
                {
                    blockPosition[player.xPos, i].Despawn();
                    blockPosition[player.xPos, i] = null;
                }
                if(blockPosition[i, player.yPos] != null && blockPosition[i, player.yPos] != player)
                {
                    blockPosition[i, player.yPos].Despawn();
                    blockPosition[i, player.yPos] = null;
                }
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
