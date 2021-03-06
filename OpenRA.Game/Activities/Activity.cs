#region Copyright & License Information
/*
 * Copyright 2007-2016 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */
#endregion

using System.Collections.Generic;
using System.Linq;
using OpenRA.Traits;

namespace OpenRA.Activities
{
	public abstract class Activity
	{
		public Activity NextActivity { get; set; }
		public bool IsInterruptible { get; protected set; }
		protected bool IsCanceled { get; private set; }

		public Activity()
		{
			IsInterruptible = true;
		}

		public abstract Activity Tick(Actor self);

		public virtual bool Cancel(Actor self)
		{
			if (!IsInterruptible)
				return false;

			IsCanceled = true;
			NextActivity = null;

			return true;
		}

		public virtual void Queue(Activity activity)
		{
			if (NextActivity != null)
				NextActivity.Queue(activity);
			else
				NextActivity = activity;
		}

		public virtual IEnumerable<Target> GetTargets(Actor self)
		{
			yield break;
		}
	}

	public static class ActivityExts
	{
		public static IEnumerable<Target> GetTargetQueue(this Actor self)
		{
			return self.CurrentActivity
				.Iterate(u => u.NextActivity)
				.TakeWhile(u => u != null)
				.SelectMany(u => u.GetTargets(self));
		}
	}
}
