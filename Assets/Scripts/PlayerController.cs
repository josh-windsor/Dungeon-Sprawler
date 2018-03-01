using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DigitalRubyShared;
using MORPH3D;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static PlayerController _thisPlayerController;
    public FingersScript FingersScript;
    public static Animator PlayerAnimator;
    public static Animation PlayerAnimation;
    public static M3DCharacterManager CharacterManager;


    public static bool Dead;

    public static bool Defending;
    public static bool Parrying;

    private static readonly float _chargeTime = 5.0f;
    private static float _chargeTimer;

    private static bool _dying;
    private static int _attackChain;
    private static bool _canChain;
    private static bool _isStunned;
    private static LevelController _levelController;

    private static float _hitDamage = 0.4f;


    private readonly Dictionary<Func<int, bool>, Action> _itemSwitch = new Dictionary<Func<int, bool>, Action>
    {
        {x => x < 10, () => _thisPlayerController.AttackEnemy()},
        {x => x < 20, OpenChest}
    };










    // Use this for initialization
    private void Start()
    {
        CharacterManager = GetComponent<M3DCharacterManager>();
        _levelController = GameObject.Find("LevelController").GetComponent<LevelController>();
        Dead = false;
        Defending = false;
        Parrying = false;
        _dying = false;
        _attackChain = 0;
        _canChain = false;
        _isStunned = false;

        if (_thisPlayerController != null)
        {
            Destroy(this);
        }
        _thisPlayerController = this;
        SwipeGestureRecognizer downSwipe = new SwipeGestureRecognizer
        {
            Direction = SwipeGestureRecognizerDirection.Down
        };
        downSwipe.Updated += Defend;
        FingersScript.AddGesture(downSwipe);

        SwipeGestureRecognizer upSwipe = new SwipeGestureRecognizer
        {
            Direction = SwipeGestureRecognizerDirection.Up
        };
        upSwipe.Updated += HandleAction;
        FingersScript.AddGesture(upSwipe);

        LongPressGestureRecognizer longPress = new LongPressGestureRecognizer
        {
            MinimumDurationSeconds = 0.2f
        };
        longPress.Updated += HeavyAtack;
        FingersScript.AddGesture(longPress);
        PlayerAnimator = GetComponent<Animator>();
        PlayerAnimation = GetComponent<Animation>();
        UpgradeController.LoadAllUpgradesMg();

        switch (GameController.PlayerStats.UpgradesPurchased[1])
        {
            case 1:
                _hitDamage += 0.2f;
                break;
            case 2:
                _hitDamage += 0.5f;
                goto case 1;
               
        }

        if (GameController.PlayerStats.UpgradesPurchased[1] == 1)
        {
            
        }

    }

    // Update is called once per frame
    private void Update()
    {

        if (Dead && !_dying)
        {
            _dying = true;
            StopAllCoroutines();
            StartCoroutine(FadeDeath());

        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            UiController.PauseGame(true);
        }
    }



    private IEnumerator Parry()
    {
        Parrying = true;

        PlayerAnimator.SetTrigger("Block");

        yield return new WaitForSeconds(0.3f);
        Parrying = false;

    }

    //Replace with dying animation but leave for player death
    private IEnumerator FadeDeath()
    {
        PlayerAnimator.SetTrigger("Die");
        yield return new WaitForSeconds(2f);

    }


    private void HandleAction(GestureRecognizer gesture, ICollection<GestureTouch> touches)
    {
        if (gesture.State == GestureRecognizerState.Began && !UtilityVars.ScrollingInProgress && !UtilityVars.Paused &&
            !UtilityVars.GameOver && !_dying)
        {
            if (UtilityVars.CurrentItem.Value != null)
            {
                Defending = false;

                _itemSwitch.First(sw => sw.Key(UtilityVars.CurrentItem.Key)).Value();
            }
            else
            {
                _levelController.UpdateSpawn();
            }
        }
    }

    private void AttackEnemy()
    {
        Defending = false;

        if (_attackChain == 0 && !_isStunned)
        {
            StartCoroutine(Attack1());
        }
        //if within chain time
        else if (_canChain)
        {
            if (_attackChain == 1)
            {
                StartCoroutine(Attack2());
            }
            else if (_attackChain == 2)
            {
                StartCoroutine(Attack3());
            }
        }
    }


    private IEnumerator Attack1()
    {
        StopAllCoroutines();
        _canChain = false;
        PlayerAnimator.SetInteger("Attack", 1);
        _attackChain = 1;
        StartCoroutine(ChainWindow(0.1f, 0.8f));
        StartCoroutine(LockMovementAndAttack(0.4f));
        yield return null;
    }

    private IEnumerator Attack2()
    {
        StopAllCoroutines();
        _canChain = false;
        PlayerAnimator.SetInteger("Attack", 2);
        _attackChain = 2;
        StartCoroutine(ChainWindow(0.1f, 0.8f));
        StartCoroutine(LockMovementAndAttack(0.4f));
        yield return null;
    }

    private IEnumerator Attack3()
    {
        StopAllCoroutines();
        _canChain = false;
        PlayerAnimator.SetInteger("Attack", 3);
        _attackChain = 3;
        StartCoroutine(LockMovementAndAttack(0.4f));
        yield return null;
    }

    public IEnumerator ChainWindow(float timeToWindow, float chainLength)
    {
        yield return new WaitForSeconds(timeToWindow);
        _canChain = true;
        PlayerAnimator.SetInteger("Attack", 0);
        yield return new WaitForSeconds(chainLength);
        _canChain = false;
    }

    public void LockAnimations(float pauseTime)
    {
        StartCoroutine(LockMovementAndAttack(pauseTime));
    }

    public IEnumerator LockMovementAndAttack(float pauseTime)
    {
        _isStunned = true;
        yield return new WaitForSeconds(pauseTime);
        PlayerAnimator.SetInteger("Attack", 0);
        _canChain = false;
        _isStunned = false;
        //small pause to let blending finish
        yield return new WaitForSeconds(0.1f);
        _attackChain = 0;
    }

    public static void ResetAttack()
    {
        PlayerAnimator.SetInteger("Attack", 0);
        _attackChain = 0;
    }


    private static void DealDamage(float damage)
    {
        if (UtilityVars.CurrentItem.Value != null)
        {
            UtilityVars.CurrentItem.Value.GetComponent<EnemyController>().TakeDamage(damage);
        }
    }

    private static void OpenChest()
    {
        //OPENING ANIMATION
        UtilityVars.CurrentItem.Value.GetComponent<ChestController>().Open();
    }


    private void Defend(GestureRecognizer gesture, ICollection<GestureTouch> touches)
    {
        if (gesture.State == GestureRecognizerState.Began && !Defending && !UtilityVars.ScrollingInProgress &&
            UtilityVars.CurrentItem.Value != null && !UtilityVars.Paused && !UtilityVars.GameOver && !_dying)
        {
            PlayerAnimator.SetInteger("Attack", 0);
            _attackChain = 0;
            Defending = true;
            StartCoroutine(Parry());
        }
    }


    private void HeavyAtack(GestureRecognizer gesture, ICollection<GestureTouch> touches)
    {
        if (gesture.State == GestureRecognizerState.Ended)
        {
            _chargeTimer = 0;
        }
        if (!UtilityVars.ScrollingInProgress && !UtilityVars.Paused && !UtilityVars.GameOver && !_dying &&
            UtilityVars.CurrentItem.Key < 10)
        {
            if (gesture.State == GestureRecognizerState.Began)
            {
                Defending = false;
            }
            if (gesture.State == GestureRecognizerState.Executing)
            {
                _chargeTimer += 0.1f;
                if (_chargeTimer >= _chargeTime)
                {

                    DealDamage(5f);
                    _chargeTimer = 0;
                }
            }
        }
    }
    public void WeaponSwitch()
    {
        UpgradeController.Sword.transform.SetParent(GameObject.Find("rHandAttachmentPoint").transform);
    }


    public void Hit()
    {
        DealDamage(_hitDamage);
    }

    public void FootR()
    {
    }

    public void FootL()
    {
    }
}

