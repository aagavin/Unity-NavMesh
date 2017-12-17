using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class RayCaster : MonoBehaviour {
	public GameObject targetGO;
	private UnityEngine.AI.NavMeshAgent[] navMeshAgents;
	private Ray ray;
	private bool M_pressed = false;
	private Camera cam;
	private Dictionary<int, Color> dOrigColors;
	private UnityEngine.AI.NavMeshAgent closest;


	public void Awake(){

		navMeshAgents = FindObjectsOfType<UnityEngine.AI.NavMeshAgent>();
		closest = _GetClosest (targetGO.transform.position);
		print (navMeshAgents);
		print ("closest:"+closest);
		dOrigColors = new Dictionary<int,Color> ();
		for (int i = 0; i < navMeshAgents.Length; i++) {
			dOrigColors [navMeshAgents [i].GetHashCode ()] = navMeshAgents [i].GetComponent<Renderer> ().material.color;
		}
		cam = this.GetComponentInParent (typeof(Camera)) as Camera;


	}
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		bool lMouse = Input.GetMouseButtonDown (0);
		bool rMouse = Input.GetMouseButtonDown (1);

		if (lMouse || rMouse) {
			Vector3 mousePos = Input.mousePosition;
			ray = cam.ScreenPointToRay (mousePos);
			RaycastHit hit;
			if(Physics.Raycast(ray.origin,ray.direction, out hit)){
				targetGO.transform.position = hit.point;
				if (lMouse) {
					closest = _GetClosest (hit.point);
					Highlight (closest.tag);
				} else {
					SendNPCsToDestination ();
				}
			}
		}
	}


	private UnityEngine.AI.NavMeshAgent _GetClosest(Vector3 toPos){
		UnityEngine.AI.NavMeshAgent closeNav = navMeshAgents [0];
		Vector3 pos = closeNav.transform.position;
		float minDist = Vector3.Distance (toPos, pos);
		for (int i = 1; i < navMeshAgents.Length; i++) {
			pos = navMeshAgents [i].transform.position;
			if (Vector3.Distance (toPos, pos) < minDist) {
				minDist=Vector3.Distance (toPos, pos);
				closeNav = navMeshAgents[i];
			}
		}

		return closeNav;
	}


	void OnGUI(){
		int y = 10, h = 5;
		GUI.Label (new Rect (10, y, 200, 20), "LMB -> Select Unit"); y += 20 + h;
		GUI.Label (new Rect (10, y, 200, 20), "RMB -> Move Unit");y += 20 + h;

	}
	void Highlight(string tag){
		for (int i = 0; i < navMeshAgents.Length; i++) {
			navMeshAgents[i].GetComponent<Renderer> ().material.color = dOrigColors[navMeshAgents[i].GetHashCode()];
		}

		if (tag != "") {
			GameObject[] NPCs = GameObject.FindGameObjectsWithTag (tag);
			for (int i = 0; i < NPCs.Length; i++) {
				NPCs [i].GetComponent<Renderer> ().material.color = Color.yellow;
			}
		} 


			
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.yellow;
		if (closest != null) {
			GameObject[] NPCs = GameObject.FindGameObjectsWithTag (closest.tag);
			for (int i = 0; i < NPCs.Length; i++) {
				Gizmos.DrawWireSphere (NPCs [i].transform.position, 1.1f);
			}
		}
	}

	void SendNPCsToDestination (){
		foreach (var nma in navMeshAgents) {
			if (nma.tag == closest.tag)
				nma.SetDestination (targetGO.transform.position);
		}
	}
   

}
