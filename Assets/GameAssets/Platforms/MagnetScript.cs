using UnityEngine;
using System.Collections;

public class MagnetScript : MonoBehaviour {

    [SerializeField]
    bool RandomStartRotation = true;

    [SerializeField]
    float minMagnetStrength = 7.0f;
    [SerializeField]
    float maxMagnetStrength = 15.0f;

    [SerializeField]
    Color ActiveColor = Color.magenta;

    [SerializeField]
    GameObject particlePrefab;

    [SerializeField]
    GameObject overHeatParticle; //not a prefab

    [SerializeField]
    float timeBetweenParticles = 0.3f;
    float lastParticleTimeStamp = 0.0f;

    [SerializeField]
    int numParticlePerTimePeriod = 3;

    [SerializeField]
    float onTime = 3.0f;
    [SerializeField]
    float cooldownTime = 2.0f;

    float timer = 0.0f; // for ontime

    AudioSource magnetAudioSource;

    SpriteRenderer spriteRenderer;
    float maxMag;

    Coroutine coolOffRoutine;

    void Start()
    {
        maxMag = this.GetComponent<BoxCollider2D>().size.y;
        magnetAudioSource = this.GetComponent<AudioSource>();
    }

    IEnumerator CoolOffCoroutine(float timeToCool)
    {
        spriteRenderer.color = Color.red;
        if(overHeatParticle != null)
        {
            overHeatParticle.SetActive(true);
            overHeatParticle.GetComponent<ParticleSystem>().Play();
        }

        for(int i = 0; i <  this.transform.childCount; ++i)
        {
            var childParticle = this.transform.GetChild(i).gameObject;
            if (childParticle == overHeatParticle)
                continue; // don't remove the overheat!

            childParticle.SetActive(false);
            GameObjectPooler.Current.PoolObject(childParticle);
        }

        yield return new WaitForSeconds(timeToCool);
        spriteRenderer.color = Color.white;
        coolOffRoutine = null;
        if (overHeatParticle != null)
        {
            overHeatParticle.SetActive(false);
        }
    }

    void PullCollider(Collider2D other)
    {
        if (coolOffRoutine == null &&
            other.gameObject.CompareTag("Player")) // why is this here?
        {
            if (!magnetAudioSource.isPlaying)
            {
                magnetAudioSource.Play();
            }

            var mag = (transform.position - other.transform.position).magnitude;
            var dirNorm = (transform.position - other.transform.position) / mag;

            var t = mag / maxMag;

            magnetAudioSource.volume = Mathf.Max(0.25f, 1.0f - t);

            // the strength of the magnet should be inversely proportional to the distance from the magnet
            var currStrength = Mathf.Lerp(maxMagnetStrength, minMagnetStrength, t);
            other.attachedRigidbody.AddForce(dirNorm * currStrength, ForceMode2D.Force);

            spriteRenderer.color = Color.Lerp(ActiveColor, Color.white, Mathf.Min(0.3f, t));

            if ((Time.time - lastParticleTimeStamp) > timeBetweenParticles)
            {
                var playerCollider = other.GetComponent<CircleCollider2D>();
                for (int i = 0; i < numParticlePerTimePeriod; ++i)
                {
                    var particle = GameObjectPooler.Current.GetObject(particlePrefab);
                    particle.transform.parent = this.transform;
                    particle.transform.position = Random.insideUnitSphere * playerCollider.radius * other.transform.localScale.x + other.transform.position;

                    var particleBody = particle.GetComponent<Rigidbody2D>();
                    particleBody.AddForce(dirNorm * currStrength, ForceMode2D.Impulse);
                }
                lastParticleTimeStamp = Time.time;
            }

            timer += Time.deltaTime;

            if(timer >= onTime)
            {
                coolOffRoutine = StartCoroutine(CoolOffCoroutine(cooldownTime));
                timer = 0.0f;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    { 
        PullCollider(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        PullCollider(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            spriteRenderer.color = Color.white;
            magnetAudioSource.Stop();
        }
    }

    void OnEnable()
    {
        if (coolOffRoutine != null)
        {
            StopCoroutine(coolOffRoutine);
        }
        coolOffRoutine = null;

        if (RandomStartRotation)
        { 
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f));
        }
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.white;

        timer = 0.0f;
    }
}
