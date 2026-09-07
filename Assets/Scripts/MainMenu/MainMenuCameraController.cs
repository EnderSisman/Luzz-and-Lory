using System.Collections;
using UnityEngine;

public class MainMenuCameraController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera mainCamera;

    [Header("Views")]
    [SerializeField] private Transform bankView;
    [SerializeField] private Transform saloonView;
    [SerializeField] private Transform sheriffView;
    [SerializeField] private Transform bankDoorZoomView;

    [Header("Navigation Signs")]
    [SerializeField] private RectTransform bankLeftSign;
    [SerializeField] private RectTransform saloonLeftSign;
    [SerializeField] private RectTransform saloonRightSign;
    [SerializeField] private RectTransform sheriffRightSign;

    [Header("Bank Room")]
    [SerializeField] private CanvasGroup bankRoomUI;
    [SerializeField] private RectTransform infoSign;
    [SerializeField] private RectTransform backSign;
    [SerializeField] private RectTransform resetSign;

    [Header("Camera Movement")]
    [SerializeField] private float moveDuration = 1.2f;

    [Header("Sign Slide")]
    [SerializeField] private float signOffsetY = -250f;
    [SerializeField] private float signSlideDuration = 0.35f;
    [SerializeField] private float delayBetweenSigns = 0.1f;

    [Header("Bank UI Fade")]
    [SerializeField] private float bankUIFadeDelay = 0.35f;
    [SerializeField] private float bankUIFadeDuration = 0.5f;

    [Header("Bank Sign Sequence")]
    [SerializeField] private float bankSignDelay = 0.15f;

    [Header("Info Sign")]
    [SerializeField] private float infoSignOffsetY = -600f;

    private enum CurrentView
    {
        Bank,
        Saloon,
        Sheriff,
        BankDoor
    }

    private CurrentView currentView = CurrentView.Saloon;

    private Vector2 bankLeftTarget;
    private Vector2 saloonLeftTarget;
    private Vector2 saloonRightTarget;
    private Vector2 sheriffRightTarget;

    private Vector2 infoSignTarget;
    private Vector2 backSignTarget;
    private Vector2 resetSignTarget;

    private bool isMoving;

    private void Awake()
    {
        if (bankLeftSign)
            bankLeftTarget = bankLeftSign.anchoredPosition;

        if (saloonLeftSign)
            saloonLeftTarget = saloonLeftSign.anchoredPosition;

        if (saloonRightSign)
            saloonRightTarget = saloonRightSign.anchoredPosition;

        if (sheriffRightSign)
            sheriffRightTarget = sheriffRightSign.anchoredPosition;

        if (infoSign)
            infoSignTarget = infoSign.anchoredPosition;

        if (backSign)
            backSignTarget = backSign.anchoredPosition;

        if (resetSign)
            resetSignTarget = resetSign.anchoredPosition;

        if (bankRoomUI)
        {
            bankRoomUI.alpha = 0f;
            bankRoomUI.interactable = false;
            bankRoomUI.blocksRaycasts = false;
        }

        PrepareInfoSign();
        
        PrepareSign(backSign, backSignTarget);
        
        PrepareSign(resetSign, resetSignTarget);
    }

    private void Start()
    {
        if (mainCamera && saloonView)
        {
            mainCamera.transform.position = saloonView.position;
            mainCamera.transform.rotation = saloonView.rotation;
        }

        currentView = CurrentView.Saloon;

        PrepareAllNavigationSigns();

        StartCoroutine(SlideInSignsForCurrentView());
    }

    public void GoToBank()
    {
        if (isMoving)
            return;

        StartCoroutine(ChangeView(bankView, CurrentView.Bank));
    }

    public void GoToSaloon()
    {
        if (isMoving)
            return;

        StartCoroutine(ChangeView(saloonView, CurrentView.Saloon));
    }

    public void GoToSheriff()
    {
        if (isMoving)
            return;

        StartCoroutine(ChangeView(sheriffView, CurrentView.Sheriff));
    }

    public void GoToBankDoor()
    {
        if (isMoving)
            return;

        StartCoroutine(EnterBankRoom());
    }

    public void ExitBankRoom()
    {
        if (isMoving)
            return;

        StartCoroutine(LeaveBankRoom());
    }

    private IEnumerator ChangeView(Transform targetView, CurrentView targetState)
    {
        if (!targetView || !mainCamera)
            yield break;

        isMoving = true;

        yield return SlideOutCurrentSigns();

        yield return MoveCamera(targetView);

        currentView = targetState;

        if (currentView != CurrentView.BankDoor)
        {
            yield return SlideInSignsForCurrentView();
        }

        isMoving = false;
    }

    private IEnumerator EnterBankRoom()
    {
        if (!bankDoorZoomView || !mainCamera)
            yield break;

        isMoving = true;

        yield return SlideOutCurrentSigns();
        StartCoroutine(MoveCamera(bankDoorZoomView));
        
        yield return new WaitForSecondsRealtime(bankUIFadeDelay);
        yield return FadeCanvasGroup(bankRoomUI, 0f, 1f, bankUIFadeDuration);
        currentView = CurrentView.BankDoor;

        if (infoSign)
        {
            PrepareInfoSign();
            yield return SlideInWithOffset(infoSign, infoSignTarget, infoSignOffsetY);
        }

        yield return new WaitForSecondsRealtime(bankSignDelay);
 
        if (backSign)
        {
            PrepareSign(backSign, backSignTarget);
            yield return SlideIn(backSign, backSignTarget);
        }

        yield return new WaitForSecondsRealtime(bankSignDelay);
        
        if (resetSign)
        {
            PrepareSign(resetSign, resetSignTarget);
            yield return SlideIn(resetSign, resetSignTarget);
        }

        isMoving = false;
    }

    private IEnumerator LeaveBankRoom()
    {
        if (!bankView || !mainCamera)
            yield break;

        isMoving = true;

        if (resetSign)
        {
            yield return SlideOut(resetSign, resetSignTarget);
        }

        yield return new WaitForSecondsRealtime(bankSignDelay);

        if (backSign)
        {
            yield return SlideOut(backSign, backSignTarget);
        }

        yield return new WaitForSecondsRealtime(bankSignDelay);

        if (infoSign)
        {
            yield return SlideOutWithOffset(infoSign, infoSignTarget, infoSignOffsetY);
        }

        yield return FadeCanvasGroup(bankRoomUI, 1f, 0f, bankUIFadeDuration);
        yield return MoveCamera(bankView);
        currentView = CurrentView.Bank;
        yield return SlideInSignsForCurrentView();

        isMoving = false;
    }

    private IEnumerator MoveCamera(Transform targetView)
    {
        Vector3 startPosition = mainCamera.transform.position;
        Quaternion startRotation = mainCamera.transform.rotation;
        Vector3 targetPosition = targetView.position;
        Quaternion targetRotation = targetView.rotation;

        float timer = 0f;

        while (timer < moveDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(timer / moveDuration);
            float easedProgress = progress * progress * progress * (progress * (progress * 6f - 15f) + 10f);

            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, easedProgress);
            mainCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, easedProgress);

            yield return null;
        }

        mainCamera.transform.position = targetPosition;
        mainCamera.transform.rotation = targetRotation;
    }

    private IEnumerator SlideOutCurrentSigns()
    {
        switch (currentView)
        {
            case CurrentView.Bank:

                yield return SlideOut(saloonRightSign, saloonRightTarget);
                break;

            case CurrentView.Saloon:

                StartCoroutine(SlideOut(bankLeftSign, bankLeftTarget));

                yield return new WaitForSecondsRealtime(delayBetweenSigns);

                yield return SlideOut(sheriffRightSign, sheriffRightTarget);
                break;

            case CurrentView.Sheriff:

                yield return SlideOut(saloonLeftSign, saloonLeftTarget);
                break;

            case CurrentView.BankDoor:
                break;
        }
    }

    private IEnumerator SlideInSignsForCurrentView()
    {
        switch (currentView)
        {
            case CurrentView.Bank:

                PrepareSign(saloonRightSign, saloonRightTarget);

                yield return SlideIn(saloonRightSign, saloonRightTarget);
                break;

            case CurrentView.Saloon:

                PrepareSign(bankLeftSign, bankLeftTarget);
                PrepareSign(sheriffRightSign, sheriffRightTarget);

                StartCoroutine(SlideIn(bankLeftSign, bankLeftTarget));

                yield return new WaitForSecondsRealtime(delayBetweenSigns);

                yield return SlideIn(sheriffRightSign, sheriffRightTarget);
                break;

            case CurrentView.Sheriff:

                PrepareSign(saloonLeftSign, saloonLeftTarget);

                yield return SlideIn(saloonLeftSign, saloonLeftTarget);
                break;

            case CurrentView.BankDoor:
                break;
        }
    }

    private void PrepareAllNavigationSigns()
    {
        PrepareSign(bankLeftSign, bankLeftTarget);
        PrepareSign(saloonLeftSign, saloonLeftTarget);
        PrepareSign(saloonRightSign, saloonRightTarget);
        PrepareSign(sheriffRightSign, sheriffRightTarget);
    }

    private void PrepareInfoSign()
    {
        if (!infoSign)
            return;

        infoSign.gameObject.SetActive(false);
        infoSign.anchoredPosition = infoSignTarget + new Vector2(0f, infoSignOffsetY);
    }

    private void PrepareSign(RectTransform sign, Vector2 targetPosition)
    {
        if (!sign)
            return;

        sign.gameObject.SetActive(false);
        sign.anchoredPosition = targetPosition + new Vector2(0f, signOffsetY);
    }

    private IEnumerator SlideIn(RectTransform sign, Vector2 targetPosition)
    {
        if (!sign)
            yield break;

        sign.gameObject.SetActive(true);

        Vector2 startPosition = targetPosition + new Vector2(0f, signOffsetY);

        sign.anchoredPosition = startPosition;

        float timer = 0f;

        while (timer < signSlideDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(timer / signSlideDuration);
            float easedProgress = 1f - Mathf.Pow(1f - progress, 3f);

            sign.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, easedProgress);

            yield return null;
        }

        sign.anchoredPosition = targetPosition;
    }

    private IEnumerator SlideOut(RectTransform sign, Vector2 targetPosition)
    {
        if (!sign)
            yield break;

        Vector2 startPosition = sign.anchoredPosition;

        Vector2 endPosition = targetPosition + new Vector2(0f, signOffsetY);

        float timer = 0f;

        while (timer < signSlideDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(timer / signSlideDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            
            sign.anchoredPosition = Vector2.Lerp(startPosition, endPosition, easedProgress);

            yield return null;
        }

        sign.anchoredPosition = endPosition;
        sign.gameObject.SetActive(false);
    }

    private IEnumerator SlideInWithOffset(RectTransform sign, Vector2 targetPosition, float customOffsetY)
    {
        if (!sign)
            yield break;

        sign.gameObject.SetActive(true);

        Vector2 startPosition = targetPosition + new Vector2(0f, customOffsetY);

        sign.anchoredPosition = startPosition;

        float timer = 0f;

        while (timer < signSlideDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(timer / signSlideDuration);
            float easedProgress = 1f - Mathf.Pow(1f - progress, 3f);
            
            sign.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, easedProgress);
            
            yield return null;
        }

        sign.anchoredPosition = targetPosition;
    }

    private IEnumerator SlideOutWithOffset(RectTransform sign, Vector2 targetPosition, float customOffsetY)
    {
        if (!sign)
            yield break;

        Vector2 startPosition = sign.anchoredPosition;

        Vector2 endPosition = targetPosition + new Vector2(0f, customOffsetY);
        
        float timer = 0f;

        while (timer < signSlideDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(timer / signSlideDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

            sign.anchoredPosition = Vector2.Lerp(startPosition, endPosition, easedProgress);

            yield return null;
        }

        sign.anchoredPosition = endPosition;

        sign.gameObject.SetActive(false);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float startAlpha, float endAlpha, float duration)
    {
        if (!group)
            yield break;

        group.alpha = startAlpha;
        group.interactable = false;
        group.blocksRaycasts = false;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(timer / duration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

            group.alpha = Mathf.Lerp(startAlpha, endAlpha, easedProgress);

            yield return null;
        }

        group.alpha = endAlpha;

        bool visible = endAlpha > 0.5f;

        group.interactable = visible;
        group.blocksRaycasts = visible;
    }
}