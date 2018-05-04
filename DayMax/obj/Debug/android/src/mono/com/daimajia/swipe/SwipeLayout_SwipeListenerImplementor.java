package mono.com.daimajia.swipe;


public class SwipeLayout_SwipeListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.daimajia.swipe.SwipeLayout.SwipeListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onClose:(Lcom/daimajia/swipe/SwipeLayout;)V:GetOnClosed_Lcom_daimajia_swipe_SwipeLayout_Handler:AndroidSwipeLayout.SwipeLayout/ISwipeListenerInvoker, AndroidSwipeLayout\n" +
			"n_onHandRelease:(Lcom/daimajia/swipe/SwipeLayout;FF)V:GetOnHandRelease_Lcom_daimajia_swipe_SwipeLayout_FFHandler:AndroidSwipeLayout.SwipeLayout/ISwipeListenerInvoker, AndroidSwipeLayout\n" +
			"n_onOpen:(Lcom/daimajia/swipe/SwipeLayout;)V:GetOnOpened_Lcom_daimajia_swipe_SwipeLayout_Handler:AndroidSwipeLayout.SwipeLayout/ISwipeListenerInvoker, AndroidSwipeLayout\n" +
			"n_onStartClose:(Lcom/daimajia/swipe/SwipeLayout;)V:GetOnClosing_Lcom_daimajia_swipe_SwipeLayout_Handler:AndroidSwipeLayout.SwipeLayout/ISwipeListenerInvoker, AndroidSwipeLayout\n" +
			"n_onStartOpen:(Lcom/daimajia/swipe/SwipeLayout;)V:GetOnOpening_Lcom_daimajia_swipe_SwipeLayout_Handler:AndroidSwipeLayout.SwipeLayout/ISwipeListenerInvoker, AndroidSwipeLayout\n" +
			"n_onUpdate:(Lcom/daimajia/swipe/SwipeLayout;II)V:GetOnUpdate_Lcom_daimajia_swipe_SwipeLayout_IIHandler:AndroidSwipeLayout.SwipeLayout/ISwipeListenerInvoker, AndroidSwipeLayout\n" +
			"";
		mono.android.Runtime.register ("AndroidSwipeLayout.SwipeLayout+ISwipeListenerImplementor, AndroidSwipeLayout, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null", SwipeLayout_SwipeListenerImplementor.class, __md_methods);
	}


	public SwipeLayout_SwipeListenerImplementor ()
	{
		super ();
		if (getClass () == SwipeLayout_SwipeListenerImplementor.class)
			mono.android.TypeManager.Activate ("AndroidSwipeLayout.SwipeLayout+ISwipeListenerImplementor, AndroidSwipeLayout, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onClose (com.daimajia.swipe.SwipeLayout p0)
	{
		n_onClose (p0);
	}

	private native void n_onClose (com.daimajia.swipe.SwipeLayout p0);


	public void onHandRelease (com.daimajia.swipe.SwipeLayout p0, float p1, float p2)
	{
		n_onHandRelease (p0, p1, p2);
	}

	private native void n_onHandRelease (com.daimajia.swipe.SwipeLayout p0, float p1, float p2);


	public void onOpen (com.daimajia.swipe.SwipeLayout p0)
	{
		n_onOpen (p0);
	}

	private native void n_onOpen (com.daimajia.swipe.SwipeLayout p0);


	public void onStartClose (com.daimajia.swipe.SwipeLayout p0)
	{
		n_onStartClose (p0);
	}

	private native void n_onStartClose (com.daimajia.swipe.SwipeLayout p0);


	public void onStartOpen (com.daimajia.swipe.SwipeLayout p0)
	{
		n_onStartOpen (p0);
	}

	private native void n_onStartOpen (com.daimajia.swipe.SwipeLayout p0);


	public void onUpdate (com.daimajia.swipe.SwipeLayout p0, int p1, int p2)
	{
		n_onUpdate (p0, p1, p2);
	}

	private native void n_onUpdate (com.daimajia.swipe.SwipeLayout p0, int p1, int p2);

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
