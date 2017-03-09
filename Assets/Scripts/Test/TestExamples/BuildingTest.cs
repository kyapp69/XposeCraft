﻿using System;
using XposeCraft.Game;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Actors.Units;
using XposeCraft.Game.Enums;
using XposeCraft.Game.Helpers;

namespace XposeCraft.Test.TestExamples
{
    /// <summary>
    /// Druha faza hry.
    ///
    /// Cielom je pomocou pracovnikov vytvarat nove budovy,
    /// ktore budu produkovat vojenske jednotky, alebo inak ich vylepsovanim rozsirovat pravdepodobnost vyhry.
    /// </summary>
    class BuildingTest
    {
        public BuildingTest()
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

        public void BuildingStage(Action startNextStage)
        {
            // A first building
            Event.Register(EventType.MineralsChanged, args =>
            {
                if (args.Minerals > 150)
                {
                    var baseCenter = BuildingHelper.GetBuildings<BaseCenter>()[0];
                    var position = BuildingHelper.ClosestEmptySpaceTo(baseCenter);
                    FindWorkerThatGathers().CreateBuilding(Game.Enums.BuildingType.NubianArmory, position);

                    // We only need one army production building for now
                    args.ThisEvent.UnregisterEvent();
                }
            });

            // Worker will return to work afterwards
            Event.Register(EventType.BuildingCreated, args =>
            {
                if (
                    args.MyBuilding.GetType().Equals(typeof(NubianArmory))
                    &&
                    args.MyUnit.GetType().Equals(typeof(Worker))
                )
                {
                    var worker = (Worker) args.MyUnit;
                    worker.SendGather(MaterialHelper.GetNearestMineralsTo(worker));
                    BuildArmy(startNextStage);
                }
                args.ThisEvent.UnregisterEvent();
            });
        }

        void BuildArmy(Action startNextStage)
        {
            Event.Register(EventType.MineralsChanged, args =>
            {
                if (args.Minerals > 100)
                {
                    foreach (NubianArmory armory in BuildingHelper.GetBuildings<NubianArmory>())
                    {
                        if (armory.CreateUnit(Game.Enums.UnitType.DonkeyGun))
                        {
                            if (MyBot.Army++ >= 5)
                            {
                                args.ThisEvent.UnregisterEvent();
                                startNextStage();
                            }
                            break;
                        }
                    }
                }
            });
        }
    }
}
