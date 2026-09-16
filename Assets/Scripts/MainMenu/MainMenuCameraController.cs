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
    [SerializeField] private Transform sheriffDoorZoomView;
    [SerializeField] private Transform saloonDoorZoomView;

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
    
    [Header("Sheriff Room")]
    [SerializeField] private CanvasGroup sheriffRoomUI;
    [SerializeField] private RectTransform sheriffBackSign;

    [Header("Sheriff Steuerung")]
    [SerializeField] private RectTransform steuerungPanel;
    [SerializeField] private float steuerungOffsetY = 300f;

    [Header("Sheriff Sound Button")]
    [SerializeField] private RectTransform sheriffSoundButton;
    [SerializeField] private float soundButtonScaleDuration = 0.35f;

    [Header("Timo Tresor")]
    [SerializeField] private RectTransform timoTresor;
    [SerializeField] private CanvasGroup timoTresorCanvasGroup;
    [SerializeField] private float timoOffsetX = -40f;
    [SerializeField] private float timoFadeDuration = 0.6f;

    [Header("Rolf Revolver")]
    [SerializeField] private RectTransform rolfRevolver;
    [SerializeField] private CanvasGroup rolfRevolverCanvasGroup;
    [SerializeField] private float rolfOffsetX = 40f;
    [SerializeField] private float rolfFadeDuration = 0.6f;

    [Header("Camera Movement")]
    [SerializeField] private float moveDuration = 1.2f;

    [Header("Sign Slide")]
    [SerializeField] private float signOffsetY = -250f;
    [SerializeField] private float signSlideDuration = 0.35f;
    [SerializeField] private float delayBetweenSigns = 0.1f;

    [Header("Bank Sign Sequence")]
    [SerializeField] private float bankSignDelay = 0.15f;

    [Header("Info Sign")]
    [SerializeField] private float infoSignOffsetY = -600f;
    
    [Header("Bank UI Fade")]
    [SerializeField] private float bankUIFadeDelay = 0.35f;
    [SerializeField] private float bankUIFadeDuration = 0.5f;

    [Header("Sheriff UI Fade")]
    [SerializeField] private float sheriffUIFadeDelay = 0.35f;
    [SerializeField] private float sheriffUIFadeDuration = 0.5f;

    private enum CurrentView
    {
        Bank,
        Saloon,
        Sheriff,
        BankDoor,
        SheriffDoor,
        SaloonDoor
    }

    private CurrentView currentView = CurrentView.Saloon;

    private Vector2 bankLeftTarget;
    private Vector2 saloonLeftTarget;
    private Vector2 saloonRightTarget;
    private Vector2 sheriffRightTarget;

    private Vector2 infoSignTarget;
    private Vector2 backSignTarget;
    private Vector2 resetSignTarget;

    private Vector2 timoTresorTarget;

    private Vector2 sheriffBackSignTarget;
    private Vector2 steuerungPanelTarget;

    private Vector2 rolfRevolverTarget;

    private Vector3 sheriffSoundButtonTargetScale;

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

        if (timoTresor)
            timoTresorTarget = timoTresor.anchoredPosition;

        if (sheriffBackSign)
            sheriffBackSignTarget = sheriffBackSign.anchoredPosition;

        if (steuerungPanel)
            steuerungPanelTarget = steuerungPanel.anchoredPosition;

        if (rolfRevolver)
            rolfRevolverTarget = rolfRevolver.anchoredPosition;

        if (sheriffSoundButton)
            sheriffSoundButtonTargetScale = sheriffSoundButton.localScale;

        if (bankRoomUI)
        {
            bankRoomUI.alpha = 0f;
            bankRoomUI.interactable = false;
            bankRoomUI.blocksRaycasts = false;
        }

        if (sheriffRoomUI)
        {
            sheriffRoomUI.alpha = 0f;
            sheriffRoomUI.interactable = false;
            sheriffRoomUI.blocksRaycasts = false;
        }

        PrepareInfoSign();
        PrepareSign(backSign, backSignTarget);
        PrepareSign(resetSign, resetSignTarget);
        PrepareTimoTresor();

        PrepareSign(sheriffBackSign, sheriffBackSignTarget);
        PrepareSteuerungPanel();
        PrepareRolfRevolver();
        PrepareSheriffSoundButton();
    }

    private IEnumerator Start()
    {
        if (mainCamera && saloonView)
        {
            mainCamera.transform.position = saloonView.position;
            mainCamera.transform.rotation = saloonView.rotation;
        }

        currentView = CurrentView.Saloon;
        PrepareAllNavigationSigns();

        if (ScreenFader.Instance)
        {
            while (ScreenFader.Instance.IsFading)
            {
                yield return null;
            }
        }

        yield return SlideInSignsForCurrentView();
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

    public void GoToSheriffDoor()
    {
        if (isMoving)
            return;

        StartCoroutine(EnterSheriffRoom());
    }

    public void ExitSheriffRoom()
    {
        if (isMoving)
            return;

        StartCoroutine(LeaveSheriffRoom());
    }

    public void GoToSaloonDoor()
    {
        if (isMoving)
            return;

        StartCoroutine(EnterSaloonRoom());
    }

    private IEnumerator ChangeView(Transform targetView, CurrentView targetState)
    {
        if (!targetView || !mainCamera)
            yield break;

        isMoving = true;

        yield return SlideOutCurrentSigns();
        yield return MoveCamera(targetView);

        currentView = targetState;

        if (currentView != CurrentView.BankDoor && currentView != CurrentView.SheriffDoor && currentView != CurrentView.SaloonDoor)
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

        if (timoTresor && timoTresorCanvasGroup)
            StartCoroutine(FadeInTimoTresor());

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

        if (timoTresor && timoTresorCanvasGroup)
            StartCoroutine(FadeOutTimoTresor());

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

        PrepareTimoTresor();

        yield return MoveCamera(bankView);

        currentView = CurrentView.Bank;
        yield return SlideInSignsForCurrentView();

        isMoving = false;
    }

    private IEnumerator EnterSheriffRoom()
    {
        if (!sheriffDoorZoomView || !mainCamera)
            yield break;

        isMoving = true;

        PrepareSheriffSoundButton();

        yield return SlideOutCurrentSigns();
        StartCoroutine(MoveCamera(sheriffDoorZoomView));

        yield return new WaitForSecondsRealtime(sheriffUIFadeDelay);
        yield return FadeCanvasGroup(sheriffRoomUI, 0f, 1f, sheriffUIFadeDuration);

        currentView = CurrentView.SheriffDoor;

        if (rolfRevolver && rolfRevolverCanvasGroup)
            StartCoroutine(FadeInRolfRevolver());

        if (steuerungPanel)
        {
            PrepareSteuerungPanel();
            yield return SlideInSteuerungPanel();
        }

        if (sheriffSoundButton)
            yield return ScaleInSheriffSoundButton();

        yield return new WaitForSecondsRealtime(bankSignDelay);

        if (sheriffBackSign)
        {
            PrepareSign(sheriffBackSign, sheriffBackSignTarget);
            yield return SlideIn(sheriffBackSign, sheriffBackSignTarget);
        }

        isMoving = false;
    }

    private IEnumerator LeaveSheriffRoom()
    {
        if (!sheriffView || !mainCamera)
            yield break;

        isMoving = true;

        if (rolfRevolver && rolfRevolverCanvasGroup)
            StartCoroutine(FadeOutRolfRevolver());

        if (sheriffBackSign)
        {
            yield return SlideOut(sheriffBackSign, sheriffBackSignTarget);
        }

        yield return new WaitForSecondsRealtime(bankSignDelay);

        if (sheriffSoundButton)
        {
            yield return ScaleOutSheriffSoundButton();
        }

        if (steuerungPanel)
        {
            yield return SlideOutSteuerungPanel();
        }

        yield return FadeCanvasGroup(sheriffRoomUI, 1f, 0f, sheriffUIFadeDuration);

        PrepareRolfRevolver();
        PrepareSteuerungPanel();
        PrepareSheriffSoundButton();

        yield return MoveCamera(sheriffView);

        currentView = CurrentView.Sheriff;
        yield return SlideInSignsForCurrentView();

        isMoving = false;
    }

    private IEnumerator EnterSaloonRoom()
    {
        if (!saloonDoorZoomView || !mainCamera)
            yield break;

        isMoving = true;

        yield return SlideOutCurrentSigns();

        StartCoroutine(MoveCamera(saloonDoorZoomView));

        GameResultData.Reset();

        if (ScreenFader.Instance)
            ScreenFader.Instance.FadeToScene("Level01_Scene");

        currentView = CurrentView.SaloonDoor;
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

            case CurrentView.SheriffDoor:
                break;

            case CurrentView.SaloonDoor:
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

            case CurrentView.SheriffDoor:
                break;

            case CurrentView.SaloonDoor:
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

    private void PrepareSteuerungPanel()
    {
        if (!steuerungPanel)
            return;

        steuerungPanel.gameObject.SetActive(false);
        steuerungPanel.anchoredPosition = steuerungPanelTarget + new Vector2(0f, steuerungOffsetY);
    }

    private IEnumerator SlideInSteuerungPanel()
    {
        if (!steuerungPanel)
            yield break;

        steuerungPanel.gameObject.SetActive(true);
        Vector2 startPosition = steuerungPanelTarget + new Vector2(0f, steuerungOffsetY);
        steuerungPanel.anchoredPosition = startPosition;
        float timer = 0f;

        while (timer < signSlideDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(timer / signSlideDuration);
            float easedProgress = 1f - Mathf.Pow(1f - progress, 3f);

            steuerungPanel.anchoredPosition = Vector2.Lerp(startPosition, steuerungPanelTarget, easedProgress);

            yield return null;
        }

        steuerungPanel.anchoredPosition = steuerungPanelTarget;
    }

    private IEnumerator SlideOutSteuerungPanel()
    {
        if (!steuerungPanel)
            yield break;

        Vector2 startPosition = steuerungPanel.anchoredPosition;
        Vector2 endPosition = steuerungPanelTarget + new Vector2(0f, steuerungOffsetY);

        float timer = 0f;

        while (timer < signSlideDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(timer / signSlideDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

            steuerungPanel.anchoredPosition = Vector2.Lerp(startPosition, endPosition, easedProgress);

            yield return null;
        }

        steuerungPanel.anchoredPosition = endPosition;
        steuerungPanel.gameObject.SetActive(false);
    }

    private void PrepareSheriffSoundButton()
    {
        if (!sheriffSoundButton)
            return;

        sheriffSoundButton.localScale = Vector3.zero;
    }

    private IEnumerator ScaleInSheriffSoundButton()
    {
        if (!sheriffSoundButton)
            yield break;

        sheriffSoundButton.localScale = Vector3.zero;

        float timer = 0f;

        while (timer < soundButtonScaleDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(timer / soundButtonScaleDuration);
            float easedProgress = 1f - Mathf.Pow(1f - progress, 3f);

            sheriffSoundButton.localScale = Vector3.Lerp(Vector3.zero, sheriffSoundButtonTargetScale, easedProgress);

            yield return null;
        }

        sheriffSoundButton.localScale = sheriffSoundButtonTargetScale;
    }

    private IEnumerator ScaleOutSheriffSoundButton()
    {
        if (!sheriffSoundButton)
            yield break;

        Vector3 startScale = sheriffSoundButton.localScale;

        float timer = 0f;

        while (timer < soundButtonScaleDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(timer / soundButtonScaleDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

            sheriffSoundButton.localScale = Vector3.Lerp(startScale, Vector3.zero, easedProgress);

            yield return null;
        }

        sheriffSoundButton.localScale = Vector3.zero;
    }

    private void PrepareTimoTresor()
    {
        if (!timoTresor || !timoTresorCanvasGroup)
            return;

        timoTresor.anchoredPosition = timoTresorTarget + new Vector2(timoOffsetX, 0f);
        timoTresorCanvasGroup.alpha = 0f;
        timoTresorCanvasGroup.interactable = false;
        timoTresorCanvasGroup.blocksRaycasts = false;

        timoTresor.gameObject.SetActive(false);
    }

    private IEnumerator FadeInTimoTresor()
    {
        if (!timoTresor || !timoTresorCanvasGroup)
            yield break;

        timoTresor.gameObject.SetActive(true);

        Vector2 startPosition = timoTresorTarget + new Vector2(timoOffsetX, 0f);

        timoTresor.anchoredPosition = startPosition;
        timoTresorCanvasGroup.alpha = 0f;

        float timer = 0f;

        while (timer < timoFadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(timer / timoFadeDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

            timoTresor.anchoredPosition = Vector2.Lerp(startPosition, timoTresorTarget, easedProgress);
            timoTresorCanvasGroup.alpha = Mathf.Lerp(0f, 1f, easedProgress);

            yield return null;
        }

        timoTresor.anchoredPosition = timoTresorTarget;
        timoTresorCanvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOutTimoTresor()
    {
        if (!timoTresor || !timoTresorCanvasGroup)
            yield break;

        Vector2 startPosition = timoTresor.anchoredPosition;
        Vector2 endPosition = timoTresorTarget + new Vector2(timoOffsetX, 0f);

        float startAlpha = timoTresorCanvasGroup.alpha;
        float timer = 0f;

        while (timer < timoFadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(timer / timoFadeDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

            timoTresor.anchoredPosition = Vector2.Lerp(startPosition, endPosition, easedProgress);
            timoTresorCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, easedProgress);

            yield return null;
        }

        timoTresorCanvasGroup.alpha = 0f;
        timoTresor.anchoredPosition = endPosition;
        timoTresor.gameObject.SetActive(false);
    }

    private void PrepareRolfRevolver()
    {
        if (!rolfRevolver || !rolfRevolverCanvasGroup)
            return;

        rolfRevolver.anchoredPosition = rolfRevolverTarget + new Vector2(rolfOffsetX, 0f);
        rolfRevolverCanvasGroup.alpha = 0f;
        rolfRevolverCanvasGroup.interactable = false;
        rolfRevolverCanvasGroup.blocksRaycasts = false;

        rolfRevolver.gameObject.SetActive(false);
    }

    private IEnumerator FadeInRolfRevolver()
    {
        if (!rolfRevolver || !rolfRevolverCanvasGroup)
            yield break;

        rolfRevolver.gameObject.SetActive(true);

        Vector2 startPosition = rolfRevolverTarget + new Vector2(rolfOffsetX, 0f);

        rolfRevolver.anchoredPosition = startPosition;
        rolfRevolverCanvasGroup.alpha = 0f;

        float timer = 0f;

        while (timer < rolfFadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(timer / rolfFadeDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

            rolfRevolver.anchoredPosition = Vector2.Lerp(startPosition, rolfRevolverTarget, easedProgress);
            rolfRevolverCanvasGroup.alpha = Mathf.Lerp(0f, 1f, easedProgress);

            yield return null;
        }

        rolfRevolver.anchoredPosition = rolfRevolverTarget;
        rolfRevolverCanvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOutRolfRevolver()
    {
        if (!rolfRevolver || !rolfRevolverCanvasGroup)
            yield break;

        Vector2 startPosition = rolfRevolver.anchoredPosition;
        Vector2 endPosition = rolfRevolverTarget + new Vector2(rolfOffsetX, 0f);

        float startAlpha = rolfRevolverCanvasGroup.alpha;
        float timer = 0f;

        while (timer < rolfFadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(timer / rolfFadeDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

            rolfRevolver.anchoredPosition = Vector2.Lerp(startPosition, endPosition, easedProgress);
            rolfRevolverCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, easedProgress);

            yield return null;
        }

        rolfRevolverCanvasGroup.alpha = 0f;
        rolfRevolver.anchoredPosition = endPosition;
        rolfRevolver.gameObject.SetActive(false);
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