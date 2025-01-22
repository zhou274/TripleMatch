using TMPro;
using UnityEngine;
using UnityEngine.UI;
using StarkSDKSpace;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;


public class EndLevelScreen : MonoBehaviour
{
    [SerializeField] Image CloseButton;
    [SerializeField] Image Ribbon;
    [SerializeField] TMP_Text RibbonText;
    [SerializeField] RectTransform Stars;
    [SerializeField] Image StarLeft;
    [SerializeField] Image StarMid;
    [SerializeField] Image StarRight;
    [SerializeField] RectTransform TimeLeft;
    [SerializeField] TMP_Text TimeLeft_Text;
    [SerializeField] Image ContinueButton;
    [SerializeField] GameObject restartButton;
    // TODO: make more elaborate
    [SerializeField] RectTransform Booster_Choice;

    [SerializeField] Sprite RibbonPositive;
    [SerializeField] Sprite RibbonNegative;
    [SerializeField] string RibbonTextPositive = "游戏成功";
    [SerializeField] string RibbonTextNegative = "游戏失败";
    private StarkAdManager starkAdManager;

    public string clickid;

    public void InitWinScreen(string timeString)
    {
        Deconstruct(); // just in case
        gameObject.SetActive(true);
        InitCommonParts();

        Ribbon.sprite = RibbonPositive;
        RibbonText.text = "游戏成功";
        Stars.gameObject.SetActive(true);
        TimeLeft.gameObject.SetActive(true);
        TimeLeft_Text.text = timeString;
        restartButton.SetActive(false);
        ShowInterstitialAd("27u8cdewpt6j293lmo",
            () => {

            },
            (it, str) => {
                Debug.LogError("Error->" + str);
            });
    }
    public void HideLoseScreen()
    {
        gameObject.SetActive(false);
    }
    public void InitLoseScreen()
    {
        Deconstruct(); // just in case
        gameObject.SetActive(true);
        InitCommonParts();

        Ribbon.sprite = RibbonNegative;
        RibbonText.text = "游戏失败";
        //Booster_Choice.gameObject.SetActive(true);
        restartButton.SetActive(true);
        ShowInterstitialAd("27u8cdewpt6j293lmo",
            () => {

            },
            (it, str) => {
                Debug.LogError("Error->" + str);
            });
    }

    private void InitCommonParts()
    {
        CloseButton.gameObject.SetActive(true);
        Ribbon.gameObject.SetActive(true);
        ContinueButton.gameObject.SetActive(true);
    }

    public void Deconstruct()
    {
        CloseButton.gameObject.SetActive(false);
        Ribbon.gameObject.SetActive(false);
        RibbonText.text = "";
        Stars.gameObject.SetActive(false);
        TimeLeft.gameObject.SetActive(false);
        TimeLeft_Text.text = "";
        ContinueButton.gameObject.SetActive(false);
        Booster_Choice.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 播放插屏广告
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="errorCallBack"></param>
    /// <param name="closeCallBack"></param>
    public void ShowInterstitialAd(string adId, System.Action closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            var mInterstitialAd = starkAdManager.CreateInterstitialAd(adId, errorCallBack, closeCallBack);
            mInterstitialAd.Load();
            mInterstitialAd.Show();
        }
    }

}
