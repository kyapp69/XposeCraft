﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XposeCraft_UI_API_Prototype_Test.Game.Actors.Buildings
{
	public interface IBuilding: IActor
	{
		bool IsFinished { get; set; }
	}
}
