using System;
using XposeCraft.Game;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Actors.Units;
using XposeCraft.Game.Enums;
using XposeCraft.Game.Helpers;

namespace XposeCraft.GameInternal.TestExamples
{
    /// <summary>
    /// Prva faza hry.
    ///
    /// Cielom je zbierat suroviny pomocou jednotiek pracovnikov
    /// a pri dostatocnom pocte surovin vytvarat dalsich pracovnikov na zrychlenie ekonomie.
    /// </summary>
    class EconomyTest
    {
        public void EconomyStage(Action startNextStage)
        {
            // Game started, the first worker will get to work
            Worker[] firstWorkers = UnitHelper.GetMyUnits<Worker>();
            foreach (Worker worker in firstWorkers)
            {
                worker.SendGather(ResourceHelper.GetNearestMineralTo(worker));
            }

            EventForCreatingAnother();
            startNextStage();
        }

        void EventForCreatingAnother()
        {
            Event.Register(EventType.MineralsChanged, argsA =>
            {
                if (argsA.Minerals >= 50)
                {
                    // After he collected minerals, another worker will be built
                    var baseCenter = BuildingHelper.GetMyBuildings<BaseCenter>()[0];
                    baseCenter.ProduceUnit(UnitType.Worker);

                    // After creating (it means after few seconds), he will need to go gather too
                    Event.Register(EventType.UnitProduced, argsB =>
                    {
                        if (argsB.MyUnit is Worker)
                        {
                            Worker worker = (Worker) argsB.MyUnit;
                            worker.SendGather(ResourceHelper.GetNearestMineralTo(worker));
                            argsB.ThisEvent.UnregisterEvent();
                        }
                    });
                }

                // This event will work only while there are not enough workers.
                // After that, minerals will be left to go over 150.
                if (UnitHelper.GetMyUnits<Worker>().Length >= 5)
                {
                    argsA.ThisEvent.UnregisterEvent();
                }
            });
        }
    }
}
