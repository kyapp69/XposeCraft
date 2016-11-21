﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.Game.Control;

namespace XposeCraft_UI_API_Prototype_Test.Game.Actors.Units
{
	public interface IUnit : IActor
	{
		void MoveTo(Position position);

		void ReplaceActionQueue(UnitActionQueue queue);
	}
}
