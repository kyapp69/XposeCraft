﻿using System;
using System.Collections.Generic;
using XposeCraft.App.TestRunner;

namespace XposeCraft.Test
{
	/// <summary>
	/// Tretia faza hry.
	///
	/// Cielom je pouzitim postavenych jednotiek znicit nepriatela,
	/// pripadne pocas boja stavat dalsie jednotky a rozsirovat svoju zakladnu.
	/// </summary>
	class BattleTest
	{
		public BattleTest()
		{
		}

		public void BattleStage(Runner.NextStageStarter startNextStage)
		{
			startNextStage();
		}
	}
}
