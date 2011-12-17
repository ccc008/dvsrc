package com.kbitubit.android.demo;

import com.kbitubit.android.demo.R;
import com.kbitubit.android.demo.UniDialogUtils.ViewHolder;

import java.util.AbstractCollection;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;

import android.app.Activity;
import android.app.Dialog;
import android.content.DialogInterface;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.ListView;

/** Dialog to select several items. */
public class UniDialogMultySelector {
	
	/** Show dialog and suggest to select multy items from list of items.
	 * Returns list of selected items or null, if user canceled dialog
	 * bSelectAllByDefault - select/unselect all items by default */	
	public static<T> UniDialogUtils.INotification<Void> showDialog(Activity activity
			, UniDialogUtils.DialogConfig dialogProperties
			, List<T> listItems
			, MultyItemsReceiver<T> resultReceiver
			, boolean bSelectAllByDefault) {
		return show_dialog(activity, dialogProperties, listItems, resultReceiver, bSelectAllByDefault);
	}
	
	/** Shows dialog and suggests to select multy items from list of items.
	 * Returns list of selected items or null, if user canceled dialog
	 * selectedItemsByDefault - list of items (from listItems) that should be selected by default */
	public static<T> UniDialogUtils.INotification<Void> showDialog(Activity activity
			, UniDialogUtils.DialogConfig dialogConfig
			, List<T> listItems
			, MultyItemsReceiver<T> resultReceiver
			, List<T> selectedItemsByDefault) {
		return show_dialog(activity, dialogConfig, listItems, resultReceiver, selectedItemsByDefault);
	}	

	private static<T> UniDialogUtils.INotification<Void> show_dialog(Activity activity
			, UniDialogUtils.DialogConfig dialogConfig
			, List<T> listItems
			, MultyItemsReceiver<T> resultReceiver
			, Object selectedItemsByDefault) 
	{
		Dialog dialog = new Dialog(activity);

		dialog.setContentView(R.layout.unidialog_selector);
		dialog.setTitle(dialogConfig.DialogTitle); 
	
		dialog.setOwnerActivity(activity);
		
		ListView list_view = (ListView) dialog.findViewById(R.id.unidialog_listview);
		MultyAdapter<T> adapter = new MultyAdapter<T>(activity, listItems, selectedItemsByDefault, dialogConfig);
		list_view.setAdapter(adapter);
		
		Button bt_ok = (Button)dialog.findViewById(R.id.unidialog_button_ok);
		bt_ok.setOnClickListener(new OnButtonOkClickListener<T>(resultReceiver, adapter, false, dialog));			
		Button bt_cancel = (Button)dialog.findViewById(R.id.unidialog_button_cancel);
		bt_cancel.setOnClickListener(new OnButtonOkClickListener<T>(resultReceiver, adapter, true, dialog));			
		
		dialog.setOnCancelListener(new MultyCancelListener<T>(resultReceiver));			
		dialog.show();
		
		return adapter;
	}
	
	/** * listener: receives list of selected items OR null if user has canceled the dialog */
	public static interface MultyItemsReceiver<T> {
		void OnSelectItems(List<T> selectedItems); 
	}	

	/** Ok/Cancel buttons handler */
	private static class OnButtonOkClickListener<T> implements View.OnClickListener {
		private final MultyItemsReceiver<T> _ResultsReceiver;
		private final MultyAdapter<T> _Adapter;
		private final boolean _Cancel;
		private final Dialog _Dialog;
		public OnButtonOkClickListener(MultyItemsReceiver<T> resultReceiver, MultyAdapter<T> adapter, boolean bCancel, Dialog dialog) {
			this._ResultsReceiver = resultReceiver;
			this._Cancel = bCancel;
			this._Adapter = adapter; 
			this._Dialog = dialog;
		}
		@Override
		public void onClick(View arg0) {
			if (this._Cancel) {
				this._ResultsReceiver.OnSelectItems(null);
			} else {
				this._ResultsReceiver.OnSelectItems(this._Adapter.getSelectedItems());
			}
			this._Dialog.dismiss();
		}		
	}
	
	/** Cancel handler */
	private static class MultyCancelListener<T> implements DialogInterface.OnCancelListener {
		private final MultyItemsReceiver<T> _ResultsReceiver;
		public MultyCancelListener(MultyItemsReceiver<T> resultReceiver) {
			_ResultsReceiver = resultReceiver;
		}
		@Override public void onCancel(DialogInterface arg0) {
			if (_ResultsReceiver != null) { //user canceled dialog
				_ResultsReceiver.OnSelectItems(null);
			}
		} 
	}

	private static class MultyAdapter<T> extends ArrayAdapter<T> implements UniDialogUtils.INotification<Void> {
		private LayoutInflater _Inflater;
		private final List<T> _ListItems;
		private final HashSet<T> _SelectedItems = new HashSet<T>();
		private final UniDialogUtils.DialogConfig _DialogConfig;
	
		@SuppressWarnings("unchecked")
		public MultyAdapter(Activity parentActivity, List<T> listItems, Object selectedItemsByDefault, UniDialogUtils.DialogConfig dialogConfig) {
			super(parentActivity, 0, listItems);
			_Inflater = LayoutInflater.from(parentActivity);
			_ListItems = listItems;
			_DialogConfig = dialogConfig;
		//select items that should be selected by default
			if (selectedItemsByDefault instanceof Boolean) {
				if ((Boolean)selectedItemsByDefault) {
					for (T item : listItems) {
						_SelectedItems.add(item);
					}
				}
			} else if (selectedItemsByDefault instanceof AbstractCollection<?>) {
				for (Object item : (AbstractCollection<?>)selectedItemsByDefault) {
					_SelectedItems.add((T)item);
				}				
			}
		}

		@SuppressWarnings("unchecked")
		public List<T> getSelectedItems() {
			List<T> list = new ArrayList<T>();
			Object[] data = _SelectedItems.toArray();
			for (Object obj : data) {
				list.add((T)obj);
			}
			return list;
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
				holder.CheckBox.setOnClickListener(new View.OnClickListener() {
			    	public void onClick(View v) {
			    		T dialog_item = (T)v.getTag();
			    		CheckBox cb = (CheckBox)v;
			    		if (cb.isChecked()) {
			    			if (! _SelectedItems.contains(dialog_item)) {
			    				_SelectedItems.add(dialog_item);
			    			}
			    		} else {
			    			if (_SelectedItems.contains(dialog_item)) {
			    				_SelectedItems.remove(dialog_item);
			    			}			    			
			    		}
			    	}
				});					
				convertView.setTag(holder);
			} else {
				holder = (UniDialogUtils.ViewHolder)convertView.getTag();
			}			
			T di = _ListItems.get(position);
			ViewHolder.setDataToHolder(di, _DialogConfig, holder);
			holder.CheckBox.setTag(di);
			holder.CheckBox.setChecked(_SelectedItems.contains(di));

			return convertView;
		}

		/** Notification: internal list of items was changed  */
		@Override public void notify(Void param) {
			this.notifyDataSetChanged();			
		}		
	}	
}
