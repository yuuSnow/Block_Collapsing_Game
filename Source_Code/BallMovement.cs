using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallMovement : MonoBehaviour
{
    public Rigidbody2D ball_rb;
    public GameObject new_ball;
    public SpriteRenderer ball_sr;
    Color color_original;
    public GameObject ITEM_beam;
    public int ProbabilityOfITEM_beam = 20;

    public float BallSpeed = 300f;
    public float StanbyTime = 1f;
    public float PlusDoubleBallSpeed = 1.07f;
    int BallDegree;
    float x, y, first_v, now_v;
    float h, s, v;
    bool BallisMoving = false;
    bool BallVelocityChange = false;
    bool FirstTouchToPlayer = false;
    public GameManager gameManager;
    public SoundManager soundManager;
    
    // Start is called before the first frame update
    void Start()
    {
        DecisionDegree();
        color_original = ball_sr.color;
        Color.RGBToHSV(color_original, out h, out s, out v);

        if ( name == "Ball" )
        {
            GameInfo.OriginalBallisMoving = false;
        }
        
        gameManager.SetITEM_beamQuantity();
    }

    // Update is called once per frame
    void Update()
    {
        if ( !BallisMoving )
        {
            if ( name == "Ball(Clone)" )
            {
                BallisMoving = true;
                Invoke("MoveBall", StanbyTime);
            }
            else if ( ( Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) ) )
            {
                BallisMoving = true;
                GameInfo.OriginalBallisMoving = true;
                gameManager.StartGame();
                MoveBall();
            }
        }
    }

    void OnCollisionEnter2D( Collision2D collision )
    {
        int randam;
        float BallSize, new_ball_X, new_ball_Y, ITEM_beamSize;

        if ( collision.collider.name == "Player" && !FirstTouchToPlayer)
        {
            first_v = Mathf.Pow(Mathf.Pow(ball_rb.linearVelocity.x, 2f) + Mathf.Pow(ball_rb.linearVelocity.y, 2f), 0.5f);
            FirstTouchToPlayer = true;
        }

        if ( collision.collider.tag == "Target" )
        {
            soundManager.Produce_CollisionTargetSound();
            
            Destroy(collision.gameObject);
            BallVelocityChange = gameManager.AddScore();
            now_v = Mathf.Pow(Mathf.Pow(ball_rb.linearVelocity.x, 2f) + Mathf.Pow(ball_rb.linearVelocity.y, 2f), 0.5f);

            randam = Random.Range(0, 101);
            if ( randam <= ProbabilityOfITEM_beam && collision.collider.name != "Target_AddBall(Clone)")
            {
                ITEM_beamSize = collision.gameObject.transform.localScale.y * 0.25f;
                ITEM_beam.transform.localScale = new Vector3(ITEM_beamSize, ITEM_beamSize, 0);
                Instantiate(ITEM_beam, collision.gameObject.transform.position, Quaternion.identity);
            }

            if ( BallVelocityChange )
            {
                PlusBallSpeed();
            }

            switch( collision.collider.name )
            {
                case "Target_changeVelocity(Clone)" :

                    PlusBallSpeed();
                    break;
                
                case "Target_changeDirection(Clone)" :

                    DecisionDegree();
                    ball_rb.linearVelocity = now_v * new Vector2(x, y);
                    break;

                case "Target_changeSize(Clone)" :

                    BallSize = Random.Range(0.25f, 0.8f);
                    transform.localScale = new Vector3(BallSize, BallSize, 0);
                    break;
                
                case "Target_AddBall(Clone)" :

                    MissionInfo.PurpleGet = true;
                    new_ball_X = collision.gameObject.transform.position.x;
                    new_ball_Y = collision.gameObject.transform.position.y;
                    GameObject newBall = Instantiate(new_ball, new Vector3(new_ball_X, new_ball_Y, 0), Quaternion.identity);
                    
                    BallMovement newBallMovement = newBall.GetComponent<BallMovement>();
                    newBallMovement.gameManager = this.gameManager;
                    newBallMovement.soundManager = this.soundManager;
                    newBallMovement.new_ball = this.new_ball;
                    newBallMovement.ITEM_beam = this.ITEM_beam;

                    h = gameManager.AddHueValue();
                    
                    SpriteRenderer newSpriteRenderer = newBall.GetComponent<SpriteRenderer>();
                    newBallMovement.ball_sr = this.ball_sr;
                    newSpriteRenderer.color = Color.HSVToRGB(h, s, v);

                    break;
            }

            StartCoroutine(gameManager.LevelChangeJudgement());
        }

        if ( collision.collider.tag == "Wall" || collision.collider.name == "Player" )
        {
            soundManager.Produce_CollisionWallSound();
        }

        if ( collision.collider.name == "Bottom_Wall" )
        {
            ball_rb.linearVelocity = Vector2.zero;
            GameInfo.RestartIndex = SceneManager.GetActiveScene().buildIndex;
            gameManager.EndGame();
        }
    }

    void OnTriggerStay2D( Collider2D collision )
    {
        if ( collision.gameObject.name == "ITEM_beam(Clone)" )
        {
            Destroy(collision.gameObject);
            ITEM_beamInfo.quantity++;
            StartCoroutine(gameManager.FirstGetITEM_beam());
            gameManager.SetITEM_beamQuantity();
            soundManager.Produce_GetITEM_beamSound();
        }
    }

    void MoveBall ()
    {
        ball_rb.AddForce( new Vector2(x, y) * BallSpeed);
    }

    void DecisionDegree ()
    {
        for (; ; )
        {
            BallDegree = Random.Range(200, 340);
            if ( BallDegree >= 300 || BallDegree <= 240 )
            {
                break;
            }
        }

        x = Mathf.Cos(BallDegree * Mathf.PI / 180);
        y = Mathf.Sin(BallDegree * Mathf.PI / 180);
    }

    void PlusBallSpeed ()
    {
        float PlusX, PlusY;
        
        PlusX = ball_rb.linearVelocity.x / (now_v / first_v) * (PlusDoubleBallSpeed - 1);
        PlusY = ball_rb.linearVelocity.y / (now_v / first_v) * (PlusDoubleBallSpeed - 1);

        ball_rb.linearVelocity = new Vector2(ball_rb.linearVelocity.x + PlusX, ball_rb.linearVelocity.y + PlusY);
    }
    
}
