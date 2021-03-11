﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Entertainor : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private LifeManager lifeManager;
    
    [SerializeField] private float alertMinTimer = 15f;
    [SerializeField] private float alertMaxTimer = 25f;
    [SerializeField] private float moveSpeedMax = 8f;
    [SerializeField] private float distRemote = 5f;
    [SerializeField] private float firstCircle = 8f;
    [SerializeField] private GameObject canvasPivot;

    [SerializeField] private Image currentPic;
    [SerializeField] private Sprite okPic;
    [SerializeField] private Sprite coldPic;
    [SerializeField] private Sprite oxygenPic;
    [SerializeField] private Sprite interrogPic;
    [SerializeField] private AudioSource entertainorAudioSource;
    [SerializeField] private AudioClip heySound;
    [SerializeField] private AudioClip alrightSound;
    
    public static bool lookAtMe = false;
    public float moveSpeed = 0f;
    private float distanceFromObjective = 0f;
    private float alertTimer;
    private bool alerting = false;
    private enum Question {
        Ok,
        Cold,
        Oxygen,
        Interrog 
    }
    private Question question;
    [HideInInspector] public int okGood = 0;
    [HideInInspector] public int okMistakes = 0;
    [HideInInspector] public int notOkGood = 0;
    [HideInInspector] public int notOkMistakes = 0;
    [HideInInspector] public int coldGood = 0;
    [HideInInspector] public int coldMistakes = 0;
    [HideInInspector] public int midPressureGood = 0;
    [HideInInspector] public int midPressureMistakes = 0;
    [HideInInspector] public int reserveGood = 0;
    [HideInInspector] public int reserveMistakes = 0;
    [HideInInspector] public int noAirGood = 0;
    [HideInInspector] public int noAirMistakes = 0;

    public void Start()
    {
        alertTimer = Random.Range(alertMinTimer,alertMaxTimer);
    }

    public void Update()
    {
        alertTimer -= Time.deltaTime;
        if (alertTimer <= 0 && !alerting) {
            alerting = true;
            int choice = Random.Range(0,2);
            switch (choice)
            {
                case 0:
                    currentPic.sprite = okPic;
                    question = Question.Ok;
                    break;
                case 1:
                    currentPic.sprite = coldPic;
                    question = Question.Cold;
                    break;
                case 2:
                    currentPic.sprite = oxygenPic;
                    question = Question.Oxygen;
                    break;
                default:
                    break;
            }
            canvasPivot.SetActive(true);
            entertainorAudioSource.PlayOneShot(heySound,0.5f);
        }

    }

    public void FixedUpdate()
    {
        Vector3 objective = player.position + (transform.position - player.position).normalized * firstCircle;

        distanceFromObjective = (transform.position - objective).magnitude;

        if (distanceFromObjective <= distRemote) {
            moveSpeed = moveSpeedMax / distRemote * distanceFromObjective;
        }
        else moveSpeed = moveSpeedMax;

        if ( (transform.position - player.position).magnitude >= firstCircle ) {
            transform.position = Vector3.MoveTowards(transform.position, objective, moveSpeed * Time.deltaTime);
        }
        
        transform.LookAt(new Vector3(player.position.x, player.position.y + 3, player.position.z));
        canvasPivot.transform.LookAt(player.position);
    }

    public void Ask()
    {
        alerting = true;
        currentPic.sprite = interrogPic;
        question = Question.Interrog;
        canvasPivot.SetActive(true);
    }

    public void AnswerOK()
    {
        if (lookAtMe) {
            if (alerting) {
                canvasPivot.SetActive(false);
                entertainorAudioSource.PlayOneShot(alrightSound,0.8f);
                alerting = false;
                alertTimer = Random.Range(alertMinTimer,alertMaxTimer);
                if (question == Question.Ok || question == Question.Interrog) {
                    if (lifeManager.oxygenLevel <= 50 && !lifeManager.oxygenMid) okMistakes += 1;
                    else if (lifeManager.oxygenLevel <= 25 && !lifeManager.oxygenLow) okMistakes += 1;
                    else if (lifeManager.oxygenLevel <= 5 && !lifeManager.oxygenVeryLow) okMistakes += 1;
                    else if (lifeManager.coldLevel <= 10 && !lifeManager.isVeryCold) okMistakes += 1;
                    else okGood +=1;
                }
                if (question == Question.Cold) {
                    if (lifeManager.coldLevel <= 10 && !lifeManager.isVeryCold) okMistakes += 1;
                    else okGood +=1;
                }
                if (question == Question.Oxygen) {
                    if (lifeManager.oxygenLevel <= 50 && !lifeManager.oxygenMid) okMistakes += 1;
                    else if (lifeManager.oxygenLevel <= 25 && !lifeManager.oxygenLow) okMistakes += 1;
                    else if (lifeManager.oxygenLevel <= 5 && !lifeManager.oxygenVeryLow) okMistakes += 1;
                    else okGood +=1;
                }
            }
        }
    }

    public void AnswerCold()
    {
        if (lookAtMe) {
            if (alerting) {
                canvasPivot.SetActive(false);
                entertainorAudioSource.PlayOneShot(alrightSound,0.8f);
                alerting = false;
                alertTimer = Random.Range(alertMinTimer,alertMaxTimer);
                if (question != Question.Oxygen && lifeManager.coldLevel >= 10) coldMistakes +=1;
                else if (question == Question.Oxygen) coldMistakes +=1;
                else coldGood +=1;
            }
            else {
                if (lifeManager.coldLevel >= 10) coldMistakes +=1;
                else coldGood +=1;
            }
        }
    }

    public void AnswerMidPressure()
    {
        if (lookAtMe) {
            if (alerting) {
                canvasPivot.SetActive(false);
                entertainorAudioSource.PlayOneShot(alrightSound,0.8f);
                alerting = false;
                alertTimer = Random.Range(alertMinTimer,alertMaxTimer);
                if (question == Question.Cold) midPressureMistakes +=1;
                else {
                    if (lifeManager.oxygenLevel >= 50 || lifeManager.oxygenLevel <= 25) midPressureMistakes +=1;
                    else midPressureGood +=1;
                }
            }
            else {
                if (lifeManager.oxygenLevel >= 50 || lifeManager.oxygenLevel <= 25) midPressureMistakes +=1;
                else midPressureGood +=1;
            }
        }
    }

    public void AnswerReserve()
    {
        if (lookAtMe) {
            if (alerting) {
                canvasPivot.SetActive(false);
                entertainorAudioSource.PlayOneShot(alrightSound,0.8f);
                alerting = false;
                alertTimer = Random.Range(alertMinTimer,alertMaxTimer);
                if (question == Question.Cold) reserveMistakes +=1;
                else {
                    if (lifeManager.oxygenLevel >= 25 || lifeManager.oxygenLevel <= 5) reserveMistakes +=1;
                    else reserveGood +=1;
                }
            }
            else {
                if (lifeManager.oxygenLevel >= 25 || lifeManager.oxygenLevel <= 5) reserveMistakes +=1;
                else reserveGood +=1;
            }
        }
    }

    public void AnswerNoAir()
    {
        if (lookAtMe) {
            if (alerting) {
                canvasPivot.SetActive(false);
                entertainorAudioSource.PlayOneShot(alrightSound,0.8f);
                alerting = false;
                alertTimer = Random.Range(alertMinTimer,alertMaxTimer);
                if (question == Question.Cold) noAirMistakes +=1;
                else {
                    if (lifeManager.oxygenLevel >= 5) noAirMistakes +=1;
                    else noAirGood +=1;
                }
            }
            else {
                if (lifeManager.oxygenLevel >= 5) noAirMistakes +=1;
                else noAirGood +=1;
            }
        }
    }
    public void AnswerNotOK()
    {
        if (lookAtMe) {
            if (alerting) {
                canvasPivot.SetActive(false);
                //entertainorAudioSource.PlayOneShot(alrightSound,0.8f);
                alerting = false;
                //alertTimer = Random.Range(alertMinTimer,alertMaxTimer);
            }
            if (lifeManager.oxygenLevel > 50 && lifeManager.coldLevel > 10) notOkMistakes +=1;
            else notOkGood +=1;
            Ask();
        }
    }
}