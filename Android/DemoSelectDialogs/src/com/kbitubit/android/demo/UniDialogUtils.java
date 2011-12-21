package com.kbitubit.android.demo;

//UniDialog implementation
//Copyright 2011 by Victor Derevyanko, dvpublic0@gmail.com
//http://derevyanko.blogspot.com/2011/12/android.html
//http://code.google.com/p/dvsrc/downloads/detail?name=20111217DemoSelectDialog.7z&can=2&q=#makechanges

import android.graphics.Bitmap;
import android.view.View;
import android.widget.CheckBox;
import android.widget.TextView;
import android.widget.ImageView;

public class UniDialogUtils {
	
	/** Configuration of the dialog */
	public static class DialogConfig {
		public DialogConfig(String dialogTitle) {
			this(dialogTitle, false, false);
		}
		public DialogConfig(String dialogTitle, boolean supportImages, boolean supportDescriptions) {
			this.DialogTitle = dialogTitle;
			this.EnableImages = supportImages;
			this.EnableDescriptions = supportDescriptions;
		}		
		public final String DialogTitle;
		public final Boolean EnableImages;
		public final Boolean EnableDescriptions;
	}
	
	/** If T objects implement this interface then dialog will be able to display images in the list
	 * (EnableImages in DialogConfig should be set to true).*/
	public static interface ItemImageProvider {
		Bitmap getImage();
	}
	
	/** If T objects implement this interface then dialog will be able to display descriptions in the list.
	 * (EnableDescriptions in DialogConfig should be set to true). */
	public static interface ItemDescriptionProvider {
		String getDescription();
	}	
	
	/** Send notification to adapter about changes in the items list*/
	public interface INotification<Param> {
		public abstract void notify(Param param);
	}	
	
	/** Simplest implemetation of DataItem for DialogSingleSelector. Item contains id, title, image and tag */
	public static class SimplestDataItem implements UniDialogUtils.ItemImageProvider, ItemDescriptionProvider {
		public final String Title;
		public final Bitmap Icon;
		public final int Id;
		public final Object Tag;
		public final String Description;
		public SimplestDataItem(int srcId, String title, Bitmap bmp, Object tag, String description) {
			this.Title = title;
			this.Icon = bmp;
			this.Id = srcId;
			this.Tag = tag;
			this.Description = description;
		}
		public SimplestDataItem(int srcId, String title, Bitmap bmp, Object tag) {
			this(srcId, title, bmp, tag, null);
		}
		@Override public String toString() {				
			return this.Title;
		}
		@Override public Bitmap getImage() {
			return this.Icon;
		}
		@Override public String getDescription() {
			return this.Description;
		}			
	}	
	/** Auxilary class for adapters implementation */
	public static class ViewHolder {
		public TextView TextView;
		public ImageView ImageView;
		public CheckBox CheckBox;
		public TextView DescriptionView;
		
		/** Common initialization of item controls for both (multy and single) dialogs */
		public static void initializeHolderControls(View mainView, ViewHolder holder, DialogConfig dialogProperties) {
			holder.TextView = (TextView) mainView.findViewById(R.id.unidialog_text);
			holder.CheckBox = (CheckBox) mainView.findViewById(R.id.unidialog_checkbox);
			
			assert(holder.TextView != null);
			assert(holder.CheckBox != null);

			holder.ImageView = (ImageView) mainView.findViewById(R.id.unidialog_image);
			if (! dialogProperties.EnableImages) {
				if (holder.ImageView != null) {
					holder.ImageView.setVisibility(View.GONE);
				}
			} 
			holder.DescriptionView = (TextView) mainView.findViewById(R.id.unidialog_description);
			if (! dialogProperties.EnableDescriptions) {
				if (holder.DescriptionView != null) {
					holder.DescriptionView.setVisibility(View.GONE);
				}
			} 			
		}
		
		/** Common procedure to set item data to controls */
		public static<T> void setDataToHolder(T di, DialogConfig dialogConfig, ViewHolder holder) {
			holder.TextView.setTag(di);
			holder.TextView.setText(di.toString());
			if (dialogConfig.EnableImages && di instanceof UniDialogUtils.ItemImageProvider) {
				holder.ImageView.setImageBitmap(((UniDialogUtils.ItemImageProvider)di).getImage());
			}
			if (dialogConfig.EnableDescriptions && di instanceof UniDialogUtils.ItemDescriptionProvider) {
				holder.DescriptionView.setText(((UniDialogUtils.ItemDescriptionProvider)di).getDescription());
			}
		}

	}		
}
