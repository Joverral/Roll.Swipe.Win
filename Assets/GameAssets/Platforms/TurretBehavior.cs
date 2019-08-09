using UnityEngine;
using System.Collections;

public class TurretBehavior : MonoBehaviour {

    [SerializeField]
    AudioClip audioClip;

    [SerializeField]
    GameObject ProjectilePrefab;

    [SerializeField]
    float ReloadTime = 1.0f;
    [SerializeField]
    float TurnSpeed = 5.0f;
    [SerializeField]
    float FirePauseTime = 0.25f;
    [SerializeField]
    GameObject MuzzleEffectPrefab;

    [SerializeField]
    Transform MuzzlePosition;

    [SerializeField]
    float ErrorAmount = .001f;

    [SerializeField]
    float ShellVelocity;

    Transform Target;
  
    float nextFireTime;
    float nextMoveTime;
    float aimError;

    static WaitForSeconds waitForShotPauseTime;
    static WaitForSeconds waitForReloadTime;

    bool pauseRotateCoroutine;

    IEnumerator RotateTowardsTargetCoroutine()
    {
        while (gameObject != null) //silly while forever
        {
            if (Target != null && !pauseRotateCoroutine)
            {
                Rigidbody2D targetRigidBody = Target.GetComponent<Rigidbody2D>();

                //Guess where the target will be by looking where the target will be when it's time to fire. (assuming same velocity)
                float timeGuess = nextFireTime - Time.time;
                Vector2 guessPos = Target.position;

                Vector2 guessVelocity = new Vector3(targetRigidBody.velocity.x, targetRigidBody.velocity.y);
                if (timeGuess > 0.0)
                {
                    guessPos += guessVelocity * timeGuess; // guess where the enemy will move to by the time it's ready to fire.
                }

                // Adjust the position by how long the shell will take to reach the guess spot
                guessPos += ((guessPos - (Vector2)MuzzlePosition.position).magnitude / ShellVelocity) * guessVelocity;

                // TODO: adjust by a random amount of error


                var dir = guessPos - new Vector2(transform.position.x, transform.position.y);
                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                var desiredRotation = Quaternion.Euler(0.0f, 0.0f, angle);
                //var desiredRotation = transform.rotation * Quaternion.Euler(0.0f, 0.0f, -angle);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, Time.deltaTime * TurnSpeed);

            }
            yield return null;
        }
    }

    IEnumerator FireCoroutine()
    {
        while (this.gameObject != null) // silly forever loop
        {
            nextFireTime = Time.time + ReloadTime;
            yield return waitForReloadTime;

            if(Target != null)
            { 
                // pause turn coroutine
                pauseRotateCoroutine = true;

                // Pause for a short bit to 'aim' or charge the shot
                yield return waitForShotPauseTime;

                // Fire!
                var projectile = GameObjectPooler.Current.GetObject(ProjectilePrefab);
                projectile.transform.position = MuzzlePosition.position;
                projectile.transform.rotation = MuzzlePosition.rotation;

                var projectileRB = projectile.GetComponent<Rigidbody2D>();

                projectileRB.AddRelativeForce(new Vector2(0, ShellVelocity), ForceMode2D.Impulse);

                pauseRotateCoroutine = false;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Target = other.gameObject.transform;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Target = null;
    }

    public void OnDisable()
    {
        Target = null;
    }

    public void OnEnable()
    {
        if(waitForShotPauseTime == null)
        {
            waitForShotPauseTime = new WaitForSeconds(FirePauseTime);
        }

        if(waitForReloadTime == null)
        {
            waitForReloadTime = new WaitForSeconds(ReloadTime);
        }

        StartCoroutine(FireCoroutine());
        StartCoroutine(RotateTowardsTargetCoroutine());
    }
}
