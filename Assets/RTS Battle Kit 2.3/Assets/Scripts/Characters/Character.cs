using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.AI;

public class Character : MonoBehaviour {

    //variables visible in the inspector
    public float lives;
    public float damage;
    public float minAttackDistance;
    public float castleStoppingDistance;
    public string attackTag;
    public string attackCastleTag;
    public int addGold;
    public ParticleSystem dieParticles;
    public GameObject ragdoll;
    public AudioClip attackAudio;
    public AudioClip runAudio;

    //variables not visible in the inspector
    [HideInInspector]
    public Transform currentTarget;
    [HideInInspector]

    [Space(10)]

    private GameObject[] enemies;
    private GameObject health;
    private GameObject healthbar;

    [HideInInspector]
    private float startLives;
    private GameObject castle;
    private Animator[] animators;
    private Vector3 targetPosition;
    private AudioSource source;

    private Vector3 randomTarget;

    private ParticleSystem dustEffect;

    [HideInInspector]
    public Vector3 castleAttackPosition;


    void Start() {
        source = GetComponent<AudioSource>();

        //find navmesh agent component
        animators = gameObject.GetComponentsInChildren<Animator>();

        //find objects attached to this character
        health = transform.Find("Health").gameObject;
        healthbar = health.transform.Find("Healthbar").gameObject;
        health.SetActive(false);

        //set healtbar value
        healthbar.GetComponent<Slider>().maxValue = lives;
        startLives = lives;

        //find the castle closest to this character
        findClosestCastle();
    }

    void Update() {
        //find closest castle
        if (castle == null) {
            findClosestCastle();
        }

        if (lives != startLives) {
            //only use the healthbar when the character lost some lives
            if (!health.activeSelf)
                health.SetActive(true);

            health.transform.LookAt(2 * transform.position - Camera.main.transform.position);
            healthbar.GetComponent<Slider>().value = lives;
        }

        //find closest enemy
        if (currentTarget == null) {
            findCurrentTarget();
        }

        //if character ran out of lives add blood particles, add gold and destroy character
        if (lives < 1) {
            StartCoroutine(die());
        }

        if (currentTarget != null) {
            // look at target
            Vector3 currentTargetPosition = currentTarget.position;
            currentTargetPosition.y = transform.position.y;
            transform.LookAt(currentTargetPosition);

            // do damage
            currentTarget.gameObject.GetComponent<Character>().lives -= Time.deltaTime * damage;

            // animation and sound
            foreach (Animator animator in animators) {
                animator.SetBool("Attacking", true);
            }
            if (source.clip != attackAudio) {
                source.clip = attackAudio;
                source.Play();
            }
        } else if (castle != null) {
            // look at castle
            transform.LookAt(castleAttackPosition);

            // do damage
            castle.GetComponent<Castle>().lives -= Time.deltaTime * damage;

            // animation and sound
            foreach (Animator animator in animators) {
                animator.SetBool("Attacking", true);
            }
            if (source.clip != attackAudio) {
                source.clip = attackAudio;
                source.Play();
            }
        } else {
            // stop attacking
            foreach (Animator animator in animators) {
                animator.SetBool("Attacking", false);
            }
            if (source.clip != attackAudio) {
                source.clip = attackAudio;
                source.Stop();
            }
        }
    }

    public void findClosestCastle() {
        //find the castles that should be attacked by this character
        GameObject[] castles = GameObject.FindGameObjectsWithTag(attackCastleTag);

        //distance between character and its nearest castle
        float closestCastle = Mathf.Infinity;

        foreach (GameObject potentialCastle in castles) {
            //check if there are castles left to attack and check per castle if its closest to this character
            if (Vector3.Distance(transform.position, potentialCastle.transform.position) < closestCastle && potentialCastle != null) {
                //if this castle is closest to character, set closest distance to distance between character and this castle
                closestCastle = Vector3.Distance(transform.position, potentialCastle.transform.position);
                //also set current target to closest target (this castle)
                //bool sameParent = castle.transform.parent.Equals(gameObject.transform.parent);
                //if (sameParent) {

                //}
                //castle = potentialCastle;
                // only attack if on same island
                if (gameObject.transform.parent.gameObject == potentialCastle.transform.parent.gameObject) {
                    castle = potentialCastle;
                }

            }
        }

        if (castle != null)
            castleAttackPosition = castle.transform.position;
    }

    public void findCurrentTarget() {
        //find all potential targets (enemies of this character)
        enemies = GameObject.FindGameObjectsWithTag(attackTag);

        //distance between character and its nearest enemy
        float closestDistance = Mathf.Infinity;

        foreach (GameObject potentialTarget in enemies) {
            //check if there are enemies left to attack and check per enemy if its closest to this character
            if (Vector3.Distance(transform.position, potentialTarget.transform.position) < closestDistance && potentialTarget != null) {
                //if this enemy is closest to character, set closest distance to distance between character and enemy
                closestDistance = Vector3.Distance(transform.position, potentialTarget.transform.position);
                //also set current target to closest target (this enemy)
                if (!currentTarget || (currentTarget && Vector3.Distance(transform.position, currentTarget.position) > 2)) {
                    // only attack if on same island
                    if (gameObject.transform.parent.gameObject == potentialTarget.transform.parent.gameObject) {
                        currentTarget = potentialTarget.transform;
                    }
                }
            }
        }
    }

    public IEnumerator die() {
        CombatController.gold += addGold;

        if (gameObject.tag == "Enemy") {
            Manager.enemiesKilled++;
            CombatController.enemyCount -= 1;
        }

        if (gameObject.tag == "Knight") {
            CombatController.soldierCount -= 1;
        }


        foreach (Character character in GameObject.FindObjectsOfType<Character>()) {
            if (character != this)
                character.findCurrentTarget();
        }

        yield return new WaitForEndOfFrame();
        Destroy(gameObject);
    }
}
