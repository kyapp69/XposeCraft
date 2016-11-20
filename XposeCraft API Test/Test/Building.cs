﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.Game.Actors.Units;
using XposeCraft_UI_API_Prototype_Test.Game.Helpers;

/// <summary>
/// Druha faza hry.
/// 
/// Cielom je pomocou pracovnikov vytvarat nove budovy,
/// ktore budu produkovat vojenske jednotky, alebo inak ich vylepsovanim rozsirovat pravdepodobnost vyhry.
/// </summary>
namespace XposeCraft_UI_API_Prototype_Test.Test
{
	public class Building
	{
		public Building()
		{
		}

		/// <summary>
		/// Finds a worker that is just gathering any materials, without any other task
		/// </summary>
		/// <returns>A bored worker</returns>
		private Worker FindWorkerThatGathers()
		{
			foreach (Worker worker in UnitHelper.GetUnits<Worker>())
			{
				if (worker.Gathering != null)
				{
					return worker;
				}
			}
			// First one by default if all have work, a bad workaround
			return UnitHelper.GetUnits<Worker>()[0];
		}

		/*
		public void BuildingStage()
		{
			var firstBuilding = RegisterEvent(Events.MineralsChanged, args => {
				if (args<Minerals>() > 150)
				{
					var basePos = GetBase().Position;
					var position = new Position(basePos.X - 3, basePos.Y - 3);
					if (Space.Available(position))
					{
						worker.CreateBuilding<NubianArmory>(position);
						TryUnits();
						UnregisterEvent(firstBuilding);
					}
				}
			});

			RegisterEvent(Events.StartedBuilding, args => {
				if (args<Building>() == Buildings.NubianComplex)
				{
					var worker = args<Worker>();
					RegisterEvent(Events.FinishedBuilding), args => {
						worker.SendGather();
						BuildArmy();
					}
				}
			});

			var buildingArmy = RegisterEvent(Events.MineralsChanged, args => {
				if (args<Minerals>() > 100)
				{
					foreach (NerubianArmory armory in GetBuildings<NerubianArmory>())
					{
						if (armory.CreateUnit(0)) break;
					}
					if (MyBot.Army++ >= 5)
					{
						UnregisterEvent(buildingArmy);
						AttackPhase();
					}
				}
			});
		}
	*/
	}
}
