using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Tuning : ScriptableObject {
	[SerializeField]
	public System.Type NavigationInitialState;
	public string version = "1.0";
	public ContactFilter2D filter;
	public int maxContacts;
	public float recordLength;

	[Header("Colors")]
	public Color GravityColor;
	public Color DragColor;
	public Color FrictionColor;
	public Color NormalColor;
}
