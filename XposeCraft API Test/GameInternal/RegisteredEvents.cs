﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.Game;
using XposeCraft_UI_API_Prototype_Test.Game.Enums;

namespace XposeCraft_UI_API_Prototype_Test.GameInternal
{
	/// <summary>
	/// Actions hooked to events that can, but don't have to be, run at any time.
	/// </summary>
	class RegisteredEvents
	{
		public static IDictionary<EventType, IList<Event>> Registered = new Dictionary<EventType, IList<Event>>();

		// TODO: static constructor (for now in tester)
		public static void Initialize()
		{
			Registered.Clear();
			foreach (EventType gameEvent in Enum.GetValues(typeof(EventType)))
			{
				Registered.Add(gameEvent, new List<Event>());
			}
		}

		public static readonly object FiredEventLock = new object();
		public static void FiredEvent(EventType eventType, Arguments args)
		{
			lock (FiredEventLock)
			{
				foreach (Event registeredEvent in Registered[eventType])
				{
					registeredEvent.Function(args);
				}
			}
		}
	}
}
