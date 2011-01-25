package com.google.code.dvsrc;

public class Version {
	public static final boolean IS_PRO = false;
	public static int GetProFreeId(int idFree, int idPro) {
		return IS_PRO ? idPro : idFree; 
	}
	public static int GetFreeId(int idFree) {
		return IS_PRO ? 0 : idFree; 
	}
	public static int GetProId(int idPro) {
		return IS_PRO ? idPro : 0; 
	}
}
