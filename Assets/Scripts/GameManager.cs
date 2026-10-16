using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField]
    private GameObject startPromptObject;  // Displayed when waiting for initial start
    [SerializeField]
    private GameObject shrinkingObject;    // Displayed during Wait/Shrink phases
    [SerializeField]
    private GameObject loseObject;         // Displayed on Loss

    [Header("Settings")]
    [SerializeField]
    private float minWaitTime = 2.0f; 
    [SerializeField]
    private float maxWaitTime = 5.0f; 

    [SerializeField]
    private float initialReactionDuration = 1.0f; 
  

    
    [SerializeField]
    private float lossCooldownTime = 2.0f; 

    float currentReactionDuration; 
    private float stateTimer;
    private float cooldownTimer; 

    // Reusable vector to avoid memory allocation
    private Vector3 currentScaleVec;
    private Vector3 initalScaleVec; 

    private enum GameState
    {
        Idle,        // Waiting for first trigger (Show Start Prompt)
        Waiting,     // Random delay before shrink (Show Shrinking Object)
        Active,      // Shrinking phase (Show Shrinking Object)
        Cooldown     // After loss (Show Lose Object)
    }

    private GameState currentState = GameState.Idle;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        initalScaleVec = shrinkingObject.transform.localScale;
        ResetGame();
    }

    public void Trigger()
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
                Lose(); 
                break;
            case GameState.Active:
                WinRound(); 
                break;
            case GameState.Cooldown:
                // Handled by timer, no trigger needed
                break;
        }
    }

    private void StartRound()
    {
        // Increase difficulty
        currentReactionDuration *= 0.75f;

        currentState = GameState.Waiting;
        stateTimer = Random.Range(minWaitTime, maxWaitTime);

        // Update Visibility: Show Shrinking Object, Hide others
        SetVisibility(startPromptObject, false);
        SetVisibility(loseObject, false);
        SetVisibility(shrinkingObject, true);

        // Reset shrinking object scale
        currentScaleVec = initalScaleVec;
        shrinkingObject.transform.localScale = currentScaleVec;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // --- COOLDOWN LOGIC ---
        if (currentState == GameState.Cooldown)
        {
            cooldownTimer -= dt;
            if (cooldownTimer <= 0)
            {
                // Cooldown finished, return to Idle
                currentState = GameState.Idle;
                
                // Update Visibility: Show Start Prompt, Hide others
                SetVisibility(loseObject, false);
                SetVisibility(shrinkingObject, false);
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
                BeginShrinking();
            }
        }

        // --- ACTIVE (SHRINKING) LOGIC ---
        if (currentState == GameState.Active)
        {
            stateTimer += dt;

            float t = Mathf.Clamp01(stateTimer / currentReactionDuration);
            
            // Calculate scale without 'new'

            currentScaleVec = initalScaleVec * (1f - t);
            
            shrinkingObject.transform.localScale = currentScaleVec;

            if (t >= 1f)
            {
                shrinkingObject.SetActive(false);
                Lose(); // Too late
            }
        }
    }

    private void BeginShrinking()
    {
        currentState = GameState.Active;
        stateTimer = 0f;
        
        // Visibility remains the same (Shrinking Object is already visible)
        // But we ensure it's active just in case
        shrinkingObject.SetActive(true);
    }

    private void WinRound()
    {
        initalScaleVec = initalScaleVec * 1.25f;
        shrinkingObject.SetActive(false);
        StartRound(); // Immediately start next round
    }

    private void Lose()
    {
        currentState = GameState.Cooldown;
        cooldownTimer = lossCooldownTime;
        
        // Update Visibility: Show Lose Object, Hide others
        SetVisibility(shrinkingObject, false);
        SetVisibility(startPromptObject, false);
        SetVisibility(loseObject, true);
    }

    private void ResetGame()
    {
        currentState = GameState.Idle;
        currentReactionDuration = initialReactionDuration;

        // Update Visibility: Show Start Prompt
        SetVisibility(loseObject, false);
        SetVisibility(shrinkingObject, false);
        SetVisibility(startPromptObject, true);

        // Reset shrinking object scale for future use
        currentScaleVec = initalScaleVec;
        if (shrinkingObject != null)
        {
            shrinkingObject.transform.localScale = currentScaleVec;
        }
    }

    // Helper to safely toggle active state
    private void SetVisibility(GameObject obj, bool active)
    {
        if (obj != null)
            obj.SetActive(active);
    }
}