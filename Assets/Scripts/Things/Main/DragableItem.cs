using UnityEngine;
using System.Collections;

public sealed class DragableItemEventType : InteractiveThingEventType {
	public static readonly DragableItemEventType OnDragStart = new DragableItemEventType("Visitor_StartsDragging_");
	public static readonly DragableItemEventType OnDragEnd = new DragableItemEventType("Visitor_StopsDragging_");
	
	private DragableItemEventType(string value) : base(value) {}
}

[RequireComponent(typeof(Rigidbody))]
public class DragableItem : InteractiveThing {

	public virtual void OnDragStarts(object sender) { }

	public virtual void OnDragEnds(object sender) { }

	protected override void OnEnable () {
		base.OnEnable();

		On(DragableItemEventType.OnDragStart, OnDragStarts);
		On(DragableItemEventType.OnDragEnd, OnDragEnds);
	}
}
