package com.kbitubit.android.demo;

//UniDialog implementation
//Copyright 2011 by Victor Derevyanko, dvpublic0@gmail.com
//http://derevyanko.blogspot.com/2011/12/android.html
//http://code.google.com/p/dvsrc/downloads/detail?name=20111217DemoSelectDialog.7z&can=2&q=#makechanges


import java.util.List;

import com.kbitubit.android.demo.R;
import com.kbitubit.android.demo.UniDialogUtils.ViewHolder;

import android.app.Activity;
import android.app.Dialog;
import android.content.DialogInterface;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.ListView;

/** Dialog to select single item from list of items.
 * T is type of items, T should implement toString() correctly - it is used for displaying items on screen.
 * It shows dialog and returns resulted item or null
 * Resource files:  dialog_single_selector_row and dialog_single_selector
 * are required.  */
public final class UniDialogSingleSelector {
	
	/** Shows dialog and suggests to select single item from list of items.
	 * Returns selected item or null, if user canceled dialog 
	 * bShowImages = true means, that T supports ItemImageProvider interface
	 * and dialog should display item image at the left side of item text.
	 * Returns INotification - you can use it to send notifications about 
	 * updating list view. In practice, you can often use SimplestDataItem as T */
	@SuppressWarnings("unchecked")
	public static <T> UniDialogUtils.INotification<Void> showDialog(Activity activity
			, UniDialogUtils.DialogConfig dialogConfig
			, List<T> listItems
			, SingleItemReceiver<T> resultReceiver) 
	{
		Dialog dialog = new Dialog(activity);

		dialog.setContentView(R.layout.unidialog_selector);
		dialog.setTitle(dialogConfig.DialogTitle); 
	
		dialog.setOwnerActivity(activity);
		
		ListView list_view = (ListView) dialog.findViewById(R.id.unidialog_listview);
		dialog.findViewById(R.id.unidialog_layout_ok_cancel).setVisibility(View.GONE); //we don't need ok/cancel buttons in single selection dialog
		
		list_view.setAdapter(new SingleAdapter<T>(activity, listItems, resultReceiver, dialog, dialogConfig) );			
		dialog.setOnCancelListener(new SingleCancelListener<T>(resultReceiver));			
		dialog.show();
		
		return (SingleAdapter<T>)list_view.getAdapter();
	}
	
	/** listener: gets item selected in the dialog OR null if user has canceled the dialog	 */
	public static interface SingleItemReceiver<T> {
		void onSelectItem(T selectedItem); 
	}
	
	/** * Handler for cancel. Returns null to receiver. */
	private static class SingleCancelListener<T> implements DialogInterface.OnCancelListener {
		private final SingleItemReceiver<T> m_ResultsReceiver;
		public SingleCancelListener(SingleItemReceiver<T> resultReceiver) {
			m_ResultsReceiver = resultReceiver;
		}
		@Override
		public void onCancel(DialogInterface arg0) {
			if (m_ResultsReceiver != null) { //user canceled dialog
				m_ResultsReceiver.onSelectItem(null);
			}
		} 
	}

	/** ListView adapter for List of T objects.
	 * Each T-object should correctly implement toString() function
	 * results of toString() are shown in the list. */
	private static class SingleAdapter<T> extends ArrayAdapter<T>  implements UniDialogUtils.INotification<Void> {
		private final SingleItemReceiver<T> _ResultsReceiver;
		private final Dialog _Dialog;
		private LayoutInflater _Inflater;
		private final List<T> _ListItems;
		private final UniDialogUtils.DialogConfig _DialogConfig;
		
		public SingleAdapter(Activity parentActivity, List<T> listItems, SingleItemReceiver<T> resultsReceiver, Dialog dialog
				, UniDialogUtils.DialogConfig dialogConfig) {
			super(parentActivity, 0, listItems);
			_Inflater = LayoutInflater.from(parentActivity);
			_ListItems = listItems;
			_ResultsReceiver = resultsReceiver;
			_Dialog = dialog;
			_DialogConfig = dialogConfig;
		}
		public int getCount() {
			return _ListItems.size(); }
		public T getItem(int position) { 
			return _ListItems.get(position); }
		public long getItemId(int position) {
			return position; }
		
		@SuppressWarnings("unchecked")
		public View getView(int position, View convertView, ViewGroup parent) {
			UniDialogUtils.ViewHolder holder;				
			if (convertView == null) {    
				convertView = _Inflater.inflate(R.layout.unidialog_selector_row, null);
				holder = new UniDialogUtils.ViewHolder();

				ViewHolder.initializeHolderControls(convertView, holder, _DialogConfig);				
				holder.CheckBox.setVisibility(View.GONE);				
				holder.TextView.setOnClickListener(new android.view.View.OnClickListener() {						
					@Override
					public void onClick(View v) {
						T di = (T)v.getTag();
						if (_ResultsReceiver != null) {
							_ResultsReceiver.onSelectItem(di); //user selects item in dialog
						}
						_Dialog.dismiss();
					}
				});	
				convertView.setTag(holder);
			} else {
				holder = (UniDialogUtils.ViewHolder)convertView.getTag();
			}			
			T di = _ListItems.get(position);
			ViewHolder.setDataToHolder(di, _DialogConfig, holder);
			return convertView;
		}
		
		/** Notification: internal list of items was changed  */
		@Override public void notify(Void param) {
			this.notifyDataSetChanged();			
		}
	}
}
