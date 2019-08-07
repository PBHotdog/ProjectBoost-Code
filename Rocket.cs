using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip levelComplete;
    [SerializeField] ParticleSystem rocketThrust;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem finishParticles;


    AudioSource audioSource;
    Rigidbody rigidbody;

    enum State {Alive, Dying, Transcend}
    State state = State.Alive;

    bool collisionsEnabled = true;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        if(Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }
        else if( Input.GetKeyDown(KeyCode.C))
        {
            collisionsEnabled = !collisionsEnabled;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || !collisionsEnabled) { return; }

        switch(collision.gameObject.tag)
        {
            case "Friendly":
                //do nothing
                break;
            case "Finish":
                StartFinishSequence();
                break;
            default:
                StartDeathSequence();
                break;

        }
    }

    private void StartFinishSequence()
    {
        print("Hit finish");
        state = State.Transcend;
        audioSource.Stop();
        audioSource.PlayOneShot(levelComplete);
        finishParticles.Play();
        Invoke("LoadNextScene", levelLoadDelay); //parameterise time
    }

    private void StartDeathSequence()
    {
        print("Hit something deadly!");
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound);
        deathParticles.Play();
        rocketThrust.Stop();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
            SceneManager.LoadScene(nextSceneIndex); //TODO allow more then one level
            finishParticles.Stop();
    }
    void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
        deathParticles.Stop();
    }
    private void RespondToRotateInput()
    {
        rigidbody.freezeRotation = true; //take manual rotation
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward *rotationThisFrame);
        }
        rigidbody.freezeRotation = false; //resume physics
    }
    void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space)) //can thrust while rotating
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            rocketThrust.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidbody.AddRelativeForce(Vector3.up * mainThrust *Time.deltaTime);
        if (audioSource.isPlaying == false)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        rocketThrust.Play();
    }
}
