using System;
using System.Diagnostics;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading.Needles;

namespace Theraot.Threading;

[DebuggerNonUserCode]
public sealed class ReentryGuard
{
	private readonly RuntimeUniqueIdProdiver.UniqueId _id;

	private readonly SafeQueue<Action> _workQueue;

	public bool IsTaken => ReentryGuardHelper.IsTaken(_id);

	public ReentryGuard()
	{
		_workQueue = new SafeQueue<Action>();
		_id = RuntimeUniqueIdProdiver.GetNextId();
	}

	public IPromise Execute(Action operation)
	{
		IPromise result = AddExecution(operation, _workQueue);
		ExecutePending(_workQueue, _id);
		return result;
	}

	public IPromise<T> Execute<T>(Func<T> operation)
	{
		IPromise<T> result = AddExecution(operation, _workQueue);
		ExecutePending(_workQueue, _id);
		return result;
	}

	private static IPromise AddExecution(Action action, SafeQueue<Action> queue)
	{
		Promise promised = new Promise(done: false);
		ReadOnlyPromise result = new ReadOnlyPromise(promised, allowWait: false);
		queue.Add(delegate
		{
			try
			{
				action();
				promised.SetCompleted();
			}
			catch (Exception error)
			{
				promised.SetError(error);
			}
		});
		return result;
	}

	private static IPromise<T> AddExecution<T>(Func<T> action, SafeQueue<Action> queue)
	{
		PromiseNeedle<T> promised = new PromiseNeedle<T>(done: false);
		ReadOnlyPromiseNeedle<T> result = new ReadOnlyPromiseNeedle<T>(promised, allowWait: false);
		queue.Add(delegate
		{
			try
			{
				promised.Value = action();
			}
			catch (Exception error)
			{
				promised.SetError(error);
			}
		});
		return result;
	}

	private static void ExecutePending(SafeQueue<Action> queue, RuntimeUniqueIdProdiver.UniqueId id)
	{
		bool flag = false;
		try
		{
			flag = ReentryGuardHelper.Enter(id);
			if (flag)
			{
				Action item;
				while (queue.TryTake(out item))
				{
					item();
				}
			}
		}
		finally
		{
			if (flag)
			{
				ReentryGuardHelper.Leave(id);
			}
		}
	}
}
