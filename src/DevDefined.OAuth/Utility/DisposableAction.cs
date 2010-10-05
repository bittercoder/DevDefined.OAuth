using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevDefined.OAuth.Utility
{
	public class DisposableAction : IDisposable
	{
		readonly Action _action;

		public DisposableAction(Action action)
		{
			_action = action;
		}

		public void Dispose()
		{
			if (_action != null) _action();
		}
	}
}
