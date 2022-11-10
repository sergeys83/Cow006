using UnityEngine;
using TMPro;
namespace Lean.Localization
{
	/// <summary>This component will update a TextMesh component's Font with a localized font, or use a fallback if none is found.</summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(TextMeshPro))]
	[AddComponentMenu(LeanLocalization.ComponentPathPrefix + "Localized TextMesh Font")]
	public class LeanLocalizedTextMeshFont : LeanLocalizedBehaviour
	{
		[Tooltip("If PhraseName couldn't be found, this font asset will be used")]
		public Font FallbackFont;
        public TMP_FontAsset fontAsset;

		// This gets called every time the translation needs updating
		public override void UpdateTranslation(LeanTranslation translation)
		{
			// Get the TextMesh component attached to this GameObject
			var text = GetComponent<TextMeshProUGUI>();

			// Use translation?
			if (translation != null && translation.Data is TMP_FontAsset)
			{
                text.font = (TMP_FontAsset)translation.Data;

               // text.font = (Font)translation.Data;
			}
			// Use fallback?
			else
			{
                text.font = fontAsset;
               // text.font = FallbackFont;
			}
		}

		protected virtual void Awake()
		{
			// Should we set FallbackFont?
		
            if (fontAsset == null)
            {
                // Get the TextMesh component attached to this GameObject
                var text = GetComponent<TextMeshProUGUI>();

                // Copy current text to fallback
                fontAsset = text.font;
                
            }
        }
	}
}