package com.kbitubit.tests.dbic;

/** list of error codes for apsalar */
public final class ErrorCodes {
	/** List of "interesting" errors.
	 * If such error happens we should silently send report about it to google doc (TEST version only) */
	private final static int INTERESTING_ERRORS[] = {
		2, 3, 4
	};
	public static boolean isErrorInteresting(int errorCode) {
		for (int i = 0; i < INTERESTING_ERRORS.length; ++i) {
			if (INTERESTING_ERRORS[i] == errorCode) return true;
		}
		return false;
	}
	
	public final static int E1 = 1; //apsalar initalization
	public final static int E2 = 2;
	
//these codes are not used yet
	public final static int E3 = 3; 
	public final static int E4 = 4;
	public final static int E5 = 5;
	public final static int E6 = 6;
	public final static int E7 = 7;
	public final static int E8 = 8; 
	public final static int E9 = 9;
	public final static int E10 = 10;
	public final static int E11 = 11; 
	public final static int E12 = 12;
	public final static int E13 = 13;
	public final static int E14 = 14; 
	public final static int E15 = 15; 
	public final static int E16 = 16;
	public final static int E17 = 17; 
	public final static int E18 = 18;
	public final static int E19 = 19;
	public final static int E20 = 20;
	public final static int E21 = 21;
	public final static int E22 = 22;
	public final static int E23 = 23;
	public final static int E24 = 24;
	public final static int E25 = 25; 
	public final static int E26 = 26;
	public final static int E27 = 27;
	public final static int E28 = 28; 
	public final static int E29 = 29;
	public final static int E30 = 30;
	public final static int E31 = 31; 
	public final static int E32 = 32;
	public final static int E33 = 33;
	public final static int E34 = 34;
	public final static int E35 = 35; 
	public final static int E36 = 36;
	public final static int E37 = 37; 
	public final static int E38 = 38; 
	public final static int E39 = 39;
	public final static int E40 = 41;
	public final static int E41 = 41;
	public final static int E42 = 42; 
	public final static int E43 = 43;
	public final static int E44 = 44; 
	public final static int E45 = 45; 
	public final static int E46 = 46; 
	public final static int E47 = 47;
	public final static int E48 = 48;
	public final static int E49 = 49; 
	public final static int E50 = 50; 
	public final static int E51 = 51; 
	public final static int E52 = 52;
	public final static int E53 = 53;
	public final static int E54 = 54;	
	public final static int E55 = 55; 
	public final static int E56 = 56;
	public final static int E57 = 57;
	public final static int E58 = 58;
	public final static int E59 = 59;
	public final static int E60 = 60; 
	public final static int E61 = 61;
	public final static int E62 = 62; 
	public final static int E63 = 63; 
	public final static int E64 = 64; 
	public final static int E65 = 65; 
	public final static int E66 = 66; 
	public final static int E67 = 67; 
	public final static int E68 = 68; 
	public final static int E69 = 69; 
	public final static int E70 = 70; 
	public final static int E71 = 71; 
	public final static int E72 = 72; 
	public final static int E73 = 73; 
	public final static int E74 = 74; 
	public final static int E75 = 75; 
	public final static int E76 = 76; 
	public final static int E77 = 77; 
	public final static int E78 = 78;
	public final static int E79 = 79; 
	public final static int E80 = 80; 
	public final static int E81 = 81; 
	public final static int E82 = 82; 
	public final static int E83 = 83; 
	public final static int E84 = 84; 
	public final static int E85 = 85; 
	public final static int E86 = 86; 
	public final static int E87 = 87; 
	public final static int E88 = 88; 
	public final static int E89 = 89; 
	public final static int E90 = 90; 
	public final static int E91 = 91; 
	public final static int E92 = 92; 
	public final static int E93 = 93; 
	public final static int E94 = 94;
	public final static int E95 = 95; 
	public final static int E96 = 96; 
	public final static int E97 = 97;
	public final static int E98 = 98;
	public final static int E99 = 99;
	public final static int E100 = 100;	
}
