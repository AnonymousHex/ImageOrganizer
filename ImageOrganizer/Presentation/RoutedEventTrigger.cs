using System;
using System.Windows;
using System.Windows.Interactivity;
using EventTrigger = System.Windows.Interactivity.EventTrigger;

namespace ImageOrganizer.Presentation
{
	/// <summary>
	/// Wraps event trigger behavior to support routed events.
	/// </summary>
	public class RoutedEventTrigger : EventTrigger
	{
		/// <summary>
		/// 
		/// </summary>
		public RoutedEvent RoutedEvent { get; set; }

		/// <summary>
		/// 
		/// </summary>
		protected override void OnAttached()
		{
			var behavior = AssociatedObject as Behavior;
			var associatedElement = AssociatedObject as FrameworkElement;

			if (behavior != null)
				associatedElement = ((IAttachedObject)behavior).AssociatedObject as FrameworkElement;

			if (associatedElement == null)
				throw new ArgumentException("RoutedEventTrigger can only be associated to framework elements.");

			if (RoutedEvent != null)
				associatedElement.AddHandler(RoutedEvent, new RoutedEventHandler(OnRoutedEvent));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		void OnRoutedEvent(object sender, RoutedEventArgs args)
		{
			OnEvent(args);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override string GetEventName()
		{
			return RoutedEvent.Name;
		}
	}
}
