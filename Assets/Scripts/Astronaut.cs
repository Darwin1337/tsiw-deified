using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Astronaut : MonoBehaviour
{
    private Animator anim;
    private Vector3 initialPosition;
    private bool isOnTop = false;
    private bool isAnimating = false;
    private float metros;
    private bool contarMetros = true;
    public float segundos;
    public float velocidade = 1f;
    public int azuis = 0;
    public int vermelhos = 0;
    public int verdes = 0;
    public bool isPaused = false;
    private bool godMode = false;
    private bool hasStopped = false;

    private System.DateTime seconds = System.DateTime.Now;

    public AudioSource jumpSound;
    public AudioSource milestone1;
    public AudioSource footsteps;
    public AudioSource breathing;
    public AudioSource effect1;
    public AudioSource effect2;
    public AudioSource effect3;
    public AudioSource countdown;
    public AudioSource crescendo;
    public AudioSource endgame;

    public Volume volume;
    private Vignette vignette;
    private bool showOutline = false;

    [SerializeField] Text mostrador;
    [SerializeField] Text countBlues;
    [SerializeField] Text countReds;
    [SerializeField] Text countGreens;
    [SerializeField] GameObject gameController;

    // Vari�veis para mudar de cor suavemente
    private bool changeColor = false;
    private Color colorTarget = new Color32(46, 46, 46, 1);
    private Color colorInitiator = new Color32(46, 46, 46, 1);
    private float duration = 0;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = velocidade;
        anim = GetComponent<Animator>();
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if((System.DateTime.Now - seconds).TotalSeconds > 10)
        {
            if (!hasStopped)
            {
                stop();
            }
        }
        if (contarMetros)
        {
            ContadorMetros();
        }
        
        if (!gameController.GetComponent<GameController>().isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space)){
                jumpSound.Play();
            }
            if (Input.GetKeyDown(KeyCode.Space) || isAnimating)
            {
                CharacterJump();
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!isPaused)
            {
                Time.timeScale = 0f;
                isPaused = true;
            } else
            {
                Time.timeScale = velocidade;
                isPaused = false;
            }
        }

        if (isAnimating)
        {
            footsteps.Stop();
        }
        else if (!isAnimating && !footsteps.isPlaying && gameController.GetComponent<GameController>().isGameOver == false)
        {
            footsteps.Play();
        }

        if (!isPaused)
            Time.timeScale = velocidade;

        if (showOutline)
        {
            if (GetComponent<Outline>().OutlineWidth < 10)
                GetComponent<Outline>().OutlineWidth += 15f * Time.deltaTime;
        } 
        else
        {
            if (GetComponent<Outline>().OutlineWidth > 0)
                GetComponent<Outline>().OutlineWidth -= 15f * Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (changeColor)
        {
            if (volume.profile.TryGet<Vignette>(out vignette))
            {
                vignette.color.value = Color.Lerp(colorInitiator, colorTarget, duration);
                if (duration < 1)
                    duration += Time.deltaTime / 3f;
                else if (duration == 1)
                {
                    changeColor = false;
                    duration = 0;
                }
            }
        }
    }

    void ContadorMetros()
    {
        MostrarMetros(metros);
        
        if (isOnTop)
        {
            metros -= 1f * velocidade * Time.deltaTime;
        }
        else
        {
            metros += 1f * velocidade * Time.deltaTime;
        }
    }

    void CharacterJump()
    {
        if (isOnTop)
        {
            // Caso esteja em baixo executa este c�digo para saltar
            if (!isAnimating)
            {
                anim.SetTrigger("jump");
                isAnimating = true;
            }
            else
            {
                if (transform.position.y > initialPosition.y)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y - (3f * Time.deltaTime), transform.position.z);
                }

                if (transform.position.y < 2f)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, -90, 0), 5f * Time.deltaTime);

                if (transform.position.y <= initialPosition.y && Mathf.Abs(transform.rotation.eulerAngles.y - Quaternion.Euler(0, -90, 0).eulerAngles.y) <= 1)
                {
                    isAnimating = false;
                    isOnTop = false;
                }
            }
        }
        else
        {
            // Caso esteja em baixo executa este c�digo para saltar
            if (!isAnimating)
            {
                anim.SetTrigger("jump");
                isAnimating = true;
            }
            else
            {
                if (transform.position.y < 2.1f)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + (3f * Time.deltaTime), transform.position.z);
                    transform.LookAt(transform.position);
                }

                if (transform.position.y > initialPosition.y + .1f)
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(180, 90, 0), 5f * Time.deltaTime);

                if (transform.position.y >= 2.1f && Mathf.Abs(transform.rotation.eulerAngles.y - Quaternion.Euler(180, 90, 0).eulerAngles.y) <= 1)
                {
                    isAnimating = false;
                    isOnTop = true;
                }
            }
        }
	}

    void MostrarMetros(float relogio)
    {
        segundos = Mathf.FloorToInt(relogio);
		if(segundos % 100 == 0 && segundos != 0){
            if(!milestone1.isPlaying){
                milestone1.Play();
            }
        }
        mostrador.text = segundos.ToString()+" m";
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag=="blue")
        {
            if((System.DateTime.Now - seconds).TotalSeconds > 10)
            {
                countdown.Play();
                crescendo.Play();
                azuis += 1;
                countBlues.text = azuis.ToString();
                Destroy(other.gameObject);
                changeColor = true;
                godMode = true;
                effect1.Play();
                colorInitiator = colorTarget;
                colorTarget = new Color32(0, 255, 255, 1);
                duration = 0;
                showOutline = true;
                seconds = System.DateTime.Now;
                hasStopped = false;
            }
            
        }
        else if (other.gameObject.tag == "red")
        {
            if ((System.DateTime.Now - seconds).TotalSeconds > 10)
            {
               
                vermelhos += 1;
                countdown.Play();
                crescendo.Play();
                countReds.text = vermelhos.ToString();
                velocidade = 1.6f;
                breathing.pitch = 3.0f;
                footsteps.pitch = 3.0f;
                effect3.Play();
                Destroy(other.gameObject);
                changeColor = true;
                colorInitiator = colorTarget;
                colorTarget = new Color32(255, 123, 0, 1);
                duration = 0;
                seconds = System.DateTime.Now;
                hasStopped = false;
            }
            
        }
        else if (other.gameObject.tag == "green")
        {
            if ((System.DateTime.Now - seconds).TotalSeconds > 10)
            {
                //StopCoroutine(ExampleCoroutine1());
                verdes += 1;
                countdown.Play();
                crescendo.Play();
                countGreens.text = verdes.ToString();
                velocidade = 0.6f;
                //StartCoroutine(ExampleCoroutine1());
                footsteps.pitch = 1.0f;
                breathing.pitch = 1.0f;
                effect2.Play();
                Destroy(other.gameObject);
                changeColor = true;
                colorInitiator = colorTarget;
                colorTarget = new Color32(0, 255, 114, 1);
                duration = 0;
                seconds = System.DateTime.Now;
                hasStopped = false;
            }
        }
        else if (other.gameObject.tag == "trampolim")
        {
            isAnimating = false;
            CharacterJump();
        }
        else
        {
            if (!godMode)
            {
                contarMetros = false;
                gameController.GetComponent<GameController>().isGameOver = true;
                anim.SetTrigger("die");
				
				footsteps.Stop();
                breathing.Stop();
                if(!endgame.isPlaying){
                    endgame.Play();
                }
                
                if (PlayerPrefs.GetFloat("Score") < segundos)
                    PlayerPrefs.SetFloat("Score", segundos);

                StartCoroutine(ExampleCoroutine());
                GameOver.metros = segundos;
            }
        }
    }
    IEnumerator ExampleCoroutine()
    {
        yield return new WaitForSeconds(2);
        GameOver.gameover = true;
    }

    void stop()
    {
        hasStopped = true;
        if (godMode)
            godMode = false;
        footsteps.pitch = 2.05f;
        breathing.pitch = 2.0f;
        velocidade = 1f;
        showOutline = false;
        changeColor = true;
        colorInitiator = colorTarget;
        colorTarget = new Color32(46, 46, 46, 1);
        duration = 0;
    }
}
