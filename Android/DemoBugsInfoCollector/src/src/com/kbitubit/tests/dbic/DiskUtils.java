package com.kbitubit.tests.dbic;

import android.os.Environment;

public class DiskUtils {
	/** Получить доступ / создать если не существует
	 * специальную директорию для хранения данных приложения.
	 * Например, cache и customimages.
	 * Директории могут быть независимы для Pro и Free
	 * или общими для обоих пакетов (задается через packageIndependent).
	 * */
	public static java.io.File getSpecDirectory(Kernel kernel, String directoryName, boolean packageIndependent) {
		if (! isSDCardMounted()) return null;
		
		final java.io.File storage = android.os.Environment.getExternalStorageDirectory();
		if (! (storage.exists() && storage.canWrite()) ) return null;
		
		final java.io.File dir = packageIndependent
			? new java.io.File(storage, Version.getPackageName() + '/' + directoryName)
			: new java.io.File(storage.getAbsolutePath() + "/Android/data/"
				, kernel.getPackageName() + "/" + directoryName);	
		
		if (! dir.isDirectory()) {
			if (! dir.mkdirs()) return null;
		}
		return dir;	
	}

	public static boolean isSDCardMounted() {
		String state = Environment.getExternalStorageState();
		if (! ((Environment.MEDIA_MOUNTED.equals(state)) || Environment.MEDIA_MOUNTED_READ_ONLY.equals(state)) ) { 
			return false;  //устройство отсутствует или не доступно
		}
		return true;
	}
}
