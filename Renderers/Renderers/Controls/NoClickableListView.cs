using System;
using Xamarin.Forms;

namespace Renderers
{
	public class NoClickableListView : ListView
	{
		public NoClickableListView()
		{
			Effects.Add(Effect.Resolve("AwesomeEffects.NoClickableListViewEffect"));
		}
	}
}
