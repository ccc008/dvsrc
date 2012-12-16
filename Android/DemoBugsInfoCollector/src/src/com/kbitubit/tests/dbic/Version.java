package com.kbitubit.tests.dbic;

public class Version {
	public static final boolean IS_PRO = false;
	public static final boolean IS_DEBUG = true;
	public static final boolean IS_TEST = false;
	/** В отладочной версии FREE версия ведет себя как FREE, даже если установлена PRO */
	public static final boolean IS_FREE_IS_FREE_IN_DEBUG = false;
	
	public static int getFreeProId(int idFree, int idPro) {
		return IS_PRO ? idPro : idFree; 
	}
	public static int getFreeId(int idFree) {
		return IS_PRO ? 0 : idFree; 
	}
	public static int getProId(int idPro) {
		return IS_PRO ? idPro : 0; 
	}
	public static String getPackageName() {
		if (IS_PRO) {
			return "com.kbitubit.tests" + ".pro.dbic";
		} else if (IS_TEST) {
			return "com.kbitubit.tests" + ".test.dbic";
		} else {
			return "com.kbitubit.tests" + ".dbic";
		}
	}
}
