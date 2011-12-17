package com.kbitubit.android.demo;

import java.util.ArrayList;
import java.util.List;

import android.app.Activity;
import android.content.res.Resources;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.drawable.Drawable;
import android.os.Bundle;

public class DemoSelectDialogsActivity extends Activity {
    private static String[] items = {"Single dialog 1", "Single dialog 2", "Multy dialog 1", "Multy dialog 2", "Exit"};
    private static String[] descriptions = {"Single select dialog with list of strings. No images, no descriptions.."
    	, "Context menu with SimplestDataItem. Images and descriptions are visible"
    	, "Multy select dialog with custom items. They support images but doesn't support desctiptions"
    	, "Multy select dialog with SimplestDataItem. Images and descriptions are visible"
    	, "Quit the application"};
    private static int[] images = {R.drawable.i1, R.drawable.i2, R.drawable.i3, R.drawable.i4, 0};
		
    /** Called when the activity is first created. */
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.main);
        
   		show_main_dialog();
    }

	private void show_main_dialog() {
		//prepare data for main dialog
		        UniDialogUtils.DialogConfig dc = new UniDialogUtils.DialogConfig("Main dialog", true, true);
		        ArrayList<UniDialogUtils.SimplestDataItem> list = new ArrayList<UniDialogUtils.SimplestDataItem>();
		        for (int i = 0; i < items.length; ++i) {
		        	list.add(new UniDialogUtils.SimplestDataItem(i, items[i]
		        	  , drawable2Bitmap(this.getResources(), images[i])
		        	  , null
		        	  , descriptions[i]));
		        };
		        
		   //create main dialog
		        UniDialogSingleSelector.showDialog(this, dc, list
		        	, new UniDialogSingleSelector.SingleItemReceiver<UniDialogUtils.SimplestDataItem>() {
		        		/** User has selected item. Make additional actions*/
						@Override public void onSelectItem(UniDialogUtils.SimplestDataItem selectedItem) {
							if (selectedItem == null) finish();
							switch (selectedItem.Id) {
							case 0: show_single_dialog_1(); break;
							case 1: show_single_dialog_2(); break;
							case 2: show_multy_dialog_1(); break;
							case 3: show_multy_dialog_2(); break;
							default: finish(); break;
							}
						}				
		        });
	}
    
    private static String[] sub_items = {"Item 1", "Item 2", "Item 3", "Item 4"};
    private static String[] sub_descriptions = {"Description 1", "Description 2", "Description 3", "Description 4"};
    private static int[] sub_images = {R.drawable.i1, R.drawable.i2, R.drawable.i3, R.drawable.i4};
    
    /** Show list of strings*/
	private void show_single_dialog_1() {
		UniDialogUtils.DialogConfig dc = new UniDialogUtils.DialogConfig(items[0], false, false);
		ArrayList<String> list_subitems = new ArrayList<String>();
		for (String s: sub_items) list_subitems.add(s);
		
        UniDialogSingleSelector.showDialog(this, dc, list_subitems
            	, new UniDialogSingleSelector.SingleItemReceiver<String>() {
            		/** User has selected item. Make additional actions*/
    				@Override public void onSelectItem(String selectedItem) {
    					show_results(selectedItem);
    				}				
            });
	}    
    
	/** Show list of SimplestDataItem */
	private void show_single_dialog_2() {
		UniDialogUtils.DialogConfig dc = new UniDialogUtils.DialogConfig(items[1], true, true);
		ArrayList<UniDialogUtils.SimplestDataItem> list_subitems = new ArrayList<UniDialogUtils.SimplestDataItem>();
		for (int i = 0; i < sub_items.length; ++i) {
			list_subitems.add(new UniDialogUtils.SimplestDataItem(i, sub_items[i]
			      , drawable2Bitmap(this.getResources(), sub_images[i]), i, sub_descriptions[i]));
		}
		
        UniDialogSingleSelector.showDialog(this, dc, list_subitems
            	, new UniDialogSingleSelector.SingleItemReceiver<UniDialogUtils.SimplestDataItem>() {
            		/** User has selected item. Make additional actions*/
    				@Override public void onSelectItem(UniDialogUtils.SimplestDataItem selectedItem) {
    					show_results(selectedItem);
    				}				
            });
	}

	/** Custom data item. Image is stored as resourceId (not bitmap)
	 * Attention: class doesn't support ItemDescriptionProvider * */
	class CustomDataItem implements UniDialogUtils.ItemImageProvider {
		public final String Title;
		private final int DrawableId;
		public CustomDataItem(String title, int drawableId) {
			this.Title = title;
			this.DrawableId = drawableId;			
		}
		@Override public Bitmap getImage() {
			if (this.DrawableId == 0) return null; 
			return DemoSelectDialogsActivity.drawable2Bitmap(DemoSelectDialogsActivity.this.getResources(), this.DrawableId);
		}
		@Override public String toString() {
			return this.Title;
		}
	}
	
	/** Show list of CustomDataItem*/
	private void show_multy_dialog_1() {
		UniDialogUtils.DialogConfig dc = new UniDialogUtils.DialogConfig(items[2]
		        , true //UniDialogUtils.SimplestDataItem supports images
				, true //UniDialogUtils.SimplestDataItem supports desctiptions
		);
		ArrayList<CustomDataItem> list_subitems = new ArrayList<CustomDataItem>();
		for (int i = 0; i < sub_items.length; ++i) {
			list_subitems.add(new CustomDataItem(sub_items[i], sub_images[i]));
		}
		
        UniDialogMultySelector.showDialog(this, dc, list_subitems
            	, new UniDialogMultySelector.MultyItemsReceiver<CustomDataItem>() {
    		/** User has selected item. Make additional actions*/
				@Override public void OnSelectItems(List<CustomDataItem> selectedItems) {
					show_results(selectedItems);
				}
            }
        	, true //by default select all items
        );		
	}

	/** Show list of SimplestDataItem*/
	private void show_multy_dialog_2() {
		UniDialogUtils.DialogConfig dc = new UniDialogUtils.DialogConfig(items[3]
		        , true //CustomDataItem supports images
				, false //CustomDataItem doesn't support desctiptions
		);
		ArrayList<UniDialogUtils.SimplestDataItem> list_subitems = new ArrayList<UniDialogUtils.SimplestDataItem>();
		for (int i = 0; i < sub_items.length; ++i) {
			list_subitems.add(new UniDialogUtils.SimplestDataItem(i, sub_items[i]
			      , drawable2Bitmap(this.getResources(), sub_images[i]), i, sub_descriptions[i]));
		}
	//prepare list of selected items: f.e. first two items
		ArrayList<UniDialogUtils.SimplestDataItem> selected_items = new ArrayList<UniDialogUtils.SimplestDataItem>();
		for (int i = 0; i < 2; ++i) selected_items.add(list_subitems.get(i));
		
        UniDialogMultySelector.showDialog(this, dc, list_subitems
            	, new UniDialogMultySelector.MultyItemsReceiver<UniDialogUtils.SimplestDataItem>() {
    		/** User has selected item. Make additional actions*/
				@Override public void OnSelectItems(List<UniDialogUtils.SimplestDataItem> selectedItems) {
					show_results(selectedItems);
				}
            }
        	, selected_items //by default first two items are selected
        );		
	}

	private void show_results(Object selectedItem) {
		if (selectedItem == null) {
			show_main_dialog();
		} else {
			DemoSelectDialogsActivity.this.finish();
		}
	}


    
	public static Bitmap drawable2Bitmap(Resources srcResources, int idDrawable) {
		if (idDrawable == 0 || srcResources == null) return null;
		Drawable drw = srcResources.getDrawable(idDrawable);
		int w = drw.getIntrinsicWidth();
		int h = drw.getIntrinsicHeight();
		Bitmap dest = Bitmap.createBitmap(w, h, Bitmap.Config.ARGB_8888);
		Canvas cc = new Canvas(dest);
		drw.mutate().setBounds(0, 0, w, h); //используем буфер, чтобы избежать выделения памяти под Rect
		drw.draw(cc);
		return dest;	
	}    
}