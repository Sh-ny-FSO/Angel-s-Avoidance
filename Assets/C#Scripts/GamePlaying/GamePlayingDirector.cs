using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GamePlayingDirector : MonoBehaviour
{
    [SerializeField] private GameObject startUi;
    [SerializeField] private GameObject smartPhoneLeftUi;
    [SerializeField] private GameObject smartPhoneRightUi;
    [SerializeField] private GameObject pauseUi;
    [SerializeField] private GameObject platformUi;
    [SerializeField] private GameObject gameOverUi;
    [SerializeField] private GameObject gameClearUi;
    [SerializeField] private Sprite energy;
    [SerializeField] private Sprite fatigue;

    private Image noLivesBar;
    private Image livesBar;
    private Image energyBar;
    private Image upDown;
    private Image forwardBack;
    private TextMeshProUGUI timerText;
    private GameEndingDirector nextDirector;
    private float nowTimeLeft; // 現在の残り時間
    private int iTimeLeft; // 整数化した残り時間

    // インタフェースを使える状態かどうか
    private bool canUseInterf = false;

    public bool CanUseInterf
    {
        get { return canUseInterf; }
    }

    // ボタンを使える状態かどうか
    private bool canUseButton = false;

    public bool CanUseButton
    {
        get { return canUseButton; }
    }

    // 現在のプレイヤーの体力
    private int nowPlayerLives;

    public int NowPlayerLives
    {
        get { return nowPlayerLives; }
        set { nowPlayerLives = value; }
    }

    // ポーズ画面へ遷移するかどうか
    private bool pauseSwitch = false;

    public bool PauseSwitch
    {
        get { return pauseSwitch; }
        set { pauseSwitch = value; }
    }

    // ゲームプレイを続行するかどうか
    private bool continueSwitch = false;

    public bool ContinueSwitch
    {
        get { return continueSwitch; }
        set { continueSwitch = value; }
    }

    // ゲームプレイを再開始するかどうか
    private bool restartSwitch = false;

    public bool RestartSwitch
    {
        get { return restartSwitch; }
        set { restartSwitch = value; }
    }

    // プラットフォーム画面へ遷移するかどうか
    private bool platformSwitch = false;

    public bool PlatformSwitch
    {
        get { return platformSwitch; }
        set { platformSwitch = value; }
    }

    // オープニングへ遷移するかどうか
    private bool openingSwitch = false;

    public bool OpeningSwitch
    {
        get { return openingSwitch; }
        set { openingSwitch = value; }
    }

    // 移動モードが「上下」かどうか
    private bool modeChange = false;

    public bool ModeChange
    {
        get { return modeChange; }
        set { modeChange = value; }
    }

    // 疲労状態かどうか
    private bool fatigueSwitch = false;

    public bool FatigueSwitch
    {
        get { return fatigueSwitch; }
        set { fatigueSwitch = value; }
    }

    // 気力回復時間
    private float chargeTime = 0f;

    public float ChargeTime
    {
        get { return chargeTime; }
        set { chargeTime = value; }
    }

    // 疲労状態回数
    private int fatigueCnt = 0;

    public int FatigueCnt
    {
        get { return fatigueCnt; }
        set { fatigueCnt = value; }
    }

    private void Start()
    {
        // 体力バー（黒色）のイメージコンポーネントを取得する
        noLivesBar = GameObject.Find("No Lives Bar").GetComponent<Image>();

        // 体力バー（緑色）のイメージコンポーネントを取得する
        livesBar = GameObject.Find("Lives Bar").GetComponent<Image>();

        // 気力バーのイメージコンポーネントを取得する
        energyBar = GameObject.Find("Energy Bar").GetComponent<Image>();

        // 上下モード画像のイメージコンポーネントを取得する
        upDown = GameObject.Find("UpDown Mode Image").GetComponent<Image>();

        // 前後モード画像のイメージコンポーネントを取得する
        forwardBack = GameObject.Find("ForwardBack Mode Image").GetComponent<Image>();

        // タイマーのテキストコンポーネントを取得する
        timerText = GameObject.Find("Timer Text").GetComponent<TextMeshProUGUI>();

        // スタート画面を有効にする
        startUi.SetActive(true);
        smartPhoneLeftUi.SetActive(false);
        smartPhoneRightUi.SetActive(false);
        pauseUi.SetActive(false);
        platformUi.SetActive(false);
        gameOverUi.SetActive(false);
        gameClearUi.SetActive(false);

        // 現在のプレイヤーの体力を設定する
        nowPlayerLives = StaticUnits.MaxPlayerLives;

        // 上下モード画像を無効にする
        upDown.enabled = false;

        // 現在の残り時間を設定する
        nowTimeLeft = StaticUnits.GameTime;

        // ゲームプレイを開始する（1回の待機あり）
        StartCoroutine(GameStart(3.0f));
    }

    private void Update()
    {
        // 現在のプラットフォームがスマホであれば
        if (StaticUnits.SmartPhone)
        {
            // スマホUI表示を有効にする
            smartPhoneLeftUi.SetActive(true);
            smartPhoneRightUi.SetActive(true);
        }
        // 現在のプラットフォームがパソコンであれば
        else
        {
            // スマホUI表示を無効にする
            smartPhoneLeftUi.SetActive(false);
            smartPhoneRightUi.SetActive(false);
        }

        // インタフェースを使える状態で
        if (canUseInterf)
        {
            // Xキーを入力すると
            if (Input.GetKeyDown(KeyCode.X))
            {
                // ポーズ画面への遷移を有効にする
                pauseSwitch = true;
            }
        }

        /* ポーズ画面へ遷移する */
        if (pauseSwitch)
        {
            pauseSwitch = false;

            // ゲームの一時停止を実行する
            Time.timeScale = 0f;

            // インタフェースを使えない状態にする
            canUseInterf = false;

            // ポーズ画面を有効にする
            pauseUi.SetActive(true);
        }
        
        /* ゲームプレイを続行する */
        if (continueSwitch)
        {
            continueSwitch = false;

            // ポーズ画面を無効にする
            pauseUi.SetActive(false);

            // インタフェースを使える状態にする
            canUseInterf = true;

            // ゲームの一時停止を解除する
            Time.timeScale = 1.0f;
        }

        /* ゲームプレイを再開始する（1回の待機あり） */
        if (restartSwitch)
        {
            restartSwitch = false;

            // ボタンを使えない状態にする
            canUseButton = false;

            StartCoroutine(GameRestart(0.3f));
        }

        /* プラットフォーム画面へ遷移する */
        if (platformSwitch)
        {
            // プラットフォーム画面を有効にする
            platformUi.SetActive(true);
        }
        else
        {
            // プラットフォーム画面を無効にする
            platformUi.SetActive(false);
        }

        /* オープニングへ遷移する（1回の待機あり） */
        if (openingSwitch)
        {
            openingSwitch = false;

            // ボタンを使えない状態にする
            canUseButton = false;

            StartCoroutine(ToOpening(0.3f));
        }

        // 移動モードが「上下」であれば
        if (modeChange)
        {
            // 前後モード画像を無効にする
            forwardBack.enabled = false;

            // 上下モード画像を有効にする
            upDown.enabled = true;
        }
        // 移動モードが「前後」であれば
        else
        {
            // 前後モード画像を有効にする
            forwardBack.enabled = true;

            // 上下モード画像を無効にする
            upDown.enabled = false;
        }

        // 体力バー（黒色）を表示する
        noLivesBar.fillAmount = StaticUnits.MaxPlayerLives / 5.0f;

        // 体力バー（緑色）を更新する
        livesBar.fillAmount = nowPlayerLives / 5.0f;

        // 体力がゼロになれば
        if (nowPlayerLives <= 0)
        {
            // インタフェースを使えない状態にする
            canUseInterf = false;

            // ボタンを使えない状態にする
            canUseButton = false;

            // ゲームオーバーになる（1回の待機あり）
            StartCoroutine(GameOver(2.0f));
        }

        // 通常状態であれば
        if (!fatigueSwitch)
        {
            // 気力バーの色を黄色にする
            energyBar.sprite = energy;

            // 気力バーを更新する
            energyBar.fillAmount = 1.0f - chargeTime / 3.0f;
        }
        // 疲労状態であれば
        else
        {
            // 気力バーの色を水色にする
            energyBar.sprite = fatigue;

            // 気力バーを更新する
            energyBar.fillAmount = 1.0f - chargeTime / 5.0f;
        }

        // 現在の残り時間を経過させる
        nowTimeLeft -= Time.deltaTime;

        // 残り時間を整数化する
        iTimeLeft = (int)nowTimeLeft;

        // タイマーを更新する
        timerText.text = iTimeLeft.ToString();

        // 残り時間がゼロになれば
        if (nowTimeLeft < 0f)
        {
            // 時間を初期化する（正常な処理のため）
            nowTimeLeft = 0f;

            // インタフェースを使えない状態にする
            canUseInterf = false;

            // ボタンを使えない状態にする
            canUseButton = false;

            // ゲームクリアになる（1回の待機あり）
            StartCoroutine(GameClear(5.0f));
        }
    }

    private IEnumerator GameStart(float fWT)
    {
        // 1回目の待機
        yield return new WaitForSecondsRealtime(fWT);

        // インタフェースを使える状態にする
        canUseInterf = true;

        // ボタンを使える状態にする
        canUseButton = true;

        // スタート画面を無効にする
        startUi.SetActive(false);

        // ゲームの一時停止を解除する
        Time.timeScale = 1.0f;
    }

    private IEnumerator GameRestart(float fWT)
    {
        // 1回目の待機
        yield return new WaitForSecondsRealtime(fWT);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator ToOpening(float fWT)
    {
        // 1回目の待機
        yield return new WaitForSecondsRealtime(fWT);

        // ゲームの一時停止を解除する
        Time.timeScale = 1.0f;

        SceneManager.LoadScene("OpeningScene");
    }

    private IEnumerator GameOver(float fWT)
    {
        // ゲームオーバー画面を有効にする
        gameOverUi.SetActive(true);

        // ゲームの一時停止を実行する
        Time.timeScale = 0f;

        // 1回目の待機
        yield return new WaitForSecondsRealtime(fWT);

        // ボタンを使える状態にする
        canUseButton = true;
    }

    private IEnumerator GameClear(float fWT)
    {
        // ゲームクリア画面を有効にする
        gameClearUi.SetActive(true);

        // ゲームの一時停止を実行する
        Time.timeScale = 0f;

        // 1回目の待機
        yield return new WaitForSecondsRealtime(fWT);

        // 次のシーンへの変数引き渡しを開始する
        SceneManager.sceneLoaded += GameEndingLoaded;

        // ゲームの一時停止を解除する
        Time.timeScale = 1.0f;

        // ゲームエンディングへ遷移する
        SceneManager.LoadScene("GameEndingScene");
    }

    private void GameEndingLoaded(Scene scene, LoadSceneMode mode)
    {
        // ゲームエンディングディレクターを取得する
        nextDirector = GameObject.FindGameObjectWithTag("Director").GetComponent<GameEndingDirector>();

        // 被弾回数を設定する
        nextDirector.Damaged = StaticUnits.MaxPlayerLives - nowPlayerLives;

        // 疲労状態回数を設定する
        nextDirector.Fatigued = FatigueCnt;

        // 次のシーンへの変数引き渡しを終了する
        SceneManager.sceneLoaded -= GameEndingLoaded;
    }
}
