using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreItemControoler : MonoBehaviour
{

    [Header("Raw")]
    public Image Panel;
    public Image Achievements1;
    public Image Achievements2;
    public Image Rank;
    public Image Cover;
    public Image InfoiconMode;

    [Header("Text")]
    public TextMeshProUGUI SongName;
    public TextMeshProUGUI RankInB50;
    public TextMeshProUGUI ID;
    public TextMeshProUGUI DXScore;
    public TextMeshProUGUI Acc;
    public TextMeshProUGUI Level;
    public TextMeshProUGUI Rating;


    [Header("DXScore Star")]
    public GameObject Star5;
    public GameObject Star4;
    public GameObject Star3;
    public GameObject Star2;
    public GameObject Star1;

    [Header("Panel")]


    /// <summary>
    /// 0 = BSC, 1 = ADV, 2 = EXP, 3 = MST, 4 = RE
    /// </summary>
    public Sprite[] PanelBackgroundImages;
    /// <summary>
    /// 0 = FC, 1 = FCP, 2 = AP, 3 = APP
    /// </summary>
    public Sprite[] PanelAchievements1;
    /// <summary>
    /// 0 = SP, 1 = FS, 2 = FSP, 3 = FDX, 4 = FDXP
    /// </summary>
    public Sprite[] PanelAchievements2;
    /// <summary>
    /// 0 = D, 1 = C, 2 = B, 3 = BB, 4 = BBB, 5 = A, 6 = AA, 7 = AAA, 8 = S, 9 = SP, 10 = SS, 11 = SSP, 12 = SSS, 13 = SSSP
    /// </summary>
    public Sprite[] PanelRank;
    /// <summary>
    /// 0 = Standard, 1 = Deluxe
    /// </summary>
    public Sprite[] PanelInfoiconMode;

    void Start()
    {
        SongName.text = "¥Ä¥à¥®¥Ü¥·";
        RankInB50.text = "#50";
        ID.text = "ID:115293";
        DXScore.text = "3003/3012";
        Acc.text = "100.9982%";
        Level.text = "14.0¡ú";
        Rating.text = "310";
        Panel.sprite = PanelBackgroundImages[3];
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
