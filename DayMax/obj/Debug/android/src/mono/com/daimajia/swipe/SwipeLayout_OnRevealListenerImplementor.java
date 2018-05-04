package mono.com.daimajia.swipe;


public class SwipeLayout_OnRevealListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.daimajia.swipe.SwipeLayout.OnRevealListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onReveal:(Landroid/view/View;Lcom/daimajia/swipe/SwipeLayout$DragEdge;FI)V:GetOnReveal_Landroid_view_View_Lcom_daimajia_swipe_SwipeLayout_DragEdge_FIHandler:AndroidSwipeLayout.SwipeLayout/IOnRevealListenerInvoker, AndroidSwipeLayout\n" +
			"";
		mono.android.Runtime.register ("AndroidSwipeLayout.SwipeLayout+IOnRevealListenerImplementor, AndroidSwipeLayout, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null", SwipeLayout_OnRevealListenerImplementor.class, __md_methods);
	}


	public SwipeLayout_OnRevealListenerImplementor ()
	{
		super ();
		if (getClass () == SwipeLayout_OnRevealListenerImplementor.class)
			mono.android.TypeManager.Activate ("AndroidSwipeLayout.SwipeLayout+IOnRevealListenerImplementor, AndroidSwipeLayout, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onReveal (android.view.View p0, com.daimajia.swipe.SwipeLayout.DragEdge p1, float p2, int p3)
	{
		n_onReveal (p0, p1, p2, p3);
	}

	private native void n_onReveal (android.view.View p0, com.daimajia.swipe.SwipeLayout.DragEdge p1, float p2, int p3);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
