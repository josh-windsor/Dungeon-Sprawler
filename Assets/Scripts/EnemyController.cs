using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    private string _enemyTypeName;
    private float _startHp;
    private float _hp;
    private float _attackDamage;
    private float _attackChargeTime;
    private float _attackSpeedMin;
    private float _attackSpeedMax;
    private int _timeReward;
    private int _coinReward;
    private bool _dead;
    private bool _dying;
    private const float DeathSpeed = 2.5f;

    private Renderer _renderer;

    private UiController _uIController;
    private PlayerController _player;


    public void Instantiate(string iName, float iHp, float iAttackDamage, float iAttackChargeTime, float iAttackSpeedMin, float iAttackSpeedMax, int iTimeReward, int iCoinReward){
		_enemyTypeName = iName;
		_startHp = iHp;
		_hp = iHp;
        _attackDamage = iAttackDamage;

        for (int i = 0; i < GameController.PlayerStats.UpgradesPurchased[0]; i++)
        {
            _attackDamage -= Upgrades.UpgradesList[0][i].SkillIncrease;
        }
        for (int i = 0; i < GameController.PlayerStats.UpgradesPurchased[0]; i++)
        {
            if (Upgrades.UpgradesList[0][i].SkillName == "Defence")
            {
                _attackDamage -= Upgrades.UpgradesList[0][i].SkillIncrease;
            }
        }



        _attackChargeTime = iAttackChargeTime;
        _attackSpeedMin = iAttackSpeedMin;
		_attackSpeedMax = iAttackSpeedMax;
        _timeReward = iTimeReward;
        _coinReward = iCoinReward;

        _renderer = GetComponent<Renderer>();
        _uIController = GameObject.Find("UI").GetComponent<UiController>();
        _player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    public void StartEnemy()
    {
        _uIController.ShowEnemyInfo(_enemyTypeName);
        StartCoroutine(AttackCycle());
    }

    public void TakeDamage(float iDamage)
    {
        _hp -= iDamage;
        if (_hp <= 0 && !_dead)
        { 
            Die();
        }
        _uIController.UpdateHealthBar(_hp / _startHp);
    }

    private void Die()
    {
        _dead = true;
        _uIController.UpdateTimer(_timeReward);
        _uIController.UpdateCoins(_coinReward);
        UiController.EnemiesDefeated++;
        UiController.SectionsFinished++;
    }

    private void Update()
    {
        if (_dead && !_dying)
        {
            _dying = true;
            StopAllCoroutines();
            StartCoroutine(FadeDeath());
        }
    }

    //Replace with dying animation but leave for player death
    private IEnumerator FadeDeath()
    {
        while (_renderer.material.color.a > 0)
        {
            _renderer.material.color = new Color(_renderer.material.color.r, _renderer.material.color.g, _renderer.material.color.b, _renderer.material.color.a - (Time.deltaTime * DeathSpeed));
            yield return null;
        }
        Destroy(gameObject);
    }

    private IEnumerator AttackCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(_attackChargeTime);
            _renderer.material.color = Color.yellow;
            yield return new WaitForSeconds(Random.Range(_attackSpeedMin, _attackSpeedMax));
            _renderer.material.color = Color.red;
            if (PlayerController.Parrying)
            {

                _renderer.material.color = Color.green;
                PlayerController.PlayerAnimator.SetTrigger("Parry");
                PlayerController.ResetAttack();
                yield return new WaitForSeconds(0.5f);


            }
            else if (PlayerController.Defending)
            {
                PlayerController.PlayerAnimator.SetTrigger("BlockHit");
            }
            else
            {
                _player.LockAnimations(0.75f);
                PlayerController.PlayerAnimator.SetInteger("Attack", 0);
                _uIController.TakeDamage(_attackDamage);
            }
            yield return new WaitForSeconds(0.1f);
            _renderer.material.color = Color.white;
        }
    }
}
