using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public enum GameState
{
    Playing,
    GameOver
}

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<BoardManager>();
            return _instance;
        }
    }

    UIManager UIManager;

    private static BoardManager _instance;

    public GameObject activeButton;

    [SerializeField]
    int size = 4;

    [SerializeField]
    Transform nodesParent;

    [SerializeField]
    float animationDuration = 0.2f;

    [SerializeField]
    GameState gameState;

    [SerializeField]
    TMP_Text Timer;

    bool hasAnyBlockMoved = false;

    int[] randomNumbers = { 2, 4 };

    int count = 0;

    int maxCount;

    private void Start()
    {
        gameState = GameState.Playing;

        for (int i = 0; i < 2; i++)
        {
            CreateRandomNumber();
        }
    }

    public void ActivateButton()
    {
        activeButton.SetActive(true);
    }

    void Update()
    {
        if (gameState.Equals(GameState.Playing))
        {
            if (
                Input.GetButtonDown("Left") // Left Arrow or "A" Key
            )
            {
                MoveAllToLeft();
            }
            else if (
                Input.GetButtonDown("Up") // Up Arrow or "W" Key
            )
            {
                MoveAllToUp();
            }
            else if (
                Input.GetButtonDown("Right") // Right Arrow or "D" Key
            )
            {
                MoveAllToRight();
            }
            else if (
                Input.GetButtonDown("Down") // Down Arrow or "S" Key
            )
            {
                MoveAllToDown();
            }
        }
    }

    public void AutoPlayActionsFor10Seconds()
    {
        StartCoroutine(AutoPlayFor10Seconds());
    }

    public void AutoPlayActionsFor30Seconds()
    {
        StartCoroutine(AutoPlayFor30Seconds());
    }

    public void AutoPlayActionsFor60Seconds()
    {
        StartCoroutine(AutoPlayFor60Seconds());
    }

    IEnumerator AutoPlayFor10Seconds()
    {
        maxCount = 3;
        StartCoroutine(CountdownFor10Seconds());
        while (count < maxCount)
        {
            yield return new WaitForSeconds(1);
            MoveAllToLeft();
            yield return new WaitForSeconds(1);
            MoveAllToDown();
            yield return new WaitForSeconds(1);
            MoveAllToRight();
            Debug.Log (count);
            count++;
        }
        count = 0;
    }

    IEnumerator AutoPlayFor30Seconds()
    {
        maxCount = 10;
        StartCoroutine(CountdownFor30Seconds());
        while (count < maxCount)
        {
            yield return new WaitForSeconds(1);
            MoveAllToLeft();
            yield return new WaitForSeconds(1);
            MoveAllToDown();
            yield return new WaitForSeconds(1);
            MoveAllToRight();
            Debug.Log (count);
            count++;
        }
        count = 0;
    }

    IEnumerator AutoPlayFor60Seconds()
    {
        maxCount = 20;
        StartCoroutine(CountdownFor60Seconds());
        while (count < maxCount)
        {
            yield return new WaitForSeconds(1);
            MoveAllToLeft();
            yield return new WaitForSeconds(1);
            MoveAllToDown();
            yield return new WaitForSeconds(1);
            MoveAllToRight();
            Debug.Log (count);
            count++;
        }
        count = 0;
    }

    IEnumerator CountdownFor10Seconds()
    {
        Timer.gameObject.SetActive(true);
        for (int i = 10; i >= 0; i--)
        {
            Timer.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        Timer.gameObject.SetActive(false);
    }

    IEnumerator CountdownFor30Seconds()
    {
        Timer.gameObject.SetActive(true);
        for (int i = 30; i >= 0; i--)
        {
            Timer.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        Timer.gameObject.SetActive(false);
    }

    IEnumerator CountdownFor60Seconds()
    {
        Timer.gameObject.SetActive(true);
        for (int i = 60; i >= 0; i--)
        {
            Timer.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        Timer.gameObject.SetActive(false);
    }

    public void MoveAllToLeft()
    {
        for (
            int y = 0;
            y < size;
            // f??r jede Zeile [0,1,2,3]
            y++ 
        )
        {
            for (
                int x = 1;
                x < size;
                // f??r jede Spalte au??er der Spalte ganz links [1,2,3]
                x++ 
            )
            {
                var currentNode = GetNodeFromBoard(x, y);

                if (
                    // Knoten hat keine Nummer im Inneren
                    currentNode.childCount.Equals(0) 
                ) continue;

                Transform targetNode = null;

                for (int z = x - 1; z >= 0; z--)
                {
                    var iterNode = GetNodeFromBoard(z, y);

                    if (
                        // Knoten hat eine Nummer im Inneren
                        !iterNode.childCount.Equals(0)
                    )
                    {
                        if (
                            AreTheNumbersEqual(currentNode, iterNode) &&
                            !iterNode
                                .GetChild(0)
                                .GetComponent<Number>()
                                .justCombined
                        )
                        {
                            targetNode = iterNode;
                            Move(currentNode, targetNode, true);
                            break;
                        } // iter ist nicht gleich, gehe einen Schritt nach rechts
                        else
                        {
                            targetNode = GetNodeFromBoard(z + 1, y);
                            if (
                                // nur wenn sie nicht benachbart ist
                                Mathf.Abs(x - z) > 1 
                            )
                            {
                                MoveToEmptyNode (currentNode, targetNode);
                                break;
                            }
                        }
                        break;
                    }
                }

                if (
                    // Wenn es kein Ziel gibt, w??hlen Sie das ganz linke
                    targetNode == null 
                )
                {
                    targetNode = GetNodeFromBoard(0, y);
                    MoveToEmptyNode (currentNode, targetNode);
                }
            }
        }

        CreateNumberAndCheckForGameOver();
    }

    public void MoveAllToUp()
    {
        for (
            int x = 0;
            x < size;
            //[0,1,2,3]
            x++ 
        )
        {
            for (
                int y = size - 2;
                y >= 0;
                //f??r jede Zeile au??er der obersten [2,1,0]
                y-- 
            )
            {
                var currentNode = GetNodeFromBoard(x, y);

                if (
                    currentNode.childCount.Equals(0) 
                ) continue;

                Transform targetNode = null;

                for (int z = y + 1; z < size; z++)
                {
                    var iterNode = GetNodeFromBoard(x, z);

                    if (
                        !iterNode.childCount.Equals(0)
                    )
                    {
                        if (
                            AreTheNumbersEqual(currentNode, iterNode) &&
                            !iterNode
                                .GetChild(0)
                                .GetComponent<Number>()
                                .justCombined
                        )
                        {
                            targetNode = iterNode;
                            Move(currentNode, targetNode, true);
                            break;
                        } // iter ist nicht gleich, geh einen runter
                        else
                        {
                            targetNode = GetNodeFromBoard(x, z - 1);
                            if (
                                Mathf.Abs(y - z) > 1 
                            )
                            {
                                MoveToEmptyNode (currentNode, targetNode);
                                break;
                            }
                        }
                        break;
                    }
                }

                if (
                    targetNode == null
                )
                {
                    targetNode = GetNodeFromBoard(x, size - 1);
                    MoveToEmptyNode (currentNode, targetNode);
                }
            }
        }

        CreateNumberAndCheckForGameOver();
    }

    public void MoveAllToRight()
    {
        for (
            int y = 0;
            y < size;
            //[0,1,2,3]
            y++ 
        )
        {
            for (
                int x = size - 2;
                x >= 0;
                //[2,1,0]
                x-- 
            )
            {
                var currentNode = GetNodeFromBoard(x, y);

                if (
                    currentNode.childCount.Equals(0) 
                ) continue;

                Transform targetNode = null;

                for (int z = x + 1; z < size; z++)
                {
                    var iterNode = GetNodeFromBoard(z, y);

                    if (
                        !iterNode.childCount.Equals(0) 
                    )
                    {
                        if (
                            AreTheNumbersEqual(currentNode, iterNode) &&
                            !iterNode
                                .GetChild(0)
                                .GetComponent<Number>()
                                .justCombined
                        )
                        {
                            targetNode = iterNode;
                            Move(currentNode, targetNode, true);
                            break;
                        } // iter ist nicht gleich, gehe einen Schritt nach links
                        else
                        {
                            targetNode = GetNodeFromBoard(z - 1, y);
                            if (
                                Mathf.Abs(x - z) > 1 
                            )
                            {
                                MoveToEmptyNode (currentNode, targetNode);
                                break;
                            }
                        }
                        break;
                    }
                }

                if (
                    targetNode == null 
                )
                {
                    targetNode = GetNodeFromBoard(size - 1, y);
                    MoveToEmptyNode (currentNode, targetNode);
                }
            }
        }

        CreateNumberAndCheckForGameOver();
    }

    public void MoveAllToDown()
    {
        for (
            int x = 0;
            x < size;
            //[0,1,2,3]
            x++ 
        )
        {
            for (
                int y = 1;
                y < size;
                // f??r jede Zeile au??er der untersten [1,2,3]
                y++ 
            )
            {
                var currentNode = GetNodeFromBoard(x, y);

                if (
                    currentNode.childCount.Equals(0) 
                ) continue;

                Transform targetNode = null;

                for (int z = y - 1; z >= 0; z--)
                {
                    var iterNode = GetNodeFromBoard(x, z);

                    if (
                        !iterNode.childCount.Equals(0) 
                    )
                    {
                        if (
                            AreTheNumbersEqual(currentNode, iterNode) &&
                            !iterNode
                                .GetChild(0)
                                .GetComponent<Number>()
                                .justCombined
                        )
                        {
                            targetNode = iterNode;
                            Move(currentNode, targetNode, true);
                            break;
                        } 
                        // iter ist nicht gleich, gehe einen Schritt nach oben
                        else
                        {
                            targetNode = GetNodeFromBoard(x, z + 1);
                            if (
                                Mathf.Abs(y - z) > 1 
                            )
                            {
                                MoveToEmptyNode (currentNode, targetNode);
                                break;
                            }
                        }
                        break;
                    }
                }

                if (
                    targetNode == null 
                )
                {
                    targetNode = GetNodeFromBoard(x, 0);
                    MoveToEmptyNode (currentNode, targetNode);
                }
            }
        }

        CreateNumberAndCheckForGameOver();
    }

    private void CreateNumberAndCheckForGameOver()
    {
        if (hasAnyBlockMoved && !gameState.Equals(GameState.GameOver))
        {
            Invoke(nameof(CreateRandomNumber), animationDuration);
            hasAnyBlockMoved = false;
        }
    }

    private void Move(Transform currentNode, Transform targetNode, bool merge)
    {
        hasAnyBlockMoved = true;

        var movingBlock = currentNode.GetChild(0);
        Transform targetBlock = null;
        int newValue = int.Parse(movingBlock.name) * 2;
        movingBlock.SetParent (transform);

        if (
            targetNode.childCount > 0 
        )
        {
            targetBlock = targetNode.GetChild(0);
            targetBlock.SetParent (transform);

            if (
                merge 
            )
            {
                GameObject newBlock =
                    NumberPool
                        .Instance
                        .GetPooledObject(newValue, targetNode, false);
                newBlock.GetComponent<Number>().justCombined = true;

                movingBlock
                    .DOMove(targetNode.position, animationDuration)
                    .OnComplete(() =>
                    {
                        UIManager.Instance.OnInput (newValue);
                        NumberPool.Instance.SetPooledObject (movingBlock);
                        NumberPool.Instance.SetPooledObject (targetBlock);

                        newBlock.GetComponent<Number>().justCombined = false;
                        newBlock.SetActive(true);
                        newBlock
                            .transform
                            .DOPunchScale(Vector3.one * 0.15f,
                            animationDuration,
                            0,
                            0f);
                    });
            } 
            else
            {
                movingBlock
                    .DOMove(targetNode.position, animationDuration)
                    .OnComplete(() =>
                    {
                        movingBlock.SetParent (targetNode);
                    });
            }
        }
    }

    void MoveToEmptyNode(Transform currentNode, Transform targetNode)
    {
        hasAnyBlockMoved = true;

        var movingBlock = currentNode.GetChild(0);
        movingBlock.SetParent (targetNode);
        movingBlock.DOMove(targetNode.position, animationDuration);
    }

    void CreateRandomNumber()
    {
        if (IsTableFilled())
        {
            return;
        }

        int randomNumber = randomNumbers[Random.Range(0, randomNumbers.Length)];
        bool hasCreated = false;

        while (!hasCreated)
        {
            int randomX = Random.Range(0, size);
            int randomY = Random.Range(0, size);

            Transform randomBlock = GetNodeFromBoard(randomX, randomY);

            if (
                randomBlock.childCount.Equals(0) 
            )
            {
                hasCreated = true;
                GameObject newBlock =
                    NumberPool
                        .Instance
                        .GetPooledObject(randomNumber, randomBlock, true);
                newBlock
                    .transform
                    .DOScale(Vector3.one, animationDuration)
                    .From(Vector3.zero);
            }
        }

        if (IsGameOver())
        {
            gameState = GameState.GameOver;
            UIManager.Instance.GameOver();
        }
    }

    bool AreTheNumbersEqual(Transform firstNode, Transform secondNode)
    {
        Transform firstBlock = firstNode.GetChild(0);
        Transform secondBlock = secondNode.GetChild(0);

        return firstBlock.name.Equals(secondBlock.name);
    }

    bool IsGameOver()
    {
        if (!IsTableFilled())
        {
            return false;
        }

        for (
            int x = 0;
            x < size;
            x++ 
        )
        {
            for (
                int y = 0;
                y < size;
                y++ 
            )
            {
                Transform currentNode = GetNodeFromBoard(x, y);

                if (
                    x + 1 < size 
                )
                {
                    Transform rightNode = GetNodeFromBoard(x + 1, y);

                    if (
                        AreTheNumbersEqual(currentNode, rightNode) 
                    )
                    {
                        return false;
                    }
                }
                if (
                    y + 1 < size // check for up node
                )
                {
                    Transform upNode = GetNodeFromBoard(x, y + 1);

                    if (
                        AreTheNumbersEqual(currentNode, upNode) 
                    )
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    bool IsTableFilled()
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (
                    GetNodeFromBoard(x, y).childCount.Equals(0) // if any node has a number inside, then the game is not over
                )
                {
                    return false;
                }
            }
        }

        return true;
    }

    private Transform GetNodeFromBoard(int x, int y)
    {
        return nodesParent.transform.GetChild(y * size + x);
    }
}
