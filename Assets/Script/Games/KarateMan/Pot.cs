using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.KarateMan
{
    public class Pot : PlayerActionObject
    {
        public float startBeat;
        public float createBeat;
        [HideInInspector] public Animator anim;

        public GameObject Holder;
        private GameObject newHolder;
        public GameObject Sprite;
        public GameObject Shadow;

        public bool isThrown;
        public bool isHit = false;

        public float hitBeat;

        private Vector3 lastPos;
        private float lastShadowX;

        public AnimationCurve hitCurve;
        public AnimationCurve hitCurveY;
        public AnimationCurve hitCurveX;
        public AnimationCurve missCurve;
        public AnimationCurve shadowCurve;
        public AnimationCurve shadowCurveScale;

        public int type;
        public string hitSnd;

        private float hitLength;

        private float lastRot;

        public bool kick;

        public float lastPotRot;

        public string throwAnim;
        public bool combo;
        public int comboIndex;

        public Vector2 endShadowThrowPos;

        private int missTimes = 0;

        private void Start()
        {
            anim = GetComponent<Animator>();

            Sprite.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));

            if (type == 2)
                hitLength = 14f;
            else
                hitLength = 14f;

            /*if (combo)
            {
                if (comboIndex == 0)
                {
                    isEligible = true;
                    // PlayerActionInit(this.gameObject, createBeat, KarateMan.instance.EligibleCombos);
                }
                else if (comboIndex == 5)
                {
                    isEligible = true;
                }
            }
            else
            {
                isEligible = true;
                // PlayerActionInit(this.gameObject, createBeat, KarateMan.instance.EligibleHits);
            }*/

            PlayerActionInit(this.gameObject, createBeat);

            Sprite.GetComponent<SpriteRenderer>().enabled = false;
        }

        public override void OnAce()
        {
            if (combo)
            {
                if (comboIndex == 0)
                {
                    KarateJoe.instance.Combo(this);
                }
                else if (comboIndex == 5)
                {
                    KarateJoe.instance.ComboPow(this, true);
                }
            }
            else
            {
                this.Hit();
            }
            // KarateJoe.instance.Swing(state);
        }

        private void Update()
        {
            if (Conductor.instance.songPositionInBeats >= createBeat)
                Sprite.GetComponent<SpriteRenderer>().enabled = true;
            else
                Sprite.GetComponent<SpriteRenderer>().enabled = false;


            float time2Destroy = Conductor.instance.GetLoopPositionFromBeat(createBeat, 4);

            if (time2Destroy >= 1)
                Destroy(this.gameObject);

            if (isThrown)
            {
                float animTime = 2.22000000002f;
                float beatTime = 1f;
                if (comboIndex == 5)
                {
                    animTime = 2.27777777777f;
                }

                float normalizedBeatAnim = Conductor.instance.GetLoopPositionFromBeat(startBeat, animTime);
                anim.Play(throwAnim, 0, normalizedBeatAnim);
                anim.speed = 0;

                float normalizedBeat = Conductor.instance.GetLoopPositionFromBeat(startBeat, beatTime);

                Shadow.transform.localScale = Vector3.Lerp(new Vector3(4.12f, 4.12f), new Vector3(0.34f, 0.34f), shadowCurveScale.Evaluate(normalizedBeatAnim));
                Shadow.transform.localPosition = new Vector3(Mathf.Lerp(7.63f, endShadowThrowPos.x, shadowCurve.Evaluate(normalizedBeatAnim)), Mathf.Lerp(-12.26f, endShadowThrowPos.y, shadowCurve.Evaluate(normalizedBeatAnim)));

                lastPos = Holder.transform.localPosition;
                lastPotRot = Holder.transform.eulerAngles.z;
                lastShadowX = Shadow.transform.localPosition.x;
                lastRot = Holder.transform.GetChild(0).eulerAngles.z;

                if (combo && comboIndex == 0 || !combo)
                {
                    if (!KarateJoe.instance.hitCombo)
                    {
                        if (normalizedBeat >= 2 && missTimes == 0)
                        {
                            if (KarateJoe.instance.missC != null) StopCoroutine(KarateJoe.instance.missC);
                            KarateJoe.instance.missC = KarateJoe.instance.StartCoroutine(KarateJoe.instance.Miss());
                            missTimes = 1;
                        }
                    }
                }

                StateCheck(normalizedBeat);

                if (!combo)
                {
                    if (PlayerInput.Pressed())
                    {
                        if (state.perfect)
                        {
                            Hit();
                        }
                        else if (state.notPerfect())
                        {
                            Miss();
                        }
                    }
                }
                else
                {
                    if (comboIndex == 0)
                    {
                        if (PlayerInput.AltPressed())
                        {
                            if (state.perfect)
                            {
                                KarateJoe.instance.Combo(this);
                            }
                        }
                    }
                    else if (comboIndex == 5)
                    {
                        if (KarateJoe.instance.comboNormalizedBeat >= 2.05f)
                        if (PlayerInput.AltPressedUp())
                        {
                            KarateJoe.instance.ComboPow(this);
                        }
                    }
                }

                if (normalizedBeat > 1)
                {
                    Sprite.GetComponent<SpriteRenderer>().sortingOrder = -20;
                    Shadow.GetComponent<SpriteRenderer>().sortingOrder = -30;
                }
            }

            if (!isHit && !isThrown)
            {
                float normalizedBeatAnim = Conductor.instance.GetLoopPositionFromBeat(hitBeat, 1.5f);
                newHolder.transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(0, 0.55f, Conductor.instance.GetLoopPositionFromBeat(hitBeat, 0.45f)));
                Holder.transform.localPosition = new Vector3(Mathf.Lerp(lastPos.x, 0.9f, normalizedBeatAnim), Mathf.Lerp(lastPos.y, -3.43f, missCurve.Evaluate(normalizedBeatAnim)));
                Holder.transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(lastRot, lastRot - 523.203f, normalizedBeatAnim));
                Shadow.transform.localPosition = new Vector3(Mathf.Lerp(lastShadowX, 0.9f, normalizedBeatAnim), Shadow.transform.localPosition.y);
            }

            if (kick == false)
            {
                if (isHit)
                {
                    float normalizedBeatAnim = Conductor.instance.GetLoopPositionFromBeat(hitBeat, 1.5f);
                    var y = Mathf.Lerp(lastPos.y, -3.27f, hitCurve.Evaluate(normalizedBeatAnim));
                    var x = Mathf.Lerp(lastPos.x, hitLength, hitCurveX.Evaluate(normalizedBeatAnim));
                    newHolder.transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(0, 0.45f, hitCurveY.Evaluate(normalizedBeatAnim)));
                    Holder.transform.localPosition = new Vector3(x, y);
                    Shadow.transform.localPosition = new Vector3(Mathf.Lerp(lastShadowX, hitLength, hitCurveX.Evaluate(normalizedBeatAnim)), Shadow.transform.localPosition.y);
                    Holder.transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(lastPotRot, lastPotRot - 360, normalizedBeatAnim));
                    // anim.Play("PotHit", 0, normalizedBeatAnim);
                    // anim.speed = 0;
                }
            }
            else
            {
                if (isHit)
                {
                    float normalizedBeatAnim = Conductor.instance.GetLoopPositionFromBeat(hitBeat, 1.5f);
                    newHolder.transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(0, 0.55f, Conductor.instance.GetLoopPositionFromBeat(hitBeat, 0.45f)));
                    Holder.transform.localPosition = new Vector3(Mathf.Lerp(lastPos.x, 0.9f, normalizedBeatAnim), Mathf.Lerp(lastPos.y, -3.43f, missCurve.Evaluate(normalizedBeatAnim)));
                    Holder.transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(lastRot, lastRot - 523.203f, normalizedBeatAnim));
                    Shadow.transform.localPosition = new Vector3(Mathf.Lerp(lastShadowX, 0.9f, normalizedBeatAnim), Shadow.transform.localPosition.y);
                }
            }
        }

        public void Hit()
        {
            Jukebox.PlayOneShotGame(hitSnd);
            KarateJoe.instance.Swing(this);

            NewHolder();

            switch (type)
            {
                case 0:
                    // HitParticle.Play();
                    break;
                case 1:
                    GameObject bulbHit = Instantiate(KarateJoe.instance.BulbHit);
                    bulbHit.transform.parent = KarateJoe.instance.BulbHit.transform.parent;
                    bulbHit.SetActive(true);
                    Destroy(bulbHit, 0.7f);
                    break;
                case 2:
                    // RockParticle.Play();
                    break;
                case 4:
                    BarrelDestroy(false);
                    break;
            }

            if (!kick)
            {

            }
            else if (kick)
            {
                KarateMan.instance.CreateBomb(this.transform.parent, Holder.transform.localScale, ref Shadow);

                Destroy(this.gameObject);
            }

            hitBeat = Conductor.instance.songPositionInBeats;

            anim.enabled = false;
            isThrown = false;
            isHit = true;

            Sprite.GetComponent<SpriteRenderer>().sortingOrder = 49;
        }

        public void Miss()
        {
            Jukebox.PlayOneShot("miss");
            
            KarateJoe.instance.SetHead(3);

            NewHolder();
            Holder.transform.parent = newHolder.transform;

            hitBeat = Conductor.instance.songPositionInBeats;
            isHit = false;
            isThrown = false;
            anim.enabled = false;
            Sprite.GetComponent<SpriteRenderer>().sortingOrder = 49;
        }

        private void NewHolder()
        {
            newHolder = new GameObject();
            newHolder.transform.parent = this.gameObject.transform;
            Holder.transform.parent = newHolder.transform;
        }

        public void BarrelDestroy(bool combo)
        {
            for (int i = 0; i < 8; i++)
            {
                GameObject be = new GameObject();
                be.transform.localPosition = Holder.transform.localPosition;
                be.transform.parent = this.transform.parent;
                be.transform.localScale = Holder.transform.localScale;
                BarrelDestroyEffect bde = be.AddComponent<BarrelDestroyEffect>();
                Vector3 pos = be.transform.localPosition;
                SpriteRenderer sprite = be.AddComponent<SpriteRenderer>();

                bde.shadow = Instantiate(Shadow, transform.parent);
                bde.shadow.transform.position = Shadow.transform.position;
                bde.shadow.transform.localScale = Shadow.transform.lossyScale;
                bde.index = i;
                bde.combo = combo;

                switch (i)
                {
                    case 0:
                        be.transform.localPosition = new Vector3(pos.x, pos.y + 1.25f);
                        sprite.sortingOrder = 35;
                        bde.spriteIndex = 3;
                        break;
                    case 1:
                        be.transform.localPosition = new Vector3(pos.x, pos.y + -0.55f);
                        sprite.sortingOrder = 31;
                        bde.spriteIndex = 3;
                        break;
                    case 2:
                        be.transform.localPosition = new Vector3(pos.x - 0.8f, pos.y + 0.45f);
                        sprite.sortingOrder = 32;
                        bde.spriteIndex = 0;
                        break;
                    case 3:
                        be.transform.localPosition = new Vector3(pos.x - 0.5f, pos.y + 0.45f);
                        sprite.sortingOrder = 33;
                        bde.spriteIndex = 1;
                        break;
                    case 4:
                        be.transform.localPosition = new Vector3(pos.x, pos.y + 0.45f);
                        sprite.sortingOrder = 34;
                        bde.spriteIndex = 2;
                        break;
                    case 5:
                        be.transform.localPosition = new Vector3(pos.x + 0.5f, pos.y + 0.45f);
                        sprite.sortingOrder = 33;
                        sprite.flipX = true;
                        bde.spriteIndex = 1;
                        break;
                    case 6:
                        be.transform.localPosition = new Vector3(pos.x + 0.8f, pos.y + 0.45f);
                        sprite.sortingOrder = 32;
                        sprite.flipX = true;
                        bde.spriteIndex = 0;
                        break;
                    case 7:
                        be.transform.localPosition = new Vector3(pos.x, pos.y + 1.25f);
                        sprite.sortingOrder = 39;
                        bde.spriteIndex = 4;
                        break;
                }
            }
        }
    }
}