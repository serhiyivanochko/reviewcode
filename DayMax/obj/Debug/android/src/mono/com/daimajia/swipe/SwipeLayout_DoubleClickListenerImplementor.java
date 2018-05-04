package mono.com.daimajia.swipe;


public class SwipeLayout_DoubleClickListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.daimajia.swipe.SwipeLayout.DoubleClickListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onDoubleClick:(Lcom/daimajia/swipe/SwipeLayout;Z)V:GetOnDoubleClick_Lcom_daimajia_swipe_SwipeLayout_ZHandler:AndroidSwipeLayout.SwipeLayout/IDoubleClickListenerInvoker, AndroidSwipeLayout\n" +
			"";
		mono.android.Runtime.register ("AndroidSwipeLayout.SwipeLayout+IDoubleClickListenerImplementor, AndroidSwipeLayout, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null", SwipeLayout_DoubleClickListenerImplementor.class, __md_methods);
	}


	public SwipeLayout_DoubleClickListenerImplementor ()
	{
		super ();
		if (getClass () == SwipeLayout_DoubleClickListenerImplementor.class)
			mono.android.TypeManager.Activate ("AndroidSwipeLayout.SwipeLayout+IDoubleClickListenerImplementor, AndroidSwipeLayout, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onDoubleClick (com.daimajia.swipe.SwipeLayout p0, boolean p1)
	{
		n_onDoubleClick (p0, p1);
	}

	private native void n_onDoubleClick (com.daimajia.swipe.SwipeLayout p0, boolean p1);

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
