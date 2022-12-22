using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SlideButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float slideTime = 1;

    public string actionMessage = "Slide complete";

    public Vector3 sliderFixedScale = new Vector3(1, 1, 1);

    public float textSize = 10f;

    public Color textColor = new Color32(0xFF, 0xFF, 0xFF, 255);

    public Image spriteSource;

    public Color sliderColor = new Color32(0xFF, 0xFF, 0xFF, 255);

    public Slider.Direction slideDirection;

    public bool doActionAfterText = false;

    private GlobalData globalData;

    public UnityEvent manualAction;

    [SerializeField] private bool canSlide = true;

    public float cantSlideAlpha = 0.5f;

    public bool CanSlide
    {
        get
        {
            return canSlide;
        }

        set
        {
            canSlide = value;

            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();

            if (canSlide)
                canvasGroup.alpha = 1f;
            else
                canvasGroup.alpha = cantSlideAlpha;
        }
    }

    private GameObject sliderInst;

    private bool canStop = true;

    private CanvasGroup canvasGroup;

    private void OnDisable()
    {
        StopAllCoroutines();
        Destroy(sliderInst);
        GameInstance.instance.canDoItemAction = true;
        canStop = true;
    }

    private void Start()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        globalData = GameInstance.instance.referenceValues.globalData;

        if (canSlide)
            canvasGroup.alpha = 1f;
        else
            canvasGroup.alpha = cantSlideAlpha;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (CanSlide)
        {
            StartCoroutine(Slide());
            GameInstance.instance.canDoItemAction = false;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (canStop)
        {
            StopAllCoroutines();
            Destroy(sliderInst);
            GameInstance.instance.canDoItemAction = true;
        }
    }

    public IEnumerator Slide()
    {
        float value = 0;

        sliderInst = Instantiate(globalData.sliderPrefab, transform);

        sliderInst.transform.localScale = sliderFixedScale;

        Slider slider = sliderInst.GetComponent<Slider>();

        Image image = slider.fillRect.GetComponent<Image>();
        switch (slideDirection)
        {
            case Slider.Direction.LeftToRight:
                image.fillMethod = Image.FillMethod.Horizontal;
                image.fillOrigin = (int)Image.OriginHorizontal.Left;
                break;
            case Slider.Direction.RightToLeft:
                image.fillMethod = Image.FillMethod.Horizontal;
                image.fillOrigin = (int)Image.OriginHorizontal.Right;
                break;
            case Slider.Direction.BottomToTop:
                image.fillMethod = Image.FillMethod.Vertical;
                image.fillOrigin = (int)Image.OriginVertical.Bottom;
                break;
            case Slider.Direction.TopToBottom:
                image.fillMethod = Image.FillMethod.Vertical;
                image.fillOrigin = (int)Image.OriginVertical.Top;
                break;
        }

        image.color = sliderColor;
        image.sprite = spriteSource.sprite;

        while (value < 1)
        {
            value += (Time.deltaTime * 1.5f) / slideTime;
            slider.value = value;

            yield return null;
        }

        GameInstance.instance.canDoItemAction = true;
        StartCoroutine(MessageDelay(sliderInst));
    }

    private IEnumerator MessageDelay(GameObject sliderInst)
    {
        canStop = false;

        if (!doActionAfterText)
            manualAction?.Invoke();

        sliderInst.transform.GetChild(1).gameObject.SetActive(true);
        sliderInst.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = actionMessage;
        sliderInst.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().color = textColor;
        sliderInst.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().fontSize = textSize;

        yield return new WaitForSeconds(0.3f);

        sliderInst.GetComponent<Animation>().Play("SlideButtonMessageTextFade");

        yield return new WaitForSeconds(0.2f);

        if (doActionAfterText)
            manualAction?.Invoke();

        canStop = true;
        Destroy(sliderInst);
    }
}
