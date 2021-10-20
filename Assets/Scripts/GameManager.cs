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
    private ImageManager imageManager = null;
    private bool[] bonusCheck = new bool[5];
    private int bonusCnt = 0;
    public enum EObjectType { block, score };
    public enum EBlockState
    {
        NORMAL,
        TOP,
        RIGHT,
        TOP_RIGHT,
        DOWN,
        TOP_DOWN,
        RIGHT_DOWN,
        TOP_RIGHT_DOWN,
        LEFT,
        TOP_LEFT,
        RIGHT_LEFT,
        TOP_RIGHT_LEFT,
        LEFT_DOWN,
        TOP_LEFT_DOWN,
        RIGHT_LEFT_DOWN,
        TOP_RIGHT_LEFT_DOWN
    }
    private Block player = null;
    private Dictionary<EObjectType, Queue<GameObject>> poolingDict = new Dictionary<EObjectType, Queue<GameObject>>();
    private bool isLoading = false;
    private bool isGameOver = false;
    private int score = 0;
    public UIManager UI { get { return uiManager; } }
    public ImageManager Image { get { return imageManager; } }
    public Block Player { get { return player; } }
    public bool Loading { get { return isLoading; } }
    int moveCnt = 0;

    private void Update()
    {
        if (GameOver() && !isGameOver)
        {
            isGameOver = true;
            UI.ActiveGameOVerPanal(true);
        }
    }
    private void Awake()
    {
        uiManager = GetComponent<UIManager>();
        imageManager = GetComponent<ImageManager>();
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
        if (bonus != 5)
        {
            block.isBonus = true;
        }
        block.SetSpawnCoords();
        CheckBlockState();
    }

    public void ScorePositionEmpty(int x, int y)
    {
        scorePosition[x, y] = null;
    }

    public void PoolEnqueue(EObjectType type, GameObject obj)
    {
        poolingDict[type].Enqueue(obj);
    }

    public void SpawnScore(int x, int y)
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
        if (CheckMaxBlock() && CheckLine(x, y))
        {
            return false;
        }

        return CheckLine(x, y) || CheckScorePos(x, y);
    }

    private bool CheckScorePos(int x, int y)
    {
        if (scorePosition[x, y] != null)
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
            for (int j = 0; j < 5; j++)
            {
                if (blockPosition[i, j] == null)
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


        if (blockPosList.Count == 0)
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
        CheckBlockState();
    }

    private void CheckBlockState()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (CheckBlock(i, j) || CheckPlayer(i,j)) continue;
                blockPosition[i, j].SetBlockState();
            }
        }
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
                if (newBlock.isBonus)
                {
                    BonusActive();
                }
                newBlock.Despawn();
                SpawnScore(i, j);
            }
            CheckBonus();
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
                }
                newBlock.Despawn();
                SpawnScore(j, i);
            }
            CheckBonus();
        }
    }

    private void CheckBonus()
    {
        
        if (bonusCnt >= 5)
        {
            bonusCheck = new bool[5];
            bonusCnt = 0;
            for(int i = 0; i < 5; i++)
            {
                UI.ActiveBonusText(i, false);

            }

            StartCoroutine(BonusEffect());
        }
    }
    private void BonusActive()
    {
        if (bonusCnt >= 5) return;
        bonusCheck[bonusCnt] = true;
        UI.ActiveBonusText(bonusCnt, true);
        bonusCnt++;

    }

    private IEnumerator BonusEffect()
    {
        List<Block> topBlockList = new List<Block>();
        List<Block> leftBlockList = new List<Block>();
        List<Block> rightBlockList = new List<Block>();
        List<Block> downBlockList = new List<Block>();
        bool breakBool = false;
        isLoading = true;
        for (int i = 0; i < 5; i++)
        {
            if (player.yPos - i >= 0 && blockPosition[player.xPos, player.yPos - i] != null && blockPosition[player.xPos, player.yPos - i] != player)
            {
                downBlockList.Add(blockPosition[player.xPos, player.yPos - i]);
                blockPosition[player.xPos, player.yPos - i] = null;
            }
            if (player.yPos + i < 5 && blockPosition[player.xPos, player.yPos + i] != null && blockPosition[player.xPos, player.yPos + i] != player)
            {
                topBlockList.Add(blockPosition[player.xPos, player.yPos + i]);
                blockPosition[player.xPos, player.yPos + i] = null;

            }
            if (player.xPos - i >= 0 && blockPosition[player.xPos - i, player.yPos] != null && blockPosition[player.xPos - i, player.yPos] != player)
            {
                leftBlockList.Add(blockPosition[player.xPos - i, player.yPos]);
                blockPosition[player.xPos - i, player.yPos] = null;

            }
            if (player.xPos + i < 5 && blockPosition[player.xPos + i, player.yPos] != null && blockPosition[player.xPos + i, player.yPos] != player)
            {
                rightBlockList.Add(blockPosition[player.xPos + i, player.yPos]);
                blockPosition[player.xPos + i, player.yPos] = null;

            }

        }

        for (int i = 0; i < 5; i++)
        {
            if(i < topBlockList.Count)
            {
                topBlockList[i].Despawn();
                breakBool = true;
            }
            if (i < downBlockList.Count)
            {
                downBlockList[i].Despawn();
                breakBool = true;
            }
            if (i < rightBlockList.Count)
            {
                rightBlockList[i].Despawn();
                breakBool = true;
            }
            if (i < leftBlockList.Count)
            {
                leftBlockList[i].Despawn();
                breakBool = true;
            }
            if(!breakBool)
            {
                isLoading = false;
                yield break;
            }
            yield return new WaitForSeconds(0.5f);
        }
        isLoading = false;
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

    public bool CheckBlock(int x, int y)
    {
        if (x < 0 || x > 4 || y < 0 || y > 4) return true;
        if (blockPosition[x, y] == null)
        {
            return true;
        }
        return false;
    }

    public bool CheckPlayer(int x, int y)
    {
        if (x < 0 || x > 4 || y < 0 || y > 4) return true;
        if (blockPosition[x, y] == player)
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
