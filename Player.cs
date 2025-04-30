using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class Player
{
    Sprite sprite;
    public Vector2 pos;
    Vector2 vel = Vector2.Zero, newVel = Vector2.Zero, externalVel = Vector2.Zero;
    public CharacterState state, lastState;

    float accel = 50, moveSpeed = 250;
    bool translating;
    public event Action<Stance> StanceChanged;
    float attackSpeedMult = .3f;
    bool attackQueued;
    Vector2 lastRequestDir = Vector2.Zero, inputVector = Vector2.Zero;
    float attackTimer, rollTimer;
    float attackDur = .15f, rollDur = .3f;
    float rollSteerAccel = 1f, rollStartSpeed = 8f;
    float delta;


    public void Initialize()
    {
        //foreach(Texture2DReference texture2DReference in textures) GameManager.texture2Ds.Add(texture2DReference);
        state.stance = Stance.Walking;
        state.velocity = Vector2.Zero;
        sprite = new("Player");
        Camera.SetTarget(ref sprite);
    }


    public void Update(float _delta)
    {
        //Debug.WriteLine(state.velocity);
        delta = _delta;
        GetInput();
        HandleStates();
        UpdateVelocity();
        Move();
        FootStepSounds();
    }


    #region Movement

    public void ChangeStance(Stance newStance)
    {
        if(state.stance == newStance) return;
        state.stance = newStance;
        
        switch (state.stance)
        {
            case Stance.Walking:
                //moveSpeed = 1.5f;
            break;

            case Stance.Attacking:
                attackQueued = false;
                //attackDir = ((Vector2)Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition) - (Vector2)transform.position).normalized;
                SetVelocity(Vector2.Zero);
                attackTimer = attackDur + .1f;
            break;

            case Stance.Rolling:
                SetVelocity(lastRequestDir * rollStartSpeed);
                rollTimer = rollDur;
            break;
        }
        StanceChanged?.Invoke(state.stance);
    }

    public void UpdateVelocity()
    {
        float moveSpeed = this.moveSpeed;// * Camera.zoom;
        Vector2 nextVel = vel;
        attackTimer -= delta;
        rollTimer -= delta;

        nextVel += externalVel;
        externalVel = Vector2.Zero;

        if(newVel != Vector2.Zero)
        {
            nextVel = newVel;
            newVel = Vector2.Zero;
        }

        switch(state.stance)
        {
            case Stance.Walking:
            // Standing Movement (same as the crouching movement except with standing speed, might find a way to combine the two later)
            {
                Vector2 targetVelocity = inputVector * moveSpeed;

                // Slowly lerp to the target velocity over a few ticks, this is our acceleration.
                Vector2 newVel = Vector2.Lerp(nextVel, targetVelocity, 1f - MathF.Exp(-accel * delta));

                nextVel = newVel;
            }
            break;

            case Stance.Attacking:
            // Standing Movement (same as the crouching movement except with standing speed, might find a way to combine the two later)
            {
                Vector2 targetVelocity = inputVector * moveSpeed * attackSpeedMult;

                // Slowly lerp to the target velocity over a few ticks, this is our acceleration.
                Vector2 newVel = Vector2.Lerp(nextVel, targetVelocity, 1f - MathF.Exp(-accel * delta));

                nextVel = newVel;
            }
            break;

            case Stance.Rolling:
            // Steer.
            {
                float curSpeed = nextVel.Length();

                // Target velocity is the player's movement direction, at the current speed.
                Vector2 tarVelocity = lastRequestDir * nextVel.Length();
                Vector2 steerVel = nextVel;

                Vector2 steerForce = (tarVelocity - steerVel) * rollSteerAccel * delta;
                steerVel += steerForce;

                // We clamp this so that the velocity gets redirected in the direction of the steer force instead of just adding it
                steerVel.Normalize();
                steerVel *= curSpeed;
                

                nextVel = Vector2.Lerp(steerVel, Vector2.Zero, 1f - MathF.Exp(-rollSteerAccel * delta));
            }
            break;
        }

        
        vel = nextVel;
    }


    public void Move()
    {
        pos += (vel * delta);
        sprite.pos = this.pos;
    }

    public void AddVelocity(Vector2 externalVelocity) { externalVel += externalVelocity; }

    public void SetVelocity(Vector2 input) { newVel = input;}


    void GetInput() 
    {
        inputVector = Input.move;
        if(inputVector != Vector2.Zero) inputVector.Normalize();
        //if(inputVector != Vector2.Zero) lastRequestDir = inputVector;
    }

    void HandleStates() 
    {
        Stance nextStance = Stance.Null;

        //if(Input.gameplay.Attack.WasPressedThisFrame()) attackQueued = true;
        if(state.stance == Stance.Attacking && attackTimer <= 0) nextStance = Stance.Walking;
        if(state.stance == Stance.Rolling && rollTimer <= 0) nextStance = Stance.Walking;
        
        if(GameManager.gameState == GameState.Playing)
        {
            //if(Input.gameplay.Roll.WasPressedThisFrame()) nextStance = Stance.Rolling;
            if(attackQueued && (state.stance == Stance.Walking)) nextStance = Stance.Attacking;
        }
      
        lastState.stance = state.stance;
        if(nextStance != Stance.Null && nextStance != state.stance) ChangeStance(nextStance);
    }
    
    public IEnumerator LerpToPos(Vector2 newPos, float dur)
    {
        translating = true;
        Vector2 oldPos = pos;
        
        for (float t = 0f; t < dur; t += delta)
        {
            pos = Vector2.Lerp(oldPos, newPos, t / dur);
            yield return 0;
        }
        pos = newPos;
        translating = false;
    }

    public CharacterState GetState() => state;
    public bool isMoving() => translating? true : !inputVector.Equals(Vector2.Zero);

    #endregion

    #region What Used To Be In Animator

    float footStepTimer = 0;

    Vector2 attackDir;
    
    // void OnStanceChanged(Stance _stance)
    // {
    //     stance = _stance;
    //     if(stance == Stance.Attacking)
    //     {
    //         attackDir = (Camera.main.ScreenToWorldPoint(Input.mouse.position.ReadValue()) - transform.position).normalized * 100f;
    //         attackSlash.SetActive(true);
    //         Vector2 lookDir =  (transform.position - Camera.main.ScreenToWorldPoint(Input.mouse.position.ReadValue())).normalized;
    //         float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
    //         attackSlash.transform.localEulerAngles = new(0, 0, angle);
    //     }
    //     if(stance == Stance.Rolling) animator.SetBool("Dodging", true);
    //     else animator.SetBool("Dodging", false);
    // }

    Vector2 lastDir;


    void FootStepSounds()
    {
        footStepTimer -= delta;
        

        if(vel.Length() > 1.5f) lastDir = vel;
        float veloMag = vel.Length() > .5f? vel.Length() : 0;

        if(state.stance == Stance.Attacking)
        {
            lastDir = attackDir;
            veloMag = 0;
        } 


        // animator.SetFloat("X", (int)lastDir.x);
        // animator.SetFloat("Y", (int)lastDir.y);
        // animator.SetFloat("Mag", (int)veloMag);

        if(veloMag < 30f)
        {
            footStepTimer = 0;
        }
        else
        {
            if(footStepTimer < 0f)
            {
                //-0.001x+0.45
                footStepTimer = .25f;
                Sound.Play("footstep", pos);
            }
        }
    }
    #endregion

}

public enum Stance 
{ 
    Walking,
    Attacking,
    Rolling,
    Immobile,
    Null
}

public struct CharacterState
{
    public Vector2 velocity;
    public Stance stance;
}