using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameInfo
{
    public static int player_score = 0;
    public static int now_index = 0;
    public static int max_index = 0;
    public static bool OriginalBallisMoving;
    public static float PlayerSpeedOriginal = 13f;
    public static string mode;
    public static int RestartIndex = 1;
}

public class MissionInfo
{
    public static bool Purple = false;
    public static bool PurpleGet = false;
    public static bool Score = false;
    public static bool Challenge = false;
    public static bool[] togglewasOn = {false, false, false};
    public static bool FirstTimeThere = false;
    public static int SelectingBgButton = 1;
    public static int SelectingSkinButton = 1;
    public static bool AllMissionAchievement = false;
}

public class ITEM_beamInfo
{
    public static int quantity = 0;
    public static bool FirstGet = false;
}

public class GameManager : MonoBehaviour
{
    public Text score;
    public Text StartTerms;
    public GameObject LevelUpUI;
    public GameObject GameClearUI;
    public GameObject Button_Level4;
    public GameObject Description_ITEM_beam;
    public GameObject Description_Mission;
    public GameObject Quit_Button;
    public GameObject Level_Selection_Pannel;
    public Text NumberOfITEM_beam;
    public GameObject spaceship_bg;
    public GameObject heaven_bg;
    public float ChangeSceneTime = 1f;

    
    int OneScene_score = 0;
    float HueValue, PlusHueValue;
    int LastLevelIndex = 3;
    int EndIndex = 4;
    int Level4Index = 5;
    int MissionIndex = 6;
    int SettingIndex = 7;
    int Level_Selection_Index = 8;
    GameObject canvas;

    // Start is called before the first frame update
    void Start ()
    {
        canvas = GameObject.Find("Canvas");

        if ( SceneManager.GetActiveScene().buildIndex != MissionIndex && SceneManager.GetActiveScene().buildIndex != Level_Selection_Index)
        {
            GameObject targetSpawnerOb, player;
            string SkinName = null;
            Color c;
        
            score.text = GameInfo.player_score.ToString();
            score.enabled = false;
            StartTerms.enabled = true;
        
            targetSpawnerOb = GameObject.Find("TargetSpawner");
            TargetSpawner targetSpawner = targetSpawnerOb.GetComponent<TargetSpawner>();
            PlusHueValue = 1f / (targetSpawner.NumberOfAddBTargets + 1);

            player = GameObject.Find("Player");
            SpriteRenderer player_sr = player.GetComponent<SpriteRenderer>();
            c = player_sr.color;

            if ( MissionInfo.SelectingBgButton == 2 )
            {
                Instantiate(spaceship_bg);
            }
            else if ( MissionInfo.SelectingBgButton == 3 )
            {
                Instantiate(heaven_bg);
            }

            if ( MissionInfo.SelectingSkinButton >= 2 && MissionInfo.SelectingSkinButton <= 4 )
            {
                SkinName = "芋けんぴ" + ( MissionInfo.SelectingSkinButton - 1 );
                Debug.Log(SkinName);
            }

            foreach ( Transform sweet_potatoTransform in player.transform )
            {
                if ( sweet_potatoTransform.gameObject.name == SkinName )
                {
                    sweet_potatoTransform.gameObject.SetActive(true);
                    c.a = 0f;
                    player_sr.color = c;
                }
            }
        }
        else if( SceneManager.GetActiveScene().buildIndex == MissionIndex )
        {
            if ( !MissionInfo.FirstTimeThere )
            {
                Description_Mission.SetActive(true);
            }
            StartCoroutine(AchieveMission());
            
            if ( MissionInfo.AllMissionAchievement )
            {
                GameObject button_Level4 = Instantiate(Button_Level4, canvas.transform);
                Animator button_Level4_ani = button_Level4.GetComponent<Animator>();
                button_Level4_ani.enabled = false;
            }
        }
        
    }

    public bool AddScore ()
    {
        GameInfo.player_score ++;
        OneScene_score ++;
        score.text = GameInfo.player_score.ToString();

        if ( OneScene_score % 10 == 0 && OneScene_score != 0 )
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public void StartGame ()
    {
        score.enabled = true;
        StartTerms.enabled = false;
    }

    public float AddHueValue ()
    {
        HueValue = (HueValue + PlusHueValue) % 1.00f;

        return HueValue;
    }

    public IEnumerator FirstGetITEM_beam ()
    {
        if ( ITEM_beamInfo.FirstGet == false )
        {
            Description_ITEM_beam.SetActive(true);
            
            Time.timeScale = 0;
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.UpArrow));
            
            Time.timeScale = 1f;
            Description_ITEM_beam.SetActive(false);
            ITEM_beamInfo.FirstGet = true;
        }
    }

    public void SetITEM_beamQuantity ()
    {
        NumberOfITEM_beam.text = ITEM_beamInfo.quantity.ToString();
    }

    public IEnumerator LevelChangeJudgement ()
    {
        yield return new WaitForSeconds(0.01f);
        // voidで実行されているからTargetが消えていないと判定される可能性があるから
        
        if ( GameObject.FindGameObjectsWithTag("Target").Length == 0 )
        {
            GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");

            foreach( GameObject ball in balls )
            {
                Rigidbody2D ball_rb = ball.GetComponent<Rigidbody2D>();
                ball_rb.linearVelocity = Vector2.zero;
            }

            LevelComplete();
        }
    }

    public void LevelComplete ()
    {
        if ( SceneManager.GetActiveScene().buildIndex == LastLevelIndex || GameInfo.mode == "Normal" )
        {
            if ( MissionInfo.PurpleGet )
            {
                MissionInfo.Purple = true;
            }

            if ( GameInfo.player_score >= 50 )
            {
                MissionInfo.Score = true;
            }

            if ( GameInfo.mode == "Challenge" )
            {
                MissionInfo.Challenge = true;
            }

            GameClearUI.SetActive(true);
        }
        else
        {
            LevelUpUI.SetActive(true);
        }
    }

    public void EndGame ()
    {
        Debug.Log("GAME OVER");
        GameInfo.max_index = SceneManager.GetActiveScene().buildIndex;
        Invoke("End", ChangeSceneTime);
    }

    public void Restart ()
    {
        GameInfo.player_score = 0;
        if ( GameInfo.mode == "Normal" && ( GameInfo.RestartIndex == 2 || GameInfo.RestartIndex == 3 ) )
        {
            ITEM_beamInfo.quantity = GameInfo.RestartIndex - 1;
        }
        else
        {
            ITEM_beamInfo.quantity = 0;
        }
        MissionInfo.PurpleGet = false;
        if ( GameInfo.mode == "Challenge" )
        {
            GameInfo.RestartIndex = 1;
        }
        SceneManager.LoadScene(GameInfo.RestartIndex);
    }

    public void LevelSkip ()
    {
        GameInfo.player_score = 0;
        if ( GameInfo.max_index == Level4Index )
        {
            ITEM_beamInfo.quantity = 1;
        }
        else
        {
            ITEM_beamInfo.quantity = 0; 
        }
        
        MissionInfo.PurpleGet = false;
        SceneManager.LoadScene(GameInfo.max_index);
    }

    public void End ()
    {
        SceneManager.LoadScene(EndIndex);
    }

    public void Setting ()
    {
        GameInfo.now_index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(SettingIndex);
    }

    public void Mission ()
    {
        GameInfo.now_index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(MissionIndex);
    }

    IEnumerator AchieveMission ()
    {
        yield return new WaitForSeconds(0.2f);
        GameObject[] MissionToggles = GameObject.FindGameObjectsWithTag("Mission_Toggle");
        foreach ( GameObject MissionToggle in MissionToggles )
        {
            Toggle toggle = MissionToggle.GetComponent<Toggle>();
            switch ( MissionToggle.name )
            {
                case "Mission1" :
                    toggle.isOn = MissionInfo.Purple;
                    break;
                
                case "Mission2" :
                    toggle.isOn = MissionInfo.Score;
                    break;
                
                case "Mission3" :
                    toggle.isOn = MissionInfo.Challenge;
                    break;
            }
        }

        yield return new WaitForSeconds(1.2f);
        if ( MissionInfo.togglewasOn[0] != MissionInfo.Purple )
        {
            Transform Mission1ITEMTransform = canvas.transform.Find("Mission1成功報酬");
            GameObject Mission1ITEM = Mission1ITEMTransform.gameObject;
            Mission1ITEM.SetActive(true);
        }

        if ( MissionInfo.togglewasOn[1] != MissionInfo.Score )
        {
            Transform Mission2ITEMTransform = canvas.transform.Find("Mission2成功報酬");
            GameObject Mission2ITEM = Mission2ITEMTransform.gameObject;
            Mission2ITEM.SetActive(true);
        }

        if ( MissionInfo.togglewasOn[2] != MissionInfo.Challenge )
        {
            Transform Mission3ITEMTransform = canvas.transform.Find("Mission3成功報酬");
            GameObject Mission3ITEM = Mission3ITEMTransform.gameObject;
            Mission3ITEM.SetActive(true);
        }
        
        MissionInfo.togglewasOn[0] = MissionInfo.Purple;
        MissionInfo.togglewasOn[1] = MissionInfo.Score;
        MissionInfo.togglewasOn[2] = MissionInfo.Challenge;
    }

    public void Delete_Description_Mission ()
    {
        Description_Mission.SetActive(false);
        Quit_Button.SetActive(true);
        MissionInfo.FirstTimeThere = true;
    }

    public void Active_Level_Selection_Pannel()
    {
        Level_Selection_Pannel.SetActive(true);
    }

    public void Inactive_Level_Selection_Pannel()
    {
        Level_Selection_Pannel.SetActive(false);
    }

    public void Leave ()
    {
        SceneManager.LoadScene(GameInfo.now_index);
    }

    public void Level4 ()
    {
        GameInfo.player_score = 0;
        ITEM_beamInfo.quantity = 1;
        SceneManager.LoadScene(Level4Index);
    }

    public void Quit ()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }

}