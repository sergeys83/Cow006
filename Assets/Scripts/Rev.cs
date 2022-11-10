
using System.Collections;
#if UNITY_ANDROID
using Google.Play.Common;
using Google.Play.Review;
#endif
using UnityEngine.UI;
using UnityEngine;

public class Rev : MonoBehaviour
{
#if UNITY_ANDROID
   private ReviewManager _reviewManager;

    public Button LikeBtn;
   // private PlayReviewInfo _playReviewInfo;
    // Start is called before the first frame update
    void Start()
    {
        _reviewManager = new ReviewManager();
       
    }

    private IEnumerator requireRate(){
        
        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        PlayReviewInfo _playReviewInfo = requestFlowOperation.GetResult();
        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow.
    }
    public void requestRev()
    {
        StartCoroutine(requireRate());
    }
#endif
}
