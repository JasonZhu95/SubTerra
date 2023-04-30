using UnityEngine;

namespace Game2DWaterKit
{
	public class GameController : MonoBehaviour
	{
		Rigidbody2D pickedGameObject;
		public float MoveSpeed = 0f;

		LayerMask raycastMask = -1 & ~( 1 << 4 );

		void Update ()
		{
			Camera mainCam = Camera.main;
			Vector2 mousePos = mainCam.ScreenToWorldPoint ( Input.mousePosition );
	
			if ( Input.GetMouseButtonDown ( 0 ) ) {
				RaycastHit2D hit = Physics2D.Raycast ( mousePos, mainCam.transform.forward, float.MaxValue, raycastMask );
				if ( hit.rigidbody != null && hit.rigidbody.CompareTag ( "Pickable" ) ) {
					pickedGameObject = hit.rigidbody;
					pickedGameObject.isKinematic = true;
				}
			}
	
			if ( pickedGameObject && Input.GetMouseButtonUp ( 0 ) ) {
				pickedGameObject.isKinematic = false;
				pickedGameObject = null;
			}
	
			if ( pickedGameObject ) {
				pickedGameObject.MovePosition ( mousePos );
			}

			if ( MoveSpeed > 0f ) {
				Vector3 camPos = mainCam.transform.position;
				float moveSpeed = Time.deltaTime * MoveSpeed;
				camPos.x += Input.GetAxis ( "Horizontal" ) * moveSpeed;
				camPos.y += Input.GetAxis ( "Vertical" ) * moveSpeed;
				mainCam.transform.position = camPos;
			}
		}
	}
}
