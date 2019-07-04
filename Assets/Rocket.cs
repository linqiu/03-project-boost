using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;
    Rigidbody rigidBody;
    AudioSource rocketAudio;

    enum State { Alive, Dying, Transcending };
    State state = State.Alive;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rocketAudio = GetComponent<AudioSource>();
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
        Invoke("LoadNextScene", 1f);
    }

    void ProcessDeath()
    {
        state = State.Dying;
        rocketAudio.Stop();
        rocketAudio.PlayOneShot(death);
        Invoke("LoadFirstScene", 2f);
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
        }
    }

    void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust);
        if (!rocketAudio.isPlaying)
        {
            rocketAudio.PlayOneShot(mainEngine);
        }
    }
}
