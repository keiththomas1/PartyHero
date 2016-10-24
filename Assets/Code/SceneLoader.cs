using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
	[SerializeField]
	private string scene;
	[SerializeField]
	private GameObject percentageTextObject;
	private TextMesh percentageText;

	private AsyncOperation asyncOp;

	void Start() {
		percentageText = percentageTextObject.GetComponent<TextMesh> ();
		if (!percentageText) {
			Debug.Log ("Error grabbing TextMesh handle for progress");
		}
		asyncOp = null;

		StartCoroutine(LoadNewScene());
	}

	// Updates once per frame
	void Update() {
	}


	// The coroutine runs on its own at the same time as Update() and takes an integer indicating which scene to load.
	IEnumerator LoadNewScene() {

		// This line waits for 3 seconds before executing the next line in the coroutine.
		// This line is only necessary for this demo. The scenes are so simple that they load too fast to read the "Loading..." text.
		yield return new WaitForSeconds(1);

		// Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
		AsyncOperation asyncOp = SceneManager.LoadSceneAsync(scene);

		// While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
		while (!asyncOp.isDone) {
			if (percentageText && asyncOp != null) {
				percentageText.text = string.Format("{0:###.##}", (100 * asyncOp.progress)) + "%";
			}
			yield return null;
		}

	}

}