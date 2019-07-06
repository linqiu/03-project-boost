using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float loadGameDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;


    Rigidbody rigidBody;
    AudioSource rocketAudio;
    Light rocketReflection;

    enum State { Alive, Dying, Transcending };
    State state = State.Alive;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rocketAudio = GetComponent<AudioSource>();
        rocketReflection = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrust();
            Rotate();
        }       
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }

    void LoadFirstScene()
    {
        SceneManager.LoadScene(0);
    }

    void ProcessSuccess()
    {
        state = State.Transcending;
        rocketAudio.Stop();
        rocketAudio.PlayOneShot(success);
        successParticles.Play();
        mainEngineParticles.Stop();
        Invoke("LoadNextScene", loadGameDelay);
    }

    void ProcessDeath()
    {
        state = State.Dying;
        rocketAudio.Stop();
        rocketAudio.PlayOneShot(death);
        mainEngineParticles.Stop();
        rocketReflection.intensity = 0;
        deathParticles.Play();
        Invoke("LoadFirstScene", loadGameDelay);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) { return; }   

        switch(collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                ProcessSuccess();
                break;
            default:
                ProcessDeath();
                break;
        }

    }

    void Rotate()
    {
        rigidBody.freezeRotation = true;      
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidBody.freezeRotation = false;
    }

    void RespondToThrust()
    {
        if (Input.GetKey(KeyCode.Space)) // can thrust while rotating
        {
            ApplyThrust();
        }
        else
        {
            rocketAudio.Stop();
            mainEngineParticles.Stop();
            rocketReflection.intensity = 0;
        }
    }

    void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust);
        if (!rocketAudio.isPlaying)
        {
            rocketAudio.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
        float newIntensity = 5 + Random.Range(-3f, 5f);
        rocketReflection.intensity = newIntensity;

     }
}
