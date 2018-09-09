using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CameraUiResponsivity : MonoBehaviour {
	[SerializeField] CameraFollow cameraFollow;
	[SerializeField] RectTransform settlementPanel;

	public Vector2 offset;
	public Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
	public float screenScale;

	void OnEnable() {
	}

	void Update() {
		//xOffset = 0f;//uiPanelOffsets.Sum();
		//yOffset = 0;

		//xOffset = settlementPanel.anchoredPosition.x / Screen.width
		//xOffset = Screen.width / 2f;
		offset.x = center.x + settlementPanel.anchoredPosition.x;
		offset.y = center.y;
		cameraFollow.Offset = (center - offset) * screenScale;
		//Camera.main.ScreenToWorldPoint(new Vector2(xOffset, yOffset));
	}

}
