using System.Collections;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_ANDROID
using Google.Play.Review;
#endif
public class IARManager : MonoBehaviour
{
#if UNITY_ANDROID

        private ReviewManager _reviewManager;
        /* [SerializeField]
         private CanvasGroup msgPanel;
         [SerializeField] 
         private Text message;
         */
        public void RequestReview()
        {
            // StartCoroutine(AndroidReview());
            var reviewManager = new ReviewManager();

            // start preloading the review prompt in the background
            var playReviewInfoAsyncOperation = reviewManager.RequestReviewFlow();

            // define a callback after the preloading is done
            playReviewInfoAsyncOperation.Completed += playReviewInfoAsync =>
            {
                if (playReviewInfoAsync.Error == ReviewErrorCode.NoError)
                {
                    // display the review prompt
                    var playReviewInfo = playReviewInfoAsync.GetResult();
                    reviewManager.LaunchReviewFlow(playReviewInfo);
                    PlayerPrefs.SetInt("review", 1);
                }
                else
                {
                    PlayerPrefs.SetInt("review", 0);
                    // handle error when loading review prompt
                }
            };
        }
        
#endif
}
