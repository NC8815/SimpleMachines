using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public static class ComponentExtensions{

	public static T GetCopy<T>(this Component comp, T other) where T : Component, new(){
		Type type = comp.GetType ();
		T result = new T();
		BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
		PropertyInfo[] pinfos = type.GetProperties (flags);
		foreach (var pinfo in pinfos) {
			if (pinfo.CanWrite){
				try{
					pinfo.SetValue (comp, pinfo.GetValue (result, null), null);
				}
				catch{
				}
			}
		}
		FieldInfo[] finfos = type.GetFields (flags);
		foreach (var finfo in finfos) {
			finfo.SetValue (comp, finfo.GetValue (result));
		}
		return result;
	}
}
