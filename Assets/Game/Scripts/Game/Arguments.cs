using System;
using System.Collections.Generic;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Actors.Units;
using Object = UnityEngine.Object;

namespace XposeCraft.Game
{
    /// <summary>
    /// Description of a Game Event occurrence.
    /// </summary>
    [Serializable]
    public class Arguments : Object
    {
        /// <summary>
        /// Before running the function, the instance to this current GameEvent will be returned here and can be used,
        /// for example, to unregister it after the run.
        /// </summary>
        public GameEvent ThisGameEvent { get; set; }

        // TODO: replace
        public IDictionary<string, string> StringMap { get; private set; }

        // Game resources

        /// <summary>
        /// Minerals available to the Player.
        /// </summary>
        public int Minerals { get; set; }

        // Actors

        /// <summary>
        /// My Unit related to the Event.
        /// </summary>
        public IUnit MyUnit { get; set; }

        /// <summary>
        /// My Building related to the Event.
        /// </summary>
        public IBuilding MyBuilding { get; set; }

        /// <summary>
        /// Enemy Units related to the Event.
        /// </summary>
        public IUnit[] EnemyUnits { get; set; }

        /// <summary>
        /// Enemy Buildings related to the Event.
        /// </summary>
        public IBuilding[] EnemyBuildings { get; set; }

        public Arguments()
        {
            StringMap = new Dictionary<string, string>();
        }

        /// <summary>
        /// Clone constructor.
        /// TODO: do a deep clone.
        /// </summary>
        /// <param name="arguments">Arguments to have its parameters copied to the new instance.</param>
        /// <param name="thisGameEvent">GameEvent represented by the Arguments instance.</param>
        public Arguments(Arguments arguments, GameEvent thisGameEvent)
        {
            ThisGameEvent = thisGameEvent;
            // TODO: clone
            StringMap = arguments.StringMap;
            Minerals = arguments.Minerals;
            MyUnit = arguments.MyUnit;
            MyBuilding = arguments.MyBuilding;
            // TODO: clone
            EnemyUnits = arguments.EnemyUnits;
            // TODO: clone
            EnemyBuildings = arguments.EnemyBuildings;
        }
    }
}
