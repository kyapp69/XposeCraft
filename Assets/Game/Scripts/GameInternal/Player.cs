using System;
using System.Collections.Generic;
using UnityEngine;
using XposeCraft.Collections;
using XposeCraft.Core.Faction;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Game;
using XposeCraft.Game.Actors;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Actors.Resources;
using XposeCraft.Game.Actors.Units;
using XposeCraft.Game.Control.GameActions;
using XposeCraft.Game.Enums;
using VisionState = XposeCraft.Core.Fog_Of_War.VisionReceiver.VisionState;

namespace XposeCraft.GameInternal
{
    /// <summary>
    /// Data structures that make up the Model of a Player used within a game.
    /// It is a good idea not to persist it as a prefab, as the instance will usually hold references to Scene objects.
    /// </summary>
    public class Player : MonoBehaviour
    {
        /// <summary>
        /// True if already lost.
        /// </summary>
        public bool LostGame { get; set; }

        /// <summary>
        /// Reason of losing.
        /// </summary>
        public enum LoseReason
        {
            ExceptionThrown,
            AllBuildingsDestroyed,
            TimeoutStalemate
        }

        // Unity hot-swap serialization workarounds.

        [Serializable]
        public class EventList : List<GameEvent>
        {
        }

        [Serializable]
        public class RegisteredEventsDictionary : SerializableDictionary2<GameEventType, EventList>
        {
        }

        /// <summary>
        /// Curently executing Player to be used as a Model.
        /// Needs to be replaced before executing any Event.
        /// </summary>
        public static Player CurrentPlayer;

        /// <summary>
        /// Faction of the Player, allied Players will share it.
        /// </summary>
        public Faction Faction;

        public int FactionIndex;

        /// <summary>
        /// This Player's Base.
        /// </summary>
        public PlaceType MyBase;

        /// <summary>
        /// This Player's Enemy's Base.
        /// </summary>
        public PlaceType EnemyBase;

        /// <summary>
        /// Actions hooked to Events that can, but don't have to be, run at any time.
        /// </summary>
        public RegisteredEventsDictionary RegisteredEvents;

        // In-game Actors available to the Player.

        public List<Unit> Units;
        public List<Building> Buildings;

        /// <summary>
        /// Only the first Player's (index 0) Resources are being used!
        /// </summary>
        public List<Resource> SharedResources;

        public List<Unit> EnemyVisibleUnits;
        public List<Building> EnemyVisibleBuildings;

        // Run-time configuration of the Player

        private bool _exceptionOnDeadUnitAction;

        public bool ExceptionOnDeadUnitAction
        {
            get { return _exceptionOnDeadUnitAction; }
            set { _exceptionOnDeadUnitAction = value; }
        }

        public void EnemyOnSight(
            GameObject enemyActorGameObject,
            Actor enemyActor,
            List<GameObject> myActorGameObjectsSaw,
            List<Actor> myActorsSaw,
            VisionState previousState,
            VisionState newState)
        {
            if (newState == VisionState.Vision)
            {
                EvaluateAttackMove(enemyActor, myActorGameObjectsSaw, myActorsSaw);
            }
            // Evaluating visibility event if a change occurred
            if (previousState != newState)
            {
                EnemyVisibilityChanged(enemyActor, myActorsSaw, previousState, newState);
            }
        }

        private static void EvaluateAttackMove(
            Actor enemyActor,
            List<GameObject> myActorGameObjectsSaw,
            List<Actor> myActorsSaw)
        {
            // Evaluating Attack Move
            for (var myActorIndex = 0; myActorIndex < myActorsSaw.Count; myActorIndex++)
            {
                var myActorSaw = myActorsSaw[myActorIndex];
                // Only Units can respond to seeing an enemy by Attack Move
                if (!(myActorSaw is IUnit))
                {
                    continue;
                }
                // TODO: optimize performance by not requesting the component every time
                var myUnitController = myActorGameObjectsSaw[myActorIndex].GetComponent<UnitController>();
                if (!myUnitController.IsAttackMove || myUnitController.AttackMoveTarget != null)
                {
                    continue;
                }
                // A new target causes Attack to be performed before continuing the queue
                Attack.AttackUnitOrBuilding(enemyActor, myUnitController);
                myUnitController.AttackMoveTarget = enemyActor;
            }
        }

        private void EnemyVisibilityChanged(
            Actor enemyActor, List<Actor> myActorsSawChange, VisionState previousState, VisionState newState)
        {
            if (previousState == VisionState.Vision && newState != VisionState.Vision)
            {
                EnemyHidden(enemyActor, newState);
            }
            else if (previousState == VisionState.Undiscovered || previousState == VisionState.Discovered)
            {
                EnemyTurnedToVisible(enemyActor, myActorsSawChange);
            }
        }

        private void EnemyTurnedToVisible(Actor enemyActor, List<Actor> myActorsSaw)
        {
            // Only the first detection matters, as it is about visibility state "change"
            var myActor = myActorsSaw.Count == 0 ? null : myActorsSaw[0];
            var enemyUnit = enemyActor as IUnit;
            if (enemyUnit != null)
            {
                GameManager.Instance.FiredEvent(
                    this,
                    GameEventType.EnemyUnitsOnSight,
                    new Arguments
                    {
                        MyUnit = myActor as IUnit,
                        MyBuilding = myActor as IBuilding,
                        EnemyUnits = new[] {enemyUnit}
                    });
                EnemyVisibleUnits.Add((Unit) enemyUnit);
                return;
            }
            var enemyBuilding = enemyActor as IBuilding;
            if (enemyBuilding != null)
            {
                GameManager.Instance.FiredEvent(
                    this,
                    GameEventType.EnemyBuildingsOnSight,
                    new Arguments
                    {
                        MyUnit = myActor as IUnit,
                        MyBuilding = myActor as IBuilding,
                        EnemyBuildings = new[] {enemyBuilding}
                    });
                EnemyVisibleBuildings.Add((Building) enemyBuilding);
            }
        }

        private void EnemyHidden(Actor enemyActor, VisionState newState)
        {
            var unit = enemyActor as IUnit;
            if (unit != null)
            {
                EnemyVisibleUnits.Remove((Unit) unit);
                return;
            }
            var building = enemyActor as IBuilding;
            if (building != null && newState == VisionState.Undiscovered)
            {
                EnemyVisibleBuildings.Remove((Building) building);
            }
        }

        public void Win()
        {
            if (this == GameManager.Instance.GuiPlayer)
            {
                GameTestRunner.Passed = true;
            }
            Log.i(string.Format("Player {0} won the game", name));
        }

        public void Lost(LoseReason loseReason)
        {
            if (LostGame)
            {
                return;
            }
            LostGame = true;
            switch (loseReason)
            {
                case LoseReason.ExceptionThrown:
                    Log.e(string.Format("Player {0} lost because an exception was thrown", name));
                    break;
                case LoseReason.AllBuildingsDestroyed:
                    Log.e(string.Format("Player {0} lost because all his buildings were destroyed", name));
                    break;
                case LoseReason.TimeoutStalemate:
                    Log.e(string.Format("Stalemate. Player {0} lost because of a game timeout", name));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("loseReason", loseReason, null);
            }
            if (this == GameManager.Instance.GuiPlayer)
            {
                GameTestRunner.Failed = true;
            }
            // This causes enemies to Win if they don't have any more enemies
            if (loseReason == LoseReason.TimeoutStalemate)
            {
                return;
            }
            foreach (var enemyFactionIndex in Faction.EnemyFactionIndexes())
            {
                foreach (var player in GameManager.Instance.Players)
                {
                    if (this != player && player.FactionIndex == enemyFactionIndex)
                    {
                        player.Win();
                    }
                }
            }
        }

        public void Lost(Exception exception)
        {
            Log.e(exception);
            Lost(LoseReason.ExceptionThrown);
        }
    }
}
