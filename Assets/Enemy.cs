using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    enum State
    {
        Entering,
        InBattle,
        Exiting,
    }

    public Text textAttackIndicator;
    public List<Sprite> enemySprites;

    private List<List<int>> damagePatterns = new List<List<int>>
    {
        new List<int>{ 7, 7, 7 }, // slime
        new List<int>{ 10, 8, 10, 8, 10, 8, 10, 8, 10 }, // skelly
        new List<int>{ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 }, // alien
        new List<int>{ 20, 30, 40, 50, 60, 70, 80, 90, 100, 200, 400, 800, 1000 } // dragon
    };
    private List<int> attackIntervals = new List<int>
    {
        15,
        10,
        5,
        12
    };

    private int counter;
    private int enemyIdx;
    private int moveIdx;
    private bool processingNextMove;
    private State state;

    // Start is called before the first frame update
    void Start()
    {
        enemyIdx = 0;
        moveIdx = 0;
        counter = attackIntervals[enemyIdx];
        processingNextMove = false;
        EnemyEnter();
    }

    public void StartGame()
    {
        enemyIdx = 0;
        moveIdx = 0;
        counter = attackIntervals[enemyIdx];
        processingNextMove = false;
        EnemyEnter();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.InBattle)
        {
            if (!processingNextMove)
            {
                processingNextMove = true;
                Invoke("CountDownOrDealDamage", 1f);
            }

            // force counter to become 1 second, effectively dealing damage immediately
            if (Input.GetKeyDown(KeyCode.Space))
                counter = 1;
        }
        else if (state == State.Entering)
        {
            transform.Translate(Vector2.left * 7 * Time.deltaTime);
            if (transform.position.x <= 5)
                state = State.InBattle;
        }
        else if (state == State.Exiting)
        {
            transform.Translate(Vector2.right * 7 * Time.deltaTime);
            if (transform.position.x >= 15)
            {
                GameController.Instance.chooseCardMenu.Setup();
                EnemyEnter();
            }
        }
    }
    
    // called every second. does a countdown or deal damage to player
    void CountDownOrDealDamage()
    {
        if (counter > 1) // count down
        {
            counter--;
        }
        else // deal damage
        {
            GameController.Instance.TakeDamage(damagePatterns[enemyIdx][moveIdx]);

            // reset stuff
            moveIdx++;
            if (moveIdx == damagePatterns[enemyIdx].Count) // if at last move of enemy
            {
                if (enemyIdx == damagePatterns.Count - 1) // if at last move of last enemy
                {
                    moveIdx--; // repeat last move indefinitely
                }
                else
                {
                    moveIdx = 0;
                    enemyIdx++;
                    EnemyExit();
                }
            }

            counter = attackIntervals[enemyIdx];
        }
        
        if (state == State.InBattle)
            UpdateAttackIndicatorDisplay(damagePatterns[enemyIdx][moveIdx], counter);

        processingNextMove = false;
    }

    void UpdateAttackIndicatorDisplay(int damage, int time)
    {
        textAttackIndicator.text = $"{damage} in {time}s";
    }

    void EnemyEnter()
    {
        state = State.Entering;
        textAttackIndicator.text = "";
        GetComponent<SpriteRenderer>().sprite = enemySprites[enemyIdx];
    }

    void EnemyExit()
    {
        state = State.Exiting;
        textAttackIndicator.text = "";
        GameController.Instance.reviveCount = 0;
    }
}
