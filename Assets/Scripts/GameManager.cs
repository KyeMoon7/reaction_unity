using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject startPromptObject; // Displayed while idle, waiting for first trigger

    [SerializeField]
    private GameObject blinkerObject; // Displayed ONLY during the reaction window

    [SerializeField]
    private GameObject loseObject; // Blinked on loss

    [Header("Settings")]
    [SerializeField]
    private float minWaitTime = 2.0f;

    [SerializeField]
    private float maxWaitTime = 5.0f;

    [SerializeField]
    private float initialReactionDuration = 1.0f;

    [Header("Loss Blink")]
    [SerializeField]
    private float lossCooldownTime = 2.0f; // Total time spent in cooldown

    [SerializeField]
    private float blinkInterval = 0.15f; // Half-period of the blink

    float currentReactionDuration;
    private float stateTimer;
    private float cooldownTimer;
    private float blinkTimer;
    private bool blinkOn;

    private Vector3 currentScaleVec;
    private Vector3 initalScaleVec;

    private enum GameState
    {
        Idle, // Waiting for first trigger (Show Start Prompt)
        Waiting, // Random delay before the window opens (nothing shown)
        Active, // Reaction window open (Show Blinker Object)
        Cooldown, // After loss (Blink Lose Object)
    }

    private GameState currentState = GameState.Idle;

    protected  void Awake()
    {
        
    }

    void Start()
    {
        initalScaleVec = blinkerObject.transform.localScale;
        ResetGame();
    }

    public  void Trigger()
    {
        // Ignore input during cooldown
        if (currentState == GameState.Cooldown)
            return;

        switch (currentState)
        {
            case GameState.Idle:
                StartRound();
                break;
            case GameState.Waiting:
                Lose(); // Clicked too early
                break;
            case GameState.Active:
                WinRound(); // Clicked inside the window
                break;
        }
    }

    private void StartRound()
    {
        currentScaleVec = initalScaleVec;
        blinkerObject.transform.localScale = currentScaleVec;

        // Increase difficulty
        currentReactionDuration *= 0.75f;

        currentState = GameState.Waiting;
        stateTimer = Random.Range(minWaitTime, maxWaitTime);

        // Nothing is shown during the wait — the player must not click yet
        SetVisibility(startPromptObject, false);
        SetVisibility(loseObject, false);
        SetVisibility(blinkerObject, false);
    }

    protected  void Update()
    {
        float dt = Time.deltaTime;

        // --- COOLDOWN / BLINK LOGIC ---
        if (currentState == GameState.Cooldown)
        {
            cooldownTimer -= dt;
            blinkTimer -= dt;

            if (blinkTimer <= 0f)
            {
                blinkOn = !blinkOn;
                blinkTimer = blinkInterval;
                SetVisibility(loseObject, blinkOn);
            }

            if (cooldownTimer <= 0)
            {
                // Cooldown finished, return to Idle
                currentState = GameState.Idle;

                SetVisibility(loseObject, false);
                SetVisibility(blinkerObject, true);
                SetVisibility(startPromptObject, true);

                // Reset difficulty
                currentReactionDuration = initialReactionDuration;
            }
            return;
        }

        // --- WAITING LOGIC ---
        if (currentState == GameState.Waiting)
        {
            stateTimer -= dt;
            if (stateTimer <= 0)
            {
                BeginWindow();
            }
        }

        // --- ACTIVE (REACTION WINDOW) LOGIC ---
        if (currentState == GameState.Active)
        {
            stateTimer += dt;

            float t = Mathf.Clamp01(stateTimer / currentReactionDuration);

            // Calculate scale without 'new'

            currentScaleVec = initalScaleVec * (1f - t);

            blinkerObject.transform.localScale = currentScaleVec;

            if (t >= 1f)
            {
                blinkerObject.SetActive(false);
                Lose(); // Window closed without a click
            }
        }
    }

    private void BeginWindow()
    {
        currentState = GameState.Active;
        stateTimer = 0f;

        // Show the blinker only now — this is the player's cue to click
        SetVisibility(blinkerObject, true);
    }

    private void WinRound()
    {
        blinkerObject.SetActive(false);
        StartRound(); // Immediately start next round
    }

    private void Lose()
    {
        currentState = GameState.Cooldown;
        cooldownTimer = lossCooldownTime;

        // Start the blink cycle
        blinkTimer = blinkInterval;
        blinkOn = true;

        SetVisibility(blinkerObject, false);
        SetVisibility(startPromptObject, false);
        SetVisibility(loseObject, true);
    }

    private void ResetGame()
    {
        currentState = GameState.Idle;
        currentReactionDuration = initialReactionDuration;

        currentScaleVec = initalScaleVec;

        blinkerObject.transform.localScale = initalScaleVec;

        SetVisibility(loseObject, false);
        SetVisibility(blinkerObject, true);
        SetVisibility(startPromptObject, true);
    }

    // Helper to safely toggle active state
    private void SetVisibility(GameObject obj, bool active)
    {
        if (obj != null)
            obj.SetActive(active);
    }
}
