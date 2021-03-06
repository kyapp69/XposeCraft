using System;
using System.Collections.Generic;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Game.Actors.Units;
using XposeCraft.Game.Control.GameActions;
using XposeCraft.GameInternal;

namespace XposeCraft.Game.Control
{
    /// <summary>
    /// Queue for <see cref="IGameAction"/> commands which can be assigned to a Unit in order to make it execute them.
    /// </summary>
    public class UnitActionQueue
    {
        /// <summary>
        /// Internal accessor for a dequeue operation over the Action Queue.
        /// </summary>
        internal class ActionDequeue
        {
            public GameAction CurrentAction { get; private set; }

            private IUnit _unit;
            private UnitController _unitController;
            private UnitActionQueue _unitActionQueue;

            internal ActionDequeue(IUnit unit, UnitController unitController, UnitActionQueue queue)
            {
                _unit = unit;
                _unitController = unitController;
                _unitActionQueue = queue;
            }

            public void Finish()
            {
                if (CurrentAction == null)
                {
                    return;
                }
                var previousAction = CurrentAction;
                CurrentAction = null;
                // Player context is needed for all asynchronous actions
                Player.CurrentPlayer = _unitController.PlayerOwner;
                previousAction.OnFinish(_unit, _unitController);
            }

            public void Dequeue()
            {
                Finish();
                if (_unitActionQueue._queue.Count == 0)
                {
                    return;
                }
                CurrentAction = _unitActionQueue._queue.Peek();
                if (CurrentAction == null)
                {
                    Log.e("Null GameAction was enqueued in " + _unitController.name);
                    _unitActionQueue._queue.Dequeue();
                    return;
                }
                // Player context is needed for all asynchronous actions
                Player.CurrentPlayer = _unitController.PlayerOwner;
                try
                {
                    if (!CurrentAction.Progress(_unit, _unitController))
                    {
                        return;
                    }
                    Remove();
                }
                catch (Exception e)
                {
                    if (e is UnitDeadException)
                    {
                        _unitActionQueue._queue.Clear();
                        if (ExceptionOnDeadUnitAction)
                        {
                            throw new UnitDeadException();
                        }
                        return;
                    }
                    // If the progress failed because of an exception, it mustn't block the queue (debugging purposes)
                    Remove();
                    throw;
                }
            }

            private void Remove()
            {
                if (CurrentAction == null)
                {
                    // If already Finished during the Progress, removal is not needed
                    return;
                }
                Log.d(string.Format(
                    "Unit {0} dequeued {1} action", _unitController.name, CurrentAction.GetType().Name));
                _unitActionQueue._queue.Dequeue();
            }
        }

        /// <summary>
        /// If true, attempt at executing an Action with a dead Unit will throw a <see cref="UnitDeadException"/>.
        /// </summary>
        public static bool ExceptionOnDeadUnitAction
        {
            get { return Player.CurrentPlayer.ExceptionOnDeadUnitAction; }
            set { Player.CurrentPlayer.ExceptionOnDeadUnitAction = value; }
        }

        private Queue<GameAction> _queue = new Queue<GameAction>();

        public GameAction CurrentAction
        {
            get { return Dequeue.CurrentAction; }
        }

        public int QueueCount
        {
            get { return _queue.Count + (Dequeue == null ? 0 : Dequeue.CurrentAction != null ? 1 : 0); }
        }

        internal ActionDequeue Dequeue { get; set; }

        public UnitActionQueue()
        {
        }

        public UnitActionQueue(IGameAction action)
        {
            After(action);
        }

        /// <summary>
        /// Enqueues a new Action at the end of the Queue.
        /// </summary>
        /// <param name="action">Action to be acted on by the Queue owner after finishing all previous Actions.</param>
        /// <returns>This queue to add other actions.</returns>
        public UnitActionQueue After(IGameAction action)
        {
            _queue.Enqueue((GameAction) action);
            if (_queue.Count > 1)
            {
                Tutorial.Instance.ActionQueue();
            }
            return this;
        }
    }
}
