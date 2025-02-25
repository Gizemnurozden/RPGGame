using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellBook : MonoBehaviour
{
    private static SpellBook instance;

    public static SpellBook MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SpellBook>();
            }

            return instance;
        }
    }




    [SerializeField]
    private Image castingBar;

    [SerializeField]
    private Text currentSpell;

    [SerializeField]
    private Image icon;

    [SerializeField]
    private Text castTime;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private Spell[] spells;

    private Coroutine spellRoutine;

    private Coroutine fadeRoutine;

    [SerializeField]
    private GameObject[] obtainableSpells;


    public void Cast (ICastable castable)
    {
        

        castingBar.fillAmount = 0; //doluluk oranı başta 0 

        castingBar.color = castable.MyBarColor; //spell casting rengi

        currentSpell.text = castable.MyTitle; //spell casting ismi

        icon.sprite = castable.MyIcon; //iconu

        spellRoutine = StartCoroutine(Progress(castable));

        fadeRoutine = StartCoroutine(FadeBar());
        
    }

    private IEnumerator Progress(ICastable castable)
    {
        float timePassed = Time.deltaTime;

        float rate = 1.0f / castable.MyCastTime;

        float progress = 0.0f;

        while (progress <= 1.0)
        {
            castingBar.fillAmount = Mathf.Lerp(0, 1, progress); //zaman geçtikçe doluluk

            progress += rate * Time.deltaTime; //arttır zamanla doluluğu

            timePassed += Time.deltaTime;

            castTime.text = (castable.MyCastTime - timePassed).ToString("F2");

            if (castable.MyCastTime - timePassed < 0)
            {
                castTime.text = "0.00";
            }

            yield return null;
        }
        StopCasting();
    }

    private IEnumerator FadeBar()
    {
      

        float rate = 1.0f / 0.50f;

        float progress = 0.0f;

        while (progress <= 1.0)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, progress);

            progress += rate * Time.deltaTime;

            yield return null;
        }
    }

    public void StopCasting()
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine); //fadei görünmez yapar.
            canvasGroup.alpha = 0;
            fadeRoutine = null;
        }


        if (spellRoutine != null)
        {
            StopCoroutine(spellRoutine);
            spellRoutine = null;
        }
    }
    public void LearnSpell(string name)
    {
        switch (name.ToLower())
        {
            case "rain of fire":
                obtainableSpells[0].SetActive(true);
                break;

            case "blizzard1":
                obtainableSpells[1].SetActive(true);
                break;

            case "chainlightning":
                obtainableSpells[2].SetActive(true);
                break;
        }
    }

    public Spell GetSpell(string spellName)
    {
        Spell spell = Array.Find(spells, x => x.MyTitle == spellName);

        return spell;
    }
}
