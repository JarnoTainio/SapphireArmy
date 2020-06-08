using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPiece : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    [Header("Effects")]
    public Animation effect;
    public SpriteRenderer effectSprite;
    public Sprite[] icons;

    [Header("Icons")]
    public GameObject armorIcon;

    [Header("Sound")]
    public AudioSource audioSource;

    [Header("Unit")]
    public Unit unit;
    public int identifier;

    [Header("Animation")]
    public bool moving;
    public bool animationRunning;

    [Header("FloatingText")]
    public FloatingText floatingTextPrefab;

    Vector2 target;
    float moveSpeed;
    Point lastMoveOrder;
    float animationWait;

    public bool isActive;

    private void Start()
    {
        animationRunning = false;
    }

    void Update()
    {
        if (animationWait > 0)
        {
            animationWait -= Time.deltaTime;
            if (animationWait <= 0)
            {
                animationRunning = false;
            }
        }
        if (moving)
        {
            if (!audioSource.isPlaying && moveSpeed < 10f)
            {
                audioSource.clip = unit.walkingSounds[Random.Range(0, unit.walkingSounds.Length)];
                audioSource.Play();
            }
            if (transform.localPosition.Equals(target))
            {
                animator.SetBool("Walking", false);
                moving = false;
                animationRunning = false;
            }
            else
            {
                transform.localPosition = Vector2.MoveTowards(transform.localPosition, target, Time.deltaTime * moveSpeed);
            }
        }
        armorIcon.SetActive(unit.armor > 0);
    }

    public void SetUnit(Unit unit)
    {
        this.unit = unit;
        unit.Init();
        spriteRenderer.sprite = unit.sprite;
        if (unit.animator != null)
        {
            //animator = unit.animator;
        }
        foreach(Passive passive in unit.passives)
        {
            if (passive != null)
            {
                passive.ownerPiece = this;
            }
        }
        spriteRenderer.material.color = unit.hasActionLeft ? Color.white : Color.grey;
    }

    public void MoveTo(CombatMap map, Point target, float moveSpeed = 3f)
    {
        lastMoveOrder = target;
        this.target = target.ToVector2();
        this.moveSpeed = moveSpeed;
        unit.MoveTo(map, target);

        if (target.z != 0)
        {
            unit.energy--;
        }

        if (moveSpeed > 0)
        {
            animator.SetBool("Walking", true);
            moving = true;
        }
        else
        {
            transform.localPosition = new Vector2(target.x, target.y);
        }
    }

    public void UndoMove()
    {
        if (lastMoveOrder.z != 0)
        {
            unit.energy++;
        }
    }

    public List<Point> GetMoves(CombatMap map)
    {
        return unit.GetMoves(map);
    }
    public void PerformingAction(Action action)
    {
        audioSource.PlayOneShot(action.sound);
    }

    public void Damage(Unit attacker, int amount)
    {
        animationRunning = true;
        animationWait = .5f;
        int damaged = unit.Damage(attacker, amount);
        if (damaged > 0)
        {
            FloatingText(Color.red, damaged.ToString(), 10 + amount / 2);
            animator.SetBool("Damage", true);

            if (unit.life == 0)
            {
                audioSource.PlayOneShot(unit.dyingSound);
            }
            else
            {
                audioSource.PlayOneShot(unit.woundedSound);
            }
        }
        else
        {
            FloatingText(Color.white, "Blocked");

            animator.SetBool("Block", true);
            audioSource.PlayOneShot(unit.blockingSound);
        }
    }

    public void Effect(Attribute attribute, int value)
    {
        if (value == 0)
        {
            animator.SetBool("Block", true);
        }
        else if (value < 0)
        {
            int effected = unit.ModifyAttribute(attribute, value);
            if (effected != 0)
            {
                animator.SetBool("Damage", true);

            }
        }
        else
        {
            int effected = unit.ModifyAttribute(attribute, value);
            if (effected != 0)
            {
                animator.SetBool("Buff", true);
                effectSprite.sprite = icons[(int)attribute];
                effect.Play();

                if (attribute == Attribute.Life)
                {
                    FloatingText(Color.green, "+" + effected, 10 + effected / 2, 0f);
                }
                else if (attribute == Attribute.Energy)
                {
                    FloatingText(Color.yellow, effected.ToString(), 9 + effected, 0f);
                    audioSource.PlayOneShot(unit.restingSound);
                }
                else if (attribute == Attribute.Armor)
                {
                    FloatingText(Color.white, effected.ToString(), 9 + effected, 0f);
                }
            }
        }
    }

    public void Animation(ActionType type)
    {
        EndAnimation();
        animationRunning = true;
        animationWait = .5f;

        if (type == ActionType.Attack)
        {
            animator.SetBool("Melee", true);
        }
        else if (type == ActionType.Buff)
        {
            animator.SetBool("Buff", true);
            //effect.Play();
        }
        
    }

    public void EndAnimation()
    {
        animator.SetBool("Melee", false);
        animator.SetBool("Buff", false);
        animator.SetBool("Damage", false);
        animator.SetBool("Block", false);
        animator.SetBool("Walking", false);
        animationRunning = false;
        //baseColor = unit.hasActionLeft ? Color.white : Color.black;
        //spriteRenderer.color = baseColor;
    }

    public void Die()
    {
        gameObject.SetActive(false);
    }

    public void EndTurn()
    {
        spriteRenderer.material.color = Color.gray;
        unit.EndTurn();
    }

    public IEnumerator NewRound(CombatMap map, bool firstRound = false)
    {
        Debug.Log(unit.position + " start");
        isActive = true;

        spriteRenderer.material.color = Color.yellow;
        map.RemoveMarkings();
        
        if (!firstRound)
        {
            if (unit.hasActionLeft)
            {
                Action action = unit.actions[0];
                List<TargetTile> targets = action?.GetTargets(map, false);
                if (targets != null && action.CanUse() && action.GetTargets(map, false).Count > 0)
                {
                    map.Mark(unit.position, Marking.Orange);
                    map.Mark(targets[0].point, Marking.Red);
                    yield return new WaitForSeconds(.25f);

                    map.unitManager.PerformAction(this, action, targets[0].point);
                    yield return new WaitForSeconds(.75f);

                    map.RemoveMarkings();
                }
                else if (unit.energy < unit.maxEnergy)
                {
                    Effect(Attribute.Energy, 1);
                    yield return new WaitForSeconds(.5f);
                }
            }

        }
        spriteRenderer.material.color = Color.white;
        bool triggered = unit.NewRound(map, firstRound);
        if (triggered)
        {
            yield return new WaitForSeconds(.5f);
        }
        Debug.Log(unit.position + " done");
        isActive = false;
    }

    private void FloatingText(Color color, string message, int font = 10, float speed = 1f)
    {
        FloatingText text = Instantiate(floatingTextPrefab);
        text.SetPosition(transform.position);

        text.SetColor(color);
        text.SetText(message);
        text.SetFont(font);
        text.SetLifeTime(1f);
        text.SetVelocity(new Vector2(0, speed));
    }
}
