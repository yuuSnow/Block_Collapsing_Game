using System.Collections; //Unityでコルーチンを用いるために必要
using UnityEngine;

public class BeamController : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public GameObject player;
    public float beamLength = 10f;
    public float beamDuration = 0.1f;
    public float beamcolliderRange = 0.1f;
    public float offset = 0.05f;
    public GameObject mainCamera;
    public GameManager gameManager;
    public SoundManager soundManager;

    // Update is called once per frame
    void Update()
    {
        if ( Input.GetKeyDown(KeyCode.Space) && ITEM_beamInfo.quantity > 0 && GameInfo.OriginalBallisMoving )
        {
            StartCoroutine(Beam());
            soundManager.Produce_BeamSound();
            ITEM_beamInfo.quantity--;
            gameManager.SetITEM_beamQuantity();
        }
    }

    IEnumerator Beam () // yield return new WaitForSeconds()を使うために必要。(voidではすぐに実行されてしまう)
    {
        Vector3 BeamTransform = player.transform.position + Vector3.up * player.transform.localScale.y;
        RaycastHit2D[] hits;

        lineRenderer.SetPosition(0, player.transform.position); // index : 0 開始地点 
        
        hits = Physics2D.RaycastAll(BeamTransform + Vector3.left * beamcolliderRange / 2, transform.up, beamLength);
        // プレイヤーの上に見えない線を発射しあたったものを配列に入れている。
        if ( hits != null )
        {
            HitGameObjectDestroy(hits);
        }
        else
        {
            hits = Physics2D.RaycastAll(BeamTransform + Vector3.right * beamcolliderRange / 2, transform.up, beamLength);
            HitGameObjectDestroy(hits);
            // ビームが当たらないのを防ぐ
        }
        
        lineRenderer.SetPosition(1, player.transform.position + Vector3.up * beamLength); // index : 1 終了位置

        lineRenderer.enabled = true;
        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y + offset, -10);
        yield return new WaitForSeconds(beamDuration);
        lineRenderer.enabled = false;
        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y - offset, -10);

        StartCoroutine(gameManager.LevelChangeJudgement());
    }

    void HitGameObjectDestroy ( RaycastHit2D[] hits )
    {
        foreach ( RaycastHit2D hit in hits )
        {
            if ( hit.collider.tag == "Target" )
            {
                Destroy(hit.collider.gameObject);
            }
        }
        // foreach( 型 変数名 in 配列名): すべての配列の各々に実行する。（その中では変数名で１つずつアクセスできる）
    }
}