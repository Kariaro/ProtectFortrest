using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ToggleButtonAttribute : PropertyAttribute {
	public string name;

	public ToggleButtonAttribute(string name) {
		this.name = name;
	}
}
